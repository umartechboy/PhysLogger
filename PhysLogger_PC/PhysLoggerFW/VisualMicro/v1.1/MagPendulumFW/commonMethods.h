#pragma once
#include <Arduino.h>

void BlockCopy(uint8_t* src, uint16_t srcOffset, uint8_t* dest, uint16_t destOffset, uint16_t length);
bool SerialWaitForBytes(HardwareSerial* serial_, uint16_t count = 1, uint16_t timeOut = 5000); 
void PadRight(uint8_t* data, uint16_t offset, uint16_t totalLength);
#define BytesToUInt32(x) (uint32_t)(((uint32_t)(x)[0]) | ((uint32_t)(x)[1] << 8) | ((uint32_t)(x)[2] << 16) | ((uint32_t)(x)[3] << 24))
#define BytesToUInt16(x) (uint32_t)(((uint32_t)(x)[0]) | ((uint32_t)(x)[1] << 8))

