#include "commonMethods.h"
#include <Arduino.h>

extern unsigned int __heap_start;
extern void *__brkval;


void BlockCopy(uint8_t* src, uint16_t srcOffset, uint8_t* dest, uint16_t destOffset, uint16_t length)
{
	for (int i = 0; i < length; i++)
	{
		dest[i + destOffset] = src[i + srcOffset];
	}
}
bool SerialWaitForBytes(HardwareSerial* serial_, uint16_t count, uint16_t timeOut)
{
	uint16_t tOut = 0;
	while (serial_->available() < count && tOut < 3000)
	{
		tOut++;
		delay(1);
	}
	if (serial_->available() >= count) return true;
	else
	{
		return false;
	}
}


void PadRight(uint8_t* data, uint16_t offset, uint16_t totalLength)
{
	for (int i = offset; i < totalLength; i++)
	{
		data[i] = 0;
	}
}