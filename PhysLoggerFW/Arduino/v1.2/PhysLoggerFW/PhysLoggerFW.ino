// Title:				PhysLogger 1.2 Firmware
// Supported Devices:	Atmega2560 and similar. (4 Differential ADC Channels with gains 1, 10x , 200x and 1 I2C port required).
// Date:				21 Mar 2019
// Author:				M U Hassan
// License:				GPL

//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
//|||||||||||||| Used Library Headers |||||||||||||||||||
//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
// include the PacketCommandsMini library by FivePointNine, it is optimized for speed and stability
#include "i2cInstrument.h"
#include "PacketCommands.h"
#include <Wire.h>
#include <EEPROM.h>
#include <avr/wdt.h>

//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
//|||||||||||||||| Function Headers |||||||||||||||||||||
//|||||||||||||||||||||||||||||||||||||||||||||||||||||||

// Custom AnalogRead Function for faster reading
// Channel values are 0, 1, 2, 3 for Differential modes of channels A, B, C, D,
// Channel values are 4, 5, 6, 7 for RSE modes of channels A, B, C, D. + pins of each channel are used for RSE.
// Gain = 0, 1, 2 for 1x, 10x, 200x on differential inputs. On RSE, gain setting gain has no effect, but it must be <=2
// Reads required MUX value from MUX table, applies it, starts a conversion, 
// waits for conversion to end, collects data, applies polarity correction, formats output and returns in int16 format.
uint16_t analogRead2(uint8_t channel, uint8_t gain);

// uses analogRead2 with averaged sampling
float analogRead3(uint8_t channel, uint8_t gain, uint32_t samplingTime = 5);
// Reset Gain information in Firing packet. We don't calculate values before every send, rather we save it everytime we change the gain or type change operation.
void ResetGainInDataPacket();
// Sends device signature on the Serial port via PacketCommandMini
void sendSignature();

// scans I2C bus for PhysInstruments
void ScanI2CBusForInstruments();

// Helper functions to send strings to PC
float GetFloatValueF(String message);
float GetFloatValue(String& message);
void ShowMessageFuncF(String message, bool asConsole = false, bool newLineFeed = false);
void ShowMessageFunc(String& message, bool asConsole = false, bool newLineFeed = false);
void ConsoleMayExitFunc();
void DisconnectRequestFunc();

//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
//||||||||||||||||| Global Defines ||||||||||||||||||||||
//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
// Shortcuts to set and clear bits in a register
// Also included in the arduino core but not exposed
#define sbi(sfr, bit) (_SFR_BYTE(sfr) |= _BV(bit))
#define cli(sfr, bit) (_SFR_BYTE(sfr) &= ~_BV(bit))
union fToA {
	float    F;
	uint8_t  A[sizeof(float)];
}; 
union ul32ToA {
	uint32_t    L;
	uint8_t  A[sizeof(uint32_t)];
};
// Packet Command IDs
// Some packets can share ids if both are uni directional
// we have only 8 possible packet ids from 0-7
#define GetHWSignatures 1  // IO
#define ChangeChannelType 2 // I
#define ChangeChannelGain 3 // I
#define DataPacket 2 // O
#define BeginFire 4 //I
#define ChangeSamplingTime 5 // IO
#define GetI2CInstruments 6 // I
#define i2cInstrumentSignature 3 // O
#define SetValue 4	// O
#define GetValue 7 // I

// Additional 256 values for set values
#define SetChannelCalibration 0
#define ShowMessage 1
#define ConsolePrint 2
#define ConsolePrintln 3
#define ConsolMessageTBC 4
#define ConsoleMayExit 5
#define DisconnectRequest 6

// Additional 256 values for get values
#define GetFloatTBC 0
#define GetFloat 1
#define GetKeyChar 2
#define ChangeI2CInstrumentRange 3
#define SetI2CInstrumentParameter 4

#define LoggerVersion String(F("PhysLogger1_2"))
// The isntrument buffer supports up to 6 Instruments in Logger 1.x
#define MaxInstruments 6

#define DNLTableOnEEPROM 0
#define INLTableOnEEPROM 96
#define TypeSelectorOnEEPROM 192
#define GainSelectorOnEEPROM 196
#define SamplingPeriodOnEEPROM 200


//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
//|||||||||||||| Variables on the Heap ||||||||||||||||||
//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
PacketCommandMini datacom = PacketCommandMini();
PacketCommandMini comDO;
// send data only when this is true
bool canFire = false;
// the last time when a sample was sent
uint32_t TimeSinceLastSample = 0;
// just a copy of millis used while sending the packet. This prevents error that may be caused by changing millis during a packet send
uint32_t lastMillis = 0;

// Gain selector of the 4 channels. Value[channelID] => [Value=0 > 1x, Value=1 > 10x, Value=2 > 200x]
uint8_t gainSelector[4];
// Type and channel seletor Value[ChannelID] = [Value=ChannelID+X] where X=0 > Differential, X=4 > RSE
// if typeSelector >= 128, it is an I2C channel
uint8_t typeSelector[4];
// sampling period in millis
uint32_t samplingPeriod = 1;
uint32_t blinkTime = 0;
uint32_t SetupSince = 0;
// We need to integrate and take average of the values between two consecutive conversions, 
// alternatively, we can implement a R-C filter  to do this averaging.
// This array will hold those values.
float a_averaged[4] = { 0,0,0,0 };
// in -512 to 511 format, the absolute values are 9 bit. So, Int10 to UInt10 conversion is needed.
// With data already residing in floats and Int16/UInt16 variables, we need to cast it manually. 
// So, at some stage, we will need to have the abs values of the -512 to 511 formated data.
uint16_t absa[4];
// this is where raw data of Int10 is stored in -512 to 511 format in Int16 values.
int16_t aIn_512To511Format[4];
// the same int 16 data casted as float for averaging.
float a_int16AsFloat[4];
int avaiableInstrumentsCount = 0;
i2cInstrument avaiableInstruments[MaxInstruments];
// Pointer to the instrument attached to a channel.
i2cInstrument* SelectedInstruments[4] = { 0,0,0,0 };

//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
//|||||||||||||| ADC MUX Values Table |||||||||||||||||||
//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
// This array holds the values for the 16 combinations of ADC Mux needed for 4 Channels and their respective 4 modes,
// being, Differential 1x, Differential 10x, Differential 200x and RSE 1x
// Now, since we are separating the mux value selection in [ChannelType][gain] scheme, and our RSE doesn't support higher gain 
// values, for two channelTypes (Diff, RSE), 3 gain values, 4 channels, we need an array of [4 x 2] by [3] = 8 x 3.
// The columns [4:7][0:2] would be the same. Since any gain config for RSE is going to result in same MUX value, 
// Ignoring the gain. 
// So, the proper way to address the MUX table would be,
// MUXPolarity [ChannelType*Channel Index] x [Selected Gain],
// where 
//		ChannelType = 0 for Differential, 1 for RSE, 
//		Channel Index = 0, 1, 2, 3 for A, B, C, D and 
//		Selected Gain = 0 for 1x, 1 for 10x, 2 for 200x.

uint8_t MUX[8][3] =
{
	//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
	//||||||||||||||| Diffrential Config ||||||||||||||||||||
	//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
	// Gain 1x, Gain 10x, Gain 200x
	/*Channel A*/{ 0b010000, 0b001001, 0b001011 },
	/*Channel B*/{ 0b011011, 0b001101, 0b001111 },
	/*Channel C*/{ 0b110000, 0b101001, 0b101011 },
	/*Channel D*/{ 0b111011, 0b101101, 0b101111 },
	
	//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
	//||||||||||||||||||| RSE Config ||||||||||||||||||||||||
	//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
	// Gain 1x, Gain 10x, Gain 200x
	/*Channel A*/{ 0b000000, 0b000000, 0b000000 },
	/*Channel B*/{ 0b000010, 0b000010, 0b000010 },
	/*Channel C*/{ 0b100000, 0b100000, 0b100000 },
	/*Channel D*/{ 0b100010, 0b100010, 0b100010 }

	// see! gains for the same channel are same for RSE. So, whatever the gain is selected, ADC will return value on A1 multiplied by 1
	// So, logically, there are 16 combinations only
	// Gain 1x Diff, Gain 10x Diff,	Gain 200x Diff, Gain 1x RSE
	//Channel A { 0b010000,		0b001001,		0b001011,		0b000000 },
	//Channel B { 0b011011,		0b001101,		0b001111,		0b000010 },
	//Channel C { 0b110000,		0b101001,		0b101011,		0b100000 },
	//Channel D { 0b111011,		0b101101,		0b101111,		0b100010 },

};

//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
//|||||||||||| ADC Polarity Values Table ||||||||||||||||
//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
// Same as the MUX, the resulting ADC conversions against each [ChannelType][gain]'th MUX value is going to have a polarity.
// so the proper way to address the polarity table is same as MUX table i.e. 
// MUXPolarity [ChannelType*Channel Index] x [Selected Gain],
// where 
//		ChannelType = 0 for Differential, 1 for RSE, 
//		Channel Index = 0, 1, 2, 3 for A, B, C, D and 
//		Selected Gain = 0 for 1x, 1 for 10x, 2 for 200x.
uint8_t MUXPolarity[8][3] =
{
	//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
	//||||||||||||||| Diffrential Config ||||||||||||||||||||
	//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
	// Gain 1x, Gain 10x,  Gain 200x
	/*Channel A*/{ 0,			1,		1 },
	/*Channel B*/{ 1,			1,		1 },
	/*Channel C*/{ 0,			1,		1 },
	/*Channel D*/{ 1,			1,		1 },

	//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
	//||||||||||||||||||| RSE Config ||||||||||||||||||||||||
	//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
	// Gain 1x, Gain 10x,  Gain 200x
	/*Channel A*/{ 1,		 1,			1 },
	/*Channel B*/{ 1,		 1,			1 },
	/*Channel C*/{ 1,		 1,			1 },
	/*Channel D*/{ 1,		 1,			1 }
	// Similar to MUX Values table logically, there are 16 combinations only for the polarity as well
	// Gain 1x Diff, Gain 10x Diff,	Gain 200x Diff, Gain 1x RSE
	//Channel A {  0,			 1,				1,				1 },
	//Channel B {  1,			 1,				1,				1 },
	//Channel C {  0,			 1,				1,				1 },
	//Channel D {  1,			 1,				1,				1 },
};

//// AVR is designed in a way that all 4 differential channels's inputs can be shorted with all the gain values.
//// eg. ADC+ is shorted to ADC- if a same pin is selected as the input, say ADC1, ADC1.
//// This way, we can calculate any offset in the ADC-Amplifier combination for the selected channel and gain.
//// this table contains those combinations. To make the table readable by analogRead2(), we need to make it in [4x][3] format.
//// meaning, that there are 4x rows (for each channel set) and 3 columns for each gain value.
//uint8_t RSEADCCalibMux_1_1VRef[1][3] =
//{
//	// MUX value to give ADC a 1.1V directly
//	{ 0b011110 , 0b011110, 0b011110 },
//};
//uint8_t RSEADCCalibMux_GNDRef[1][3] =
//{
//	// MUX value to give ADC 0V directly
//	{ 0b011111 , 0b011111, 0b011111 },
//};
// AVR is designed in a way that all 4 differential channels's inputs can be shorted with all the gain values.
// eg. ADC+ is shorted to ADC- if a same pin is selected as the input, say ADC1, ADC1.
// This way, we can calculate any offset in the ADC-Amplifier combination for the selected channel and gain.
// this table contains those combinations. To make the table readable by analogRead2(), we need to make it in [4x][3] format.
// meaning, that there are 4x rows (for each channel set) and 3 columns for each gain value.
uint8_t DifferentialADCCalibMux[4][3] =
{
		// 		   1x		 10x	200x
	/*Channel A*/{ 0b010001, 0b001000, 0b001010 },
	/*Channel B*/{ 0b011010, 0b001100, 0b001110 },
	/*Channel C*/{ 0b110001, 0b101000, 0b101010 },
	/*Channel D*/{ 0b111010, 0b101100, 0b101110 }
};

float INLCorrectionTable[8][3] =
{
	{ 0, 0, 0 },
	{ 0, 0, 0 },
	{ 0, 0, 0 },
	{ 0, 0, 0 },
	{ 0, 0, 0 },
	{ 0, 0, 0 },
	{ 0, 0, 0 },
	{ 0, 0, 0 }
};
float DNLCorrectionTable[8][3] =
{
	{ 1, 1, 1 },
	{ 1, 1, 1 },
	{ 1, 1, 1 },
	{ 1, 1, 1 },
	{ 1, 1, 1 },
	{ 1, 1, 1 },
	{ 1, 1, 1 },
	{ 1, 1, 1 }
};

//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
//||||||||||||||| Data Sample format ||||||||||||||||||||
//|||||||||||||||||||||||||||||||||||||||||||||||||||||||

// ADC converts data in 10 bit format in all cases. 
// in RSE, it converts to [0, 1023] values from [0, Vref] and in Diff, to [-512, 511] format from [-Vref, +Vref]
// Consequently, we are sending data in 2 formats.
// 1 >> -512 to 511 format. 
// 2 >> 0 to 1023 format.
// Values are measured on the same reference, so the resolution is half in Differential.
// The diff channels use -512 to 511 for all gains, and RSE uses 0-1023. Instruments can use any of them.
// But they MUST use -512 to 511 as a convention
// PhysLogger 1 requires that the the values fall within -512 to 511 range or [0, 1023] range for sending.
// So, only those instruments are supported which through values within this range regardless of 
// the data type and quantity measured.
// e.g. Digital Caliper converts displacement 0-70mm into -512 to 511 format such that each 1mm = 10 divisions.
// so, with an offset of -500, the problem is solved. 0mm = -500, 1mm = -490, 50mm = 0, 100mm = 500
// we need to integrate the voltage during two successive samples and devide with dt to get the average voltage. 
// So, the resolution of a digital instrument will be altered during this conversion.


//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
//||||||||||||||| Data Packet format ||||||||||||||||||||
//|||||||||||||||||||||||||||||||||||||||||||||||||||||||
//
// In either ways of data format, we will have to have,
// >> 10 bits per channel for data and
// >> 2 bits for gain information, which can also be translated into the selected data encoding if cleverly desgined, which we have :),
// making a 12 bit data chunk per channel.
// For 4 such channels, a total of 48 bits (6 bytes) are required. A rounded number as well, right!
// In addition, we need to send a PacketID, packet length, check sum, time and start bytes.
// We have selected 0xAA as the start byte,
// 3 bits for the data packet, making possible a total of 8 data packet types,
// 5 bits for length, allowing a max of 32 bytes per command and
// 8 bits for checksum. our data integrity is important. Better discard one than misinterpretting it.
// 8 packets are not a good number, So, the packets are going to share IDs. 
// Meaning that if we do not expect to receive certain packet type that we send from a node, we can allow the other node to use the same
// packet in other types as well.
// So, the packet structure for the PacketCommand Mini is:
// We also need to send time in millis for each packet. With a 32 bit packet, we get a good maximum of ~53 days log length. 
// Since we don't need the logs to be so long, we can trim the time information to 24 bytes. 
// this gives us a maximum of 256*256*256 = 16777216 millis, 16777.216 seconds, 279.62 minuts, 4.66 Hours >> 4:40Hours. 
// That is quite enough and appropriate for most of the experiments. 

// So, packing up this information in one packet becomes a simple task. 
// Standard Fire Data Packet Length = 48 bits (data) + 1 Byte Com Signature + 1 Bytes CSum, 3 Bytes Time in millis = 10 bytes.
// ComByte 0 = 0xAA
// ComByte 1 [2:0] = PacketID
// ComByte 1 [7:3] = Data Length
// ComByte 2 = CSum = 0x55 ^ ComByte0 ^ ComByte1 ^ PayloadByte1 ^ PayloadByte2 ... ^PayloadByteN
// ComByte (3:3+N) = PayloadBytes (0:N)

// PayloadByte 0 = Time[7:0]
// PayloadByte 1 = Time[15:8]
// PayloadByte 2 = Time[23:16]
// PayloadByte 3 = DataA as UInt10[7:0]
// PayloadByte 4 = DataB as UInt10[7:0]
// PayloadByte 5 = DataC as UInt10[7:0]
// PayloadByte 6 = DataD as UInt10[7:0]
// PayloadByte 7[1:0] = DataA as UInt10[9:8]
// PayloadByte 7[3:2] = DataB as UInt10[9:8]
// PayloadByte 7[5:4] = DataC as UInt10[9:8]
// PayloadByte 7[7:6] = DataD as UInt10[9:8]
// PayloadByte 8[1:0] = DataA as GainIndex
// PayloadByte 8[3:2] = DataB as GainIndex
// PayloadByte 8[5:4] = DataC as GainIndex
// PayloadByte 8[7:6] = DataD as GainIndex

// THE setup
void setup() {
	// UART over USB init
	Serial.begin(115200);
	if (CheckForCalibConnector())
	{
		CalibLoop();
		while (1);
	}
	// I2C init as master
	Wire.begin();
	// I2C default clock is 100kbps
	Wire.setClock(400000);
	// This isn't working at the moment. Don't know why!
	// Both data requests and writes are returning before timeout even if the instrument is not connected. 
	// Needs looking in to.
	Wire.setTimeout(100);
	
	// Initialize the Instrument structure (for sure)
	for (int i = 0; i < MaxInstruments; i++)
		avaiableInstruments[i] = i2cInstrument();

	// set defaults
	for (int i = 0; i < 4; i++)
		gainSelector[i] = 0; // gain 1
	for (int i = 0; i < 4; i++)
		typeSelector[i] = i;// Differential
	pinMode(LED_BUILTIN, 1);
	// Fix the datacom length. We don't want it to be re-assigned in every Fire.
	datacom.PayLoadLength(13);
	datacom.PacketID(DataPacket);
	// Set during debugging. Lost purpose, needs looking in to so that it can be removed.
	datacom.PayLoad()[8] = 0;

	// check for eeprom corruption
	for (int i = 0; i < 4; i++)
	{
		if (EEPROM.read(TypeSelectorOnEEPROM + i) > 128 + MaxInstruments) 
			EEPROM.write(TypeSelectorOnEEPROM + i, 0);
		if (EEPROM.read(GainSelectorOnEEPROM + i) > 3)
			EEPROM.write(GainSelectorOnEEPROM + i, 0);
	}
	uint32_t spBkp1 = samplingPeriod, spBkp2 = samplingPeriod, spBkp3 = samplingPeriod;
	if (EEPROM.get(SamplingPeriodOnEEPROM, spBkp1) < 1 ||
		EEPROM.get(SamplingPeriodOnEEPROM, spBkp2) > 10000)
		EEPROM.put(SamplingPeriodOnEEPROM, spBkp3);

	// Lets speed up the ADC by 2x
	// Data Sheet Table 26.5 on page 285
	analogRead2(0, 0);
	cli(ADCSRA, ADPS0);
	// This is how we begin. The Desktop App sends a hard reset using the Serial DTR. So, we send the signature back so that the app knows that its a logger.
	GetFromEEPROM(DNLCorrectionTable, 8, DNLTableOnEEPROM);
	GetFromEEPROM(INLCorrectionTable, 8, INLTableOnEEPROM);
	sendSignature();
	SetupSince = millis();
	//CalibrateForINLandDNL();

	wdt_disable();
	analogWrite(8, 128); // 2.5V


}
void CalibLoop()
{
	wdt_disable();
	sendSignature();
	uint8_t pwm[] = { 255,25,1 };
	float v[] = { 0,0,0 };
	ShowMessageFuncF(F("PhysLogger v1.2"), true, false);
	ShowMessageFuncF(F(" connected in calibration mode\r\n"));
	for (int i = 0; i < 3; i++)
	{
		analogWrite(8, pwm[i]);
		float f = 0;
		if (i == 2)
			f = GetFloatValueF(String(i + 1) + F(": Enter voltage at calibration pin (mV): ")) / 1000.0F;
		else
			f = GetFloatValueF(String(i + 1) + F(": Enter voltage at calibration pin (V): "));
		
		bool wrongV = false;
		if (i == 0)
			if (f < 3 || f > 4.3)
				wrongV = true;

		if (i == 1)
			if (f < 0.3 || f > 0.43)
				wrongV = true;
		if (i == 2)
			if (f < 0.008 || f > 0.03)
				wrongV = true;

		if (wrongV)
		{
			ShowMessageFuncF(F("\r\nThe value seems wrong. Kindly measure again\r\n"));
			i--;
		}
		else
		{
			v[i] = f;
		}
	}
	ShowMessageFuncF(F("Kindly remove the DMM and press any key to continue..."));
	GetKeyF(F(""));
	ShowMessageFuncF(F("Calibration has begun.\r\n"));
	for (int channel = 0; channel < 4; channel++)
	{
		for (int gain = 0; gain < 3; gain++)
		{
			CalibChannel(channel, gain, pwm[0], pwm[1], pwm[2], v[0], v[1], v[2]);
			ShowMessageFuncF(F("\r\n"));
		}
	}
	CalibrateForINL();
	for (int channel = 4; channel < 8; channel++)
	{
		CalibChannel(channel, 0, pwm[0], pwm[1], pwm[2], v[0], v[1], v[2]);
		ShowMessageFuncF(F("\r\n"));
		for (int gain = 0; gain < 3; gain++)
		{
			INLCorrectionTable[channel][gain] = INLCorrectionTable[channel][0];
			DNLCorrectionTable[channel][gain] = DNLCorrectionTable[channel][0];
		}
	}
	SaveToEEPROM(DNLCorrectionTable, 8, DNLTableOnEEPROM);
	SaveToEEPROM(INLCorrectionTable, 8, INLTableOnEEPROM);
	GetFromEEPROM(DNLCorrectionTable, 8, DNLTableOnEEPROM);
	GetFromEEPROM(INLCorrectionTable, 8, INLTableOnEEPROM);
	ShowMessageFuncF(F("\n\nSaved Data:"));
	for (int channel = 0; channel < 8; channel++)
	{
		for (int gain = 0; gain < 3; gain++)
		{
			ShowMessageFuncF(F("c["));
			ShowMessageFuncF(String(channel));
			ShowMessageFuncF(F("]["));
			ShowMessageFuncF(String(gain));
			ShowMessageFuncF(F("] > DNL = "));
			ShowMessageFuncF(String(DNLCorrectionTable[channel][gain], 3));
			ShowMessageFuncF(F(", INL = "));
			ShowMessageFuncF(String(INLCorrectionTable[channel][gain], 3));
			ShowMessageFuncF(F("\r\n"));
		}
	}
	bool results[] = { 1,1,1,1 };
	for (int channel = 0; channel < 8; channel++)
	{
		for (int gain = 0; gain < 3; gain++)
		{
			if (DNLCorrectionTable[channel][gain] > 1.1 || DNLCorrectionTable[channel][gain] < 0.9)
				results[channel % 4] = 0;
		}
	}
	bool all = true;
	bool some = false;
	for (int channel = 0; channel < 4; channel++)
	{
		if (results[channel])
			all = false;
		else
			some = true;
	}
	if (all)
	{
		ShowMessageFuncF(F("\r\nCalibration pin seems to be having hardware issues."));
	}
	else
	{
		for (int channel = 0; channel < 4; channel++)
		{
			if (!results[channel])
			{
				ShowMessageFuncF(F("\r\nChannel "));
				ShowMessageFuncF(String((char)(channel + 'A')));
				ShowMessageFuncF(F(" seems to be having hardware problems"));
			}
		}
	}
	if (some)
		ShowMessageFuncF(F("\r\nCalibration has failed."));
	else

		ShowMessageFuncF(F("\r\nCalibration has finished successfully."));
	ShowMessageFuncF(F("\r\nRestart the PhysLogger for the changes to take effect."));
	ConsoleMayExitFunc();
	DisconnectRequestFunc();
	while (1);
}
bool CheckForCalibConnector()
{
	pinMode(21, 1);
	pinMode(20, 0);
	digitalWrite(20, 1);
	for (int i = 0; i < 20; i++)
	{
		uint32_t n = random();
		digitalWrite(21, n % 2);
		delay(1);
		if (digitalRead(20) != n % 2)
			return 0;
		n >>= 1;
	}
	return 1;
}
// THE loop
void loop()
{
	if (millis() > 100)
	{
		wdt_reset();
	}
	// Check if there is data on serial
	if (Serial.available())
	{
		if (PacketCommand_FromStream(comDO, Serial) == ProtocolError_None)
		{
			if (comDO.PacketID() == GetHWSignatures) // Get Signature, not used
				sendSignature();
			else if (comDO.PacketID() == BeginFire) // Set the Firing status
			{
				if (comDO.PayLoad()[0] == 1)
					canFire = true;
				else
					canFire = 0;
				digitalWrite(LED_BUILTIN, 0);

				for (int i = 0; i < 4; i++)
				{
					EEPROM.write(TypeSelectorOnEEPROM + i, typeSelector[i]);
					EEPROM.write(GainSelectorOnEEPROM + i, gainSelector[i]);
				}
				EEPROM.put(SamplingPeriodOnEEPROM, samplingPeriod);
			}
			else if (comDO.PacketID() == GetI2CInstruments) 
				// Scan, begin and report i2c. This is called by the app before Firing starts. 
				// The desktop app sends this command only if it is logger 1.
			{
				//Search for I2C devices
				ScanI2CBusForInstruments();
				// We need to send, One, address, two, type of the instrument.
				// With Type, the Desktop App will look for equipment drivers and 
				uint8_t sig[2];
				for (int i = 0; i < avaiableInstrumentsCount; i++)
				{
					sig[0] = avaiableInstruments[i].InstrumentType;
					sig[1] = avaiableInstruments[i].Address;
					PacketCommandMini(i2cInstrumentSignature, sig, 2).SendCommand(Serial);
				}
			}
			else if (comDO.PacketID() == ChangeChannelGain) // Set Channel  gains
			{
				// gains should only be set if Diff channels are set.
				// The com has data in this format:
				// comDO.PayLoad()[0] = channel ID
				// comDO.PayLoad()[1] = gain index, 0 = 1x, 1 = 10x, 2 = 200x
				gainSelector[comDO.PayLoad()[0]] = (comDO.PayLoad()[1] && 0b11);
				EEPROM.write(GainSelectorOnEEPROM + comDO.PayLoad()[0], gainSelector[comDO.PayLoad()[0]]);
				// Reset Gain information in Firing packet
				ResetGainInDataPacket();
			}
			else if (comDO.PacketID() == ChangeChannelType) // Set Channel  type
			{
				// selected channel = comDO.PayLoad()[0] // 0 = Diff, 1 = RSE, >= 128 for I2c
				// When channel comDO.PayLoad()[0] >= 128, comDO.PayLoad()[0] - 128 is the I2C address of required device.
				// Selected Type = comDO.PayLoad()[1]
				if (comDO.PayLoad()[1] < 128) // Differential or RSE
				{
					typeSelector[comDO.PayLoad()[0]] = (comDO.PayLoad()[1] == 0 ? 0 : 4) + comDO.PayLoad()[0];
					// Reset Gain information in Firing packet
					ResetGainInDataPacket();
				}
				else // Change to Instrument
				{
					uint8_t address = comDO.PayLoad()[1] - 128;
					// Find the instrument on the stack
					for (int i = 0; i < avaiableInstrumentsCount; i++)
					{
						if (avaiableInstruments[i].Address == address)
						{
							// Attach the instrument to the required channel
							SelectedInstruments[comDO.PayLoad()[0]] = avaiableInstruments + i;
							typeSelector[comDO.PayLoad()[0]] = comDO.PayLoad()[1];
							// no need to change the gain. PhysLogger 1.x supports only 0b0 which is 1x and -512 to 511 encoded

							// Reset Gain information in Firing packet
							ResetGainInDataPacket();
						}
					}
				}

				EEPROM.write(TypeSelectorOnEEPROM + comDO.PayLoad()[0], typeSelector[comDO.PayLoad()[0]]);
			}
			else if (comDO.PacketID() == ChangeSamplingTime) // set sampling rate
			{
				// simple enough?
				samplingPeriod = 
					((uint32_t)(comDO.PayLoad()[0] + (comDO.PayLoad()[1] << 8) + (comDO.PayLoad()[1] << 16) + (comDO.PayLoad()[3] << 24)))
					;

				uint8_t rate[4];
				rate[0] = (samplingPeriod) >> 0;
				rate[1] = (samplingPeriod) >> 8;
				rate[2] = (samplingPeriod) >> 16;
				rate[3] = (samplingPeriod) >> 24;
				PacketCommandMini(ChangeSamplingTime, rate, 4).SendCommand(Serial);
				EEPROM.put(SamplingPeriodOnEEPROM, samplingPeriod);
			}
			else if (comDO.PacketID() == GetValue) // PC wants to set
			{
				if (comDO.PayLoad()[0] == ChangeI2CInstrumentRange)
				{
					for (int i = 0; i < avaiableInstrumentsCount; i++)
					{
						if (avaiableInstruments[i].Address == comDO.PayLoad()[1])
						{
							if (avaiableInstruments[i].SetParameter(1, comDO.PayLoad()[2]))
							{
								comDO.SendCommand(Serial);
							}
							else
								ShowMessageFuncF(F("The selected range couldn't be applied."));
						}
					}
				}
				else if (comDO.PayLoad()[0] == SetI2CInstrumentParameter)
				{
					// Quote from PhysLogger1_1.cs Line 53
					// 0	SetI2CInstrumentParameter
					// 1	(byte)instrument.InstrumentAddress, // instrument address
					// 2	command.ID, // param id
					// 3	1, // dataType from instrument types
					// 4	0,
					/* we need to append command value bytes here*/
				
					for (int i = 0; i < avaiableInstrumentsCount; i++)
					{
						if (avaiableInstruments[i].Address == comDO.PayLoad()[1])
						{
							if (comDO.PayLoad()[3] == dt_uint8)
								avaiableInstruments[i].SetParameter(comDO.PayLoad()[2], (byte)comDO.PayLoad()[4]);
							
						}
					}
				}
			}
		}
	}
	if (!canFire)
	{
	//	 this reduces the chances for session loss when a ripple causes the logger to reset by 25%
		if (millis() - SetupSince > 1000) // after reset, the computer doesn't take a lot of time before setting begin fire.
			// if the computer isn't responding, it may not be aware that the controller resetted.
			// restore to previous state.
		{
			// we need to do everything on our own now
			ScanI2CBusForInstruments();
			for (int i = 0; i < 4; i++)
			{
				typeSelector[i] = EEPROM.read(TypeSelectorOnEEPROM + i);
				if (typeSelector[i] >= 128) // i2c device
				{
					for (int i = 0; i < avaiableInstrumentsCount; i++)
					{
						if (avaiableInstruments[i].Address == typeSelector[i])
							SelectedInstruments[i] = avaiableInstruments + i;
					}
				}
			}
			for (int i = 0; i < 4; i++)
			{
				gainSelector[i] = EEPROM.read(GainSelectorOnEEPROM + i);
			}
			EEPROM.get(SamplingPeriodOnEEPROM, samplingPeriod);
			canFire = true;
		}
		// Blink twice every three seconds before a connection is made.
		if ((millis() - blinkTime) > 3000)
		{
			digitalWrite(LED_BUILTIN,
				((millis() - blinkTime) > 0 && (millis() - blinkTime) < 100) |
				((millis() - blinkTime) > 200 && (millis() - blinkTime) < 300));
		}
		blinkTime = millis();
		return;
	}
	// a value (up to 1) telling the factor of the latest value that will be added to the average. 
	// For slower sapling speeds, this factor should be low because, even with lesser factor, there are going to be more values in-between samples
	// this will make the averging time consistant on all speeds.
	float fac = min(10 / ((float)samplingPeriod), 1);
	// Read the data for all channels
	uint8_t channelSequence[] = { 0,1,2,3 };
	for (uint8_t cid_ = 0; cid_ < 4; cid_++)
	{
		uint8_t cid = channelSequence[cid_];
		// to reduce coupling between channels due to internal capacitance, we need to short the ADC+Gain terminals and give it some time to drain the charge.

		// Diff/RSE
		if (typeSelector[cid] < 128)
		{
			sbi(ADCSRA, ADEN);
			for (uint8_t ki = 0; ki < 60; ki++)
				ADMUX = DifferentialADCCalibMux[cid][gainSelector[cid]];
			cli(ADCSRA, ADEN);
			// this data can encoded in both formats. -512 to 511 and 0 to 1023. We need proper casting before sending.
			// Need float for averaging.
			a_int16AsFloat[cid] = (int16_t)analogRead2(typeSelector[cid], gainSelector[cid]);

			// this part needs to be re-written. 
			// We need to eliminate this kind of voltage compensation either in 
			// 1. the hardware,
			// 2. software correction, 
			// 3. or prefereably on PC app with a channel calibration table.
			//if (typeSelector[cid] < 4)
			//	a_int16AsFloat[cid] *= 4.0F / 3.0F;
			//else
			//	a_int16AsFloat[cid] *= 7.0F / 8.0F;


			// This is a software based attempt to remove INL and DNL which is supposed to be quite accurate
			// At least according to the datasheet, we can mitigate a lot of INL and DNL error in the readings;

		}
		else // its an i2c channel
		{
			// this returns values in -512 to 511 format
			a_int16AsFloat[cid] = SelectedInstruments[cid]->Read();
		}

		// RC filter
		a_averaged[cid] = a_int16AsFloat[cid] * fac + a_averaged[cid] * (1 - fac);
	}
	if (samplingPeriod > 10)
	{
		delay(1);
	}
	if (millis() - TimeSinceLastSample < samplingPeriod)
		return;
	TimeSinceLastSample = millis();
	datacom.PayLoad()[7] = 0; // bit 8 and 9 of our Int10 or UInt10 data.
	for (uint8_t cid = 0; cid < 4; cid++)
	{
		aIn_512To511Format[cid] = min(max(a_averaged[cid], -511), 511);
		absa[cid] = ((uint16_t)abs((int16_t)aIn_512To511Format[cid]));
		datacom.PayLoad()[3 + cid] = absa[cid] & 0xFF;
		datacom.PayLoad()[7] |=
			(((absa[cid] >> 8) & 0b1) << (2 * cid)) |
			(((((int16_t)aIn_512To511Format[cid]) < 0) ? 1 : 0) << (1 + cid * 2));
	}
	lastMillis = TimeSinceLastSample;
	datacom.PayLoad()[0] = lastMillis >> 0;
	datacom.PayLoad()[1] = lastMillis >> 8;
	datacom.PayLoad()[2] = lastMillis >> 16;
	datacom.SendCommand(Serial);
}

float analogRead3(uint8_t channel, uint8_t gain, uint32_t samplingTime)
{
	float v = 0;
	uint32_t count = 0;
	uint32_t start = millis();
	while (millis() - start < samplingTime)
	{
		v += (int16_t)analogRead2(channel, gain);
		count++;
	}
	return v / (float)count;
}
uint16_t analogRead2(uint8_t channel, uint8_t gain)
{
	return analogRead2(channel, gain, MUX);
}
// channel IDs < 4 are differential
// channel IDs >= 4 are RSE
// gain for RSE is fixed to 1
uint16_t analogRead2(uint8_t channel, uint8_t gain, uint8_t MuxTable[][3])
{
	if ((MuxTable[channel][gain] >> 5) % 2)
		ADCSRB |= 1 << MUX5;
	else
		ADCSRB &= ~(1 << MUX5);
	ADMUX =
		(1 << REFS1) | (1 << REFS0) | //  1.1V reference
									  //(0 << ADLAR) | //           left adjust result
		((MuxTable[channel][gain]) & 0b11111)
		;

	// Start Conversion
	sbi(ADCSRA, ADEN);
	sbi(ADCSRA, ADSC);
	while ((ADCSRA >> ADSC) & 0b1);
	uint8_t adcl = ADCL;
	uint8_t adch = ADCH;
	cli(ADCSRA, ADEN);
	if (channel < 4)
	{
		int result = adcl | ((adch & 0b1) << 8);
		if ((adch >> 1) & 1)
			result = result - 512;
		if (MUXPolarity[channel][gain])
			result = -result;
		return result;
	}
	else
	{
		int result = adcl | ((adch & 0b11) << 8);
		return result;
	}
}

// Sends device signature on the Serial port via PacketCommandMini
// send signature and the current sampling rate.
void sendSignature()
{
	String str = String(F("ver=")) + LoggerVersion;
	PacketCommandMini sigCom = PacketCommandMini(GetHWSignatures, (uint8_t*)str.c_str(), str.length());
	sigCom.SendCommand(Serial);
	uint8_t rate[4];
	rate[0] = (samplingPeriod) >> 0;
	rate[1] = (samplingPeriod) >> 8;
	rate[2] = (samplingPeriod) >> 16;
	rate[3] = (samplingPeriod) >> 24;
	PacketCommandMini rCom = PacketCommandMini(ChangeSamplingTime, (uint8_t*)rate, 4);
	rCom.SendCommand(Serial);
	// also send the calibration data
	PacketCommandMini calibDataCom;
	calibDataCom.PacketID(SetValue);
	calibDataCom.PayLoadLength(11); // com ID, channel, gain, INL[4], DNL[4]
	calibDataCom.PayLoad()[0] = SetChannelCalibration;
	for (int c = 0; c < 8; c++)
		for (int g = 0; g < 3; g++)
		{
			calibDataCom.PayLoad()[1] = c;
			calibDataCom.PayLoad()[2] = g;
			fToA inl;
			inl.F = INLCorrectionTable[c][g];
			for (int i = 0; i < 4; i++)
				calibDataCom.PayLoad()[3 + i] = inl.A[i];
			fToA dnl;
			dnl.F = DNLCorrectionTable[c][g];
			for (int i = 0; i < 4; i++)
				calibDataCom.PayLoad()[7 + i] = dnl.A[i];
			calibDataCom.SendCommand(Serial);
		}
}
// Updates the firing data packet gains with currently set values.
void ResetGainInDataPacket()
{
	// Fire data packet must also contain these gains for reference.
	// So, we need to change the details in datacom.PayLoad()[8] as well.				
	datacom.PayLoad()[8] = 0;
	for (int i = 0; i < 4; i++)
	{
		if (typeSelector[i] < 4) // Differential
			datacom.PayLoad()[8] |= ((gainSelector[i] & 0b11) << (i * 2));
		else if (typeSelector[i] < 8) // RSE
			datacom.PayLoad()[8] |= (0b11 << (i * 2)); // 0b11 isn't a vlid gain but it will tell the app that the data is 0-1023 encoded
		else if (typeSelector[i] >= 128) // I2C
			// the gain shouldn't be read for the instrument types. because they should use -512To511 format by default
			// PhysLogger 1.x doesn't support devices sending data in format other than -512 to 511 
			datacom.PayLoad()[8] |= (0b0 << (i * 2)); // default to -512to511 encoding 
	}
}
void ConsoleMayExitFunc()
{
	PacketCommandMini pc =  PacketCommandMini();
	pc.PacketID(SetValue);
	pc.PayLoadLength(1);
	pc.PayLoad()[0] = ConsoleMayExit;
	pc.SendCommand(Serial);
}
void DisconnectRequestFunc()
{
	PacketCommandMini pc =  PacketCommandMini();
	pc.PacketID(SetValue);
	pc.PayLoadLength(1);
	pc.PayLoad()[0] = DisconnectRequest;
	pc.SendCommand(Serial);
}
void ShowMessageFuncF(String message, bool asConsole = false, bool newLineFeed = false)
{
	ShowMessageFunc(message, asConsole, newLineFeed);
}

void ShowMessageFunc(String& message, bool asConsole = false, bool newLineFeed = false)
{
	int sent = 0;
	while (sent < message.length())
	{
		int toSend = min(message.length() - sent, 30);
		PacketCommandMini msgCom;
		msgCom.PacketID(SetValue);
		msgCom.PayLoadLength(toSend + 1);
		msgCom.PayLoad()[0] = asConsole ? (newLineFeed? ConsolePrintln :ConsolePrint ): ShowMessage;
		if ((toSend + sent < message.length()))
			msgCom.PayLoad()[0] = ConsolMessageTBC;

		for (int i = 0; i < toSend; i++)
			msgCom.PayLoad()[i + 1] = message.charAt(sent + i);
		msgCom.SendCommand(Serial);
		sent += toSend;
	}
}
uint8_t GetKeyF(String message)
{
	GetKey(message);
}
uint8_t GetKey(String& message)
{
	ShowMessageFunc(message, true, false);
	PacketCommandMini com;
	com.PacketID(GetValue);
	com.PayLoadLength(1);
	com.PayLoad()[0] = GetKeyChar;
	com.SendCommand(Serial);

	PacketCommandMini respCom;
	while (1)
	{
		while (!Serial.available()) wdt_reset();
		if (PacketCommand_FromStream(respCom, Serial, 1000) != ProtocolError_None)
			continue;
		if (respCom.PacketID() != GetValue)
			continue;
		break;
	}
	return respCom.PayLoad()[0];
}
float GetFloatValueF(String message)
{
	return GetFloatValue(message);
}
float GetFloatValue(String& message)
{
	while (Serial.available())
		Serial.read();
	int sent = 0;
	if (message.length() == 0)
	{
		PacketCommandMini msgCom;
		msgCom.PacketID(GetValue);
		msgCom.PayLoadLength(1);
		msgCom.PayLoad()[0] = GetFloat;
		msgCom.SendCommand(Serial);
	}
	while (sent < message.length())
	{
		int toSend = min(message.length() - sent, 30);
		PacketCommandMini msgCom;
		msgCom.PacketID(GetValue);
		msgCom.PayLoadLength(toSend + 1);
		msgCom.PayLoad()[0] = (toSend + sent < message.length()) ? GetFloatTBC : GetFloat;

		for (int i = 0; i < toSend; i++)
			msgCom.PayLoad()[i + 1] = message.charAt(sent + i);
		msgCom.SendCommand(Serial);
		sent += toSend;
	}
	PacketCommandMini respCom;
	while (1)
	{
		while (!Serial.available()) wdt_reset();
		if (PacketCommand_FromStream(respCom, Serial, 1000) != ProtocolError_None)
			continue;
		if (respCom.PacketID() != GetValue)
			continue;
		break;
	}
	String num = F("");
	for (int i = 0; i < respCom.PayLoadLength(); i++)
		num += String((char)respCom.PayLoad()[i]);
	return num.toFloat();
}
// Use known conditions on ADC input to determine INL and DNL correction factors
void CalibrateForINL()
{
	ShowMessageFuncF(F("Calibrating for INL\r\n"));
	//// this code samples ADC with 1.1V InternalRef and GND to 
	//// identify INL and DNL in the ADC.

	////Serial.print(F("At 1.1V ADC = "));
	//// Data sheet requires to delay after changing to Band Gap is input
	//// Para 5, Page 217
	//(int16_t)analogRead2(0, 0, RSEADCCalibMux_1_1VRef);
	//delay(1);
	//float v = 0;
	//for (int i = 0; i < 1000; i++)
	//{
	//	v += (int16_t)analogRead2(0, 0, RSEADCCalibMux_1_1VRef);
	//}
	//v /= 1000.0F;
	//ADCINLCorrectionFactor = (1.1F / 2.56F * 1023.0F) / v;
	////Serial.println(F("INL Correction Factor: "));
	////Serial.println(ADCINLCorrectionFactor, 2);

	////Serial.print(F("At 0V ADC = "));
	//v = 0;
	//for (int i = 0; i < 1000; i++)
	//{
	//	v += (int16_t)analogRead2(0, 0, RSEADCCalibMux_GNDRef);
	//}
	//v /= 1000.0F;
	//ADCDNLCorrectionFactor = v;
	////Serial.println(v / 1023.0F * 2.56F, 3);

	// this code samples Differential ADC with shorted inputs for all 4 channels and all gains. 
	// Then takes average value to create a offset addjustment table.
	for (int cID = 0; cID < 4; cID++)
	{
		for (int gID = 0; gID < 3; gID++)
		{
			float v = 0;
			for (int i = 0; i < 1000; i++)
			{
				// lets short the differential pins
				v += (int16_t)analogRead2(cID, gID, DifferentialADCCalibMux);
			}
			v /= 1000.0F;
			float gainF = gID == 0 ? 1 : (gID == 1 ? 10 : 200);
			float voltage = v / 512.0F * 2.56F / gainF;
			INLCorrectionTable[cID][gID] = voltage;
			ShowMessageFuncF(F("Ch = "), true);
			ShowMessageFuncF(String(cID));
			ShowMessageFuncF(F(", Gain = "), true);
			ShowMessageFuncF(String(gID));
			ShowMessageFuncF(F(", "));
			ShowMessageFuncF(F("offset = "), true);;
			ShowMessageFuncF(String(voltage, 3));
			ShowMessageFuncF(F("\r\n"));
			//Serial.print(v);
			//if (gID < 2)
			//	Serial.print(F(", "));
		}
		//Serial.print(F("\n"));
	}
	// this code samples Differential ADC with shorted inputs for all 4 channels and all gains. 
	// Then takes average value to create a offset addjustment table.
	for (int cID = 4; cID < 8; cID++)
	{
		for (int gID = 0; gID < 3; gID++)
		{
			float v = 0;
			for (int i = 0; i < 1000; i++)
			{
				// lets short the differential pins
				v += (int16_t)analogRead2(cID, gID, DifferentialADCCalibMux);
			}
			v /= 1000.0F;
			float gainF = gID == 0 ? 1 : (gID == 1 ? 10 : 200);
			float voltage = v / 512.0F * 2.56F / gainF;
			INLCorrectionTable[cID][gID] = voltage;
			//Serial.print(v);
			//if (gID < 2)
			//	Serial.print(F(", "));
		}
		//Serial.print(F("\n"));
	}
	// the RSE ADC doen't have an offset in most cases.
	for (int cID = 4; cID < 8; cID++)
	{
		for (int gID = 0; gID < 3; gID++)
		{
			INLCorrectionTable[cID][gID] = 0;
		}
	}
	//for (int cID = 4; cID < 8; cID++)
	//{
	//	for (int gID = 0; gID < 3; gID++)
	//	{
	//		ADCDNLCorrectionTable[cID][gID] = ADCDNLCorrectionFactor;
	//	}
	//}
	//for (int cID = 0; cID < 8; cID++)
	//{
	//	for (int gID = 0; gID < 3; gID++)
	//	{
	//		ADCINLCorrectionTable[cID][gID] = ADCINLCorrectionFactor * Atmega2560IntrinsicINLCorrectionFactor[cID][gID];
	//	}
	//}
}


// this function saves the given [N]x[3] sized table on EEPROM.
void SaveToEEPROM(float cTable[][3], int length, int location)
{

	for (int row = 0; row < length; row++)
	{
		for (int column = 0; column < 3; column++)
		{
			fToA valueF;
			valueF.F = cTable[row][column];
			//for (int rByte = 0; rByte < 3; rByte++) // Redundant Bytes
				for (int fByte = 0; fByte < 4; fByte++) // float bytes
					//EEPROM.write(location + row * 3 * 4 * 3 + column * 4 * 3 + rByte * 4 + fByte, valueF.A[fByte]);
					EEPROM.write(location + row * 3 * 4 + column * 4 + fByte, valueF.A[fByte]);
		}
	}
}
void GetFromEEPROM(float cTable[][3], int length, int location)
{
	for (int row = 0; row < length; row++)
	{
		for (int column = 0; column < 3; column++)
		{
			fToA valueF;
			for (int fByte = 0; fByte < 4; fByte++) // float bytes			
			{
				//uint8_t rBytes[3];
				//for (int rByte = 0; rByte < 3; rByte++) // Redundant Bytes	
				//{
					//rBytes[rByte] = EEPROM.read(location + row * 3 * 4 * 3 + column * 4 * 3 + rByte * 4 + fByte);				

				//}
				//valueF.A[fByte] = GetByteFromRedundant(rBytes);
				valueF.A[fByte] = EEPROM.read(location + row * 3 * 4 + column * 4 + fByte);
			}
			cTable[row][column] = valueF.F;
		}
	}
}
uint8_t GetByteFromRedundant(uint8_t* bytes)
{
	if (bytes[0] == bytes[1] || bytes[0] == bytes[2])
		return bytes[0];
	if (bytes[1] == bytes[2])
		return bytes[1];
	return bytes[0];

}
void CalibChannel(
	uint8_t channel, uint8_t gain, 
	uint8_t pwmCalib0, uint8_t pwmCalib1, uint8_t pwmCalib2,
	float vCalib0, float vCalib1, float vCalib2)
{
	uint8_t appliedMax = 0;
	float vApplied = 0;
	if (channel >= 4) // no gain
	{
		appliedMax = pwmCalib0;
		vApplied = vCalib0;
	}
	else
	{
		if (gain == 0)
		{
			appliedMax = pwmCalib0;
			vApplied = vCalib0;
		}
		else if (gain == 1)
		{
			appliedMax = pwmCalib1;
			vApplied = vCalib1;
		}
		else
		{
			appliedMax = pwmCalib2;
			vApplied = vCalib2;
		}
	}
	analogWrite(8, appliedMax);
	delay(1400);

	float y1 = 0;
	if (channel < 4)
		y1 = analogRead3(channel, gain, 200) / 512 * 2.56 * 4.030303;
	else
		y1 = analogRead3(channel, gain, 200) / 1023 * 2.56 * 4.030303;

	analogWrite(8, 0);
	delay(1400);
	float y0 = 0;
	if (channel < 4)
		y0 = analogRead3(channel, gain, 200) / 512 * 2.56 * 4.030303;
	else
		y0 = analogRead3(channel, gain, 200) / 1023 * 2.56 * 4.030303;

	if (channel < 4)
	{
		y0 = y0 / (gain == 0 ? 1.0F : (gain == 1 ? 10.0F : 200.0F));
		y1 = y1 / (gain == 0 ? 1.0F : (gain == 1 ? 10.0F : 200.0F));
	}
	float x1 = vApplied;
	float x0 = 0;
	float m = (x1 - x0) / (y1 - y0);
	float c = x0 - m * y0;

	ShowMessageFuncF(F("Ch = "));
	ShowMessageFuncF(String(channel));
	ShowMessageFuncF(F(", Gain = "));
	ShowMessageFuncF(String(gain));
	ShowMessageFuncF(F(", PWM = "));
	ShowMessageFuncF(String(appliedMax));
	ShowMessageFuncF(F(", V Applied = "));
	ShowMessageFuncF(String(vApplied, 3));
	ShowMessageFuncF(F(", m = "));
	ShowMessageFuncF(String(m, 3));
	ShowMessageFuncF(F(", c = "));
	ShowMessageFuncF(String(c, 3));

	DNLCorrectionTable[channel][gain] = m;
	//INLCorrectionTable[channel][gain] = c;
}
void ScanI2CBusForInstruments()
{
	// Poll on all possible addresses. If the instrument receives proper data on the address, the instrument is intialized.
	for (int i = 1; i < 128; i++)
	{
		if (avaiableInstruments[avaiableInstrumentsCount].begin(i))
		{
			avaiableInstrumentsCount++;
			if (avaiableInstrumentsCount >= MaxInstruments)
				break;

			//Serial.print(F("Found: "));
			//Serial.println(i);
			
		}
		else {
			//Serial.print(F("Not Found: "));
			//Serial.println(i);
		}
	}
}