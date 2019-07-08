#pragma once
#include <Arduino.h>
#include <Wire.h>

#define pid_GetInstrumentType 1
#define pid_GetDataType 2
#define pid_GetData 3

#define PhysLabDigitalCaliper1_0 1
#define PhysLabLoadCell2_0 2

// data types 
#define dt_uint8 1
#define dt_int8 2
#define dt_uint16 3
#define dt_int16 4
#define dt_uint32 5
#define dt_int32 6
#define dt_float 7
#define dt_double 8
//data type lengths
#define dtLength_uint8 1
#define dtLength_int8 1
#define dtLength_uint16 2
#define dtLength_int16 2
#define dtLength_uint32 4
#define dtLength_int32 4
#define dtLength_float 4
#define dtLength_double 8

class i2cInstrument
{
public:
	uint8_t Address = 0;
	uint8_t DataType = 0;
	uint8_t InstrumentType = 0;
	uint8_t ActivePacket = 0;
	bool begin(uint8_t address);
	bool SetActivePacket(uint8_t packetID);
	void ReadToBuffer();
	float Read();
	uint8_t * buffer = new uint8_t[1];
	uint8_t toReceive = 0;
};