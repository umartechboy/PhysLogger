// Title:				PhysLogger 1.0 Firmware
// Supported Devices:	Atmega2560 and similar. (4 Differential ADC Channels with gains 1, 10x , 200x required)
// Date:				14 Feb 2018 
// Author:				M U Hassan
// License:				GPL

// include the PacketCommandsMini library by FivePointNine, it is optimized for speed and stability
#include "PacketCommands.h"
// Custom AnalogRead Function for faster reading
uint16_t analogRead2(uint8_t pin, uint8_t gain);
void sendSignature();

// Packet Command IDs
#define GetHWSignatures 1
#define ChangeChannelType 2
#define ChangeChannelGain 3
#define DataPacket 4
#define BeginFire 5
#define ChangeSamplingTime 6
#define LoggerVersion String(F("PhysLogger1_0"))
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
uint8_t typeSelector[4];
// sampling period in millis
uint32_t samplingPeriod = 1;
uint32_t blinkTime = 0;
float a_averaged[4] = { 0,0,0,0 };
uint16_t absa[4];
int16_t a_In_511To511Format[4];
float a_int16AsFloat[4];

void setup() {
	Serial.begin(115200);
	datacom.PayLoadLength(13);
	datacom.PayLoad()[8] = 0;
	datacom.PacketID(DataPacket);
	sendSignature();
	for (int i = 0; i < 4; i++)
		gainSelector[i] = 0; // gain 1
	for (int i = 0; i < 4; i++)
		typeSelector[i] = i;// Differential
	pinMode(LED_BUILTIN, 1);
}
void sendSignature()
{
	String str = String(F("ver=")) + LoggerVersion;
	PacketCommandMini(GetHWSignatures, (uint8_t*)str.c_str(), str.length()).SendCommand(Serial);
	uint8_t rate[4];
	rate[0] = (samplingPeriod) >> 0;
	rate[1] = (samplingPeriod) >> 8;
	rate[2] = (samplingPeriod) >> 16;
	rate[3] = (samplingPeriod) >> 24;
	PacketCommandMini(ChangeSamplingTime, rate, 4).SendCommand(Serial);
}

void loop()
{
	if (Serial.available())
	{
		if (PacketCommand_FromStream(comDO, Serial) == ProtocolError_None)
		{
			if (comDO.PacketID() == GetHWSignatures) // Set Channel  gains
			{
				sendSignature();
			}
			else if (comDO.PacketID() == BeginFire) // Set Fire
			{
				if (comDO.PayLoad()[0] == 1)
					canFire = true;
				else
					canFire = 0;
			}
			else if (comDO.PacketID() == ChangeChannelGain) // Set Channel  gains
			{
				gainSelector[comDO.PayLoad()[0]] = comDO.PayLoad()[1];
				datacom.PayLoad()[8] =
					(gainSelector[0] << 0) |
					(gainSelector[1] << 2) |
					(gainSelector[2] << 4) |
					(gainSelector[3] << 6)
					;
			}
			else if (comDO.PacketID() == ChangeChannelType) // Set Channel  type
			{
				typeSelector[comDO.PayLoad()[0]] = (comDO.PayLoad()[1] == 0 ? 0 : 4) + comDO.PayLoad()[0];
				for (int i = 0; i < 4; i++)
					gainSelector[i] = 0;
				datacom.PayLoad()[8] =
					(gainSelector[0] << 0) |
					(gainSelector[1] << 2) |
					(gainSelector[2] << 4) |
					(gainSelector[3] << 6)
					;
			}
			else if (comDO.PacketID() == ChangeSamplingTime) // set sampling rate
			{
				samplingPeriod = 
					((uint32_t)(comDO.PayLoad()[0] + (comDO.PayLoad()[1] << 8) + (comDO.PayLoad()[1] << 16) + (comDO.PayLoad()[3] << 24)))
					;

				uint8_t rate[4];
				rate[0] = (samplingPeriod) >> 0;
				rate[1] = (samplingPeriod) >> 8;
				rate[2] = (samplingPeriod) >> 16;
				rate[3] = (samplingPeriod) >> 24;
				PacketCommandMini(ChangeSamplingTime, rate, 4).SendCommand(Serial);
			}
		}
	}
	digitalWrite(LED_BUILTIN,
		((millis() - blinkTime) > 0 && (millis() - blinkTime) < 100) |
		((millis() - blinkTime) > 200 && (millis() - blinkTime) < 300));
	if (canFire)
	{
	}
	else if ((millis() - blinkTime) > 3000)
		blinkTime = millis();
	float fac = min(10 / ((float)samplingPeriod), 1);
	for (uint8_t cid = 0; cid < 4; cid++)
	{
		a_int16AsFloat[cid] = (int16_t)analogRead2(typeSelector[cid], gainSelector[cid]);
		if (typeSelector[cid] < 4)
			a_int16AsFloat[cid] *= 4.0F / 3.0F;
		else
			a_int16AsFloat[cid] *= 7.0F / 8.0F;
		a_averaged[cid] = a_int16AsFloat[cid] * fac + a_averaged[cid] * (1 - fac);
	}
	if (samplingPeriod > 10)
	{
		delay(1);
	}
	if (millis() - TimeSinceLastSample < samplingPeriod)
		return;
	TimeSinceLastSample = millis();
	datacom.PayLoad()[7] = 0;
	for (uint8_t cid = 0; cid < 4; cid++)
	{
		a_In_511To511Format[cid] = min(max(a_averaged[cid], -511), 511);
		absa[cid] = ((uint16_t)abs((int16_t)a_In_511To511Format[cid]));
		datacom.PayLoad()[3 + cid] = absa[cid] & 0xFF;
		datacom.PayLoad()[7] |=
			(((absa[cid] >> 8) & 0b1) << (2 * cid)) |
			(((((int16_t)a_In_511To511Format[cid]) < 0) ? 1 : 0) << (1 + cid * 2));
	}
	lastMillis = TimeSinceLastSample;
	datacom.PayLoad()[0] = lastMillis >> 0;
	datacom.PayLoad()[1] = lastMillis >> 8;
	datacom.PayLoad()[2] = lastMillis >> 16;
	datacom.SendCommand(Serial);
}
uint8_t MUXPolarity[8][3] =
{
	{ 0, 1, 1 },
	{ 1, 1, 1 },
	{ 0, 1, 1 },
	{ 1, 1, 1 },

	{ 1, 1, 1 },
	{ 1, 1, 1 },
	{ 1, 1, 1 },
	{ 1, 1, 1 }
};
uint8_t MUX[8][3] =
{
	{ 0b010000, 0b001001, 0b001011 },
	{ 0b011011, 0b001101, 0b001111 },
	{ 0b110000, 0b101001, 0b101011 },
	{ 0b111011, 0b101101, 0b101111 },

	{ 0b000000, 0b000000, 0b000000 },
	{ 0b000010, 0b000010, 0b000010 },
	{ 0b100000, 0b100000, 0b100000 },
	{ 0b100010, 0b100010, 0b100010 }
};
uint8_t MUXB[4] =
{
	0b010000, 0b011011, 0b110000,0b111011
};
#define sbi(sfr, bit) (_SFR_BYTE(sfr) |= _BV(bit))
#define cli(sfr, bit) (_SFR_BYTE(sfr) &= ~_BV(bit))
uint16_t analogRead2(uint8_t channel, uint8_t gain)
{
	if ((MUX[channel][gain] >> 5) % 2)
		ADCSRB |= 1 << MUX5;
	else
		ADCSRB &= ~(1 << MUX5);
	ADMUX =
		(1 << REFS1) | (1 << REFS0) | //  1.1V reference
									  //(0 << ADLAR) | //           left adjust result
		((MUX[channel][gain]) & 0b11111)
		;

	// Start Conversion
	sbi(ADCSRA, ADEN);
	sbi(ADCSRA, ADSC);
	while ((ADCSRA >> ADSC) & 0b1);
	uint8_t adcl = ADCL;
	uint8_t adch = ADCH;
	ADCSRA &= ~(1 << ADEN);
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