#include "i2cInstrument.h"

bool i2cInstrument::begin(uint8_t address)
{
	Address = address;
	if (!SetActivePacket(pid_GetInstrumentType))
	{
		buffer[0] = 0;
		InstrumentType = 0;
		Address = 0;
		return 0;
	}
	ReadToBuffer();
	if (buffer[0] > 0 && buffer[0] < 255)
	{
		Address = address;
		InstrumentType = buffer[0];
		SetActivePacket(pid_GetDataType);
		ReadToBuffer();
		DataType = buffer[0];
		SetActivePacket(pid_GetData);
		return 1;
	}
	else
		return 0;
}
union MyUnion
{

};
union fToA {
	float    F;
	uint8_t  A[sizeof(float)];
};

float i2cInstrument::Read()
{
	if (ActivePacket != pid_GetData)
		SetActivePacket(pid_GetData);
	ReadToBuffer();

	if (DataType == dt_float)
	{
		fToA valueF;
		valueF.A[0] = buffer[0];
		valueF.A[1] = buffer[1];
		valueF.A[2] = buffer[2];
		valueF.A[3] = buffer[3];
		return valueF.F;
	}
	else if (DataType == dt_uint8)
	{
		uint8_t value = *buffer;
		return (float)value;
	}
	else if (DataType == dt_int8)
	{
		int8_t value = *buffer;
		return (float)value;
	}
	else if (DataType == dt_uint16)
	{
		return (float)((uint16_t)((uint16_t)buffer[0] + ((uint16_t)((uint16_t)buffer[1] << 8))));
	}
	else if (DataType == dt_int16)
	{
		return (float)((int16_t)((uint16_t)buffer[0] + ((uint16_t)((uint16_t)buffer[1] << 8))));
	}
	else if (DataType == dt_uint32)
	{
		uint32_t value = *buffer;
		return (float)value;
	}
	else if (DataType == dt_int32)
	{
		int32_t value = *buffer;
		return (float)value;
	}
	else if (DataType == dt_double)
	{
		double value = *buffer;
		return (float)value;
	}
}
bool i2cInstrument::SetActivePacket(uint8_t packetID)
{
	Wire.beginTransmission(Address);
	Wire.write(packetID);
	Wire.endTransmission();
	ActivePacket = packetID;
	if (Wire.requestFrom((int)Address, (int)1))
	{
		toReceive = Wire.read();
		delete buffer;
		buffer = new uint8_t[toReceive];
		return 1;
	}
	return 0;
}
void i2cInstrument::ReadToBuffer()
{
	if (toReceive)
		Wire.requestFrom((int)Address, (int)toReceive);
	for (int i = 0; i < toReceive; i++)
		buffer[i] = Wire.read();
}