/**************************************************************************/
/*!
    @file     Adafruit_ADXL345.cpp
    @author   K.Townsend (Adafruit Industries)
    @license  BSD (see license.txt)

    The ADXL345 is a digital accelerometer with 13-bit resolution, capable
    of measuring up to +/-16g.  This driver communicate using I2C.

    This is a library for the Adafruit ADXL345 breakout
    ----> https://www.adafruit.com/products/1231

    Adafruit invests time and resources providing this open source code,
    please support Adafruit and open-source hardware by purchasing
    products from Adafruit!

    @section  HISTORY
    
    v1.1 - Added Adafruit_Sensor library support
    v1.0 - First release
*/
/**************************************************************************/
#if ARDUINO >= 100
 #include "Arduino.h"
#else
 #include "WProgram.h"
#endif

#include "..\SoftWire\SoftwareI2C.h"
#include <limits.h>

#include "Adafruit_ADXL345_U_SI2C.h"

Adafruit_ADXL345_Unified_SI2C::Adafruit_ADXL345_Unified_SI2C(int32_t sensorID = -1):Adafruit_ADXL345_Unified(sensorID)
{}
Adafruit_ADXL345_Unified_SI2C::Adafruit_ADXL345_Unified_SI2C(uint8_t clock, uint8_t miso, uint8_t mosi, uint8_t cs, int32_t sensorID = -1) : Adafruit_ADXL345_Unified(clock, miso, mosi, cs, sensorID)
{}

/**************************************************************************/
/*!
    @brief  Abstract away platform differences in Arduino Wire2 library
*/
/**************************************************************************/
inline uint8_t Adafruit_ADXL345_Unified_SI2C::i2cread(void) {
  #if ARDUINO >= 100
  return Wire2.read();
  #else
  return Wire2.receive();
  #endif
}

/**************************************************************************/
/*!
    @brief  Abstract away platform differences in Arduino Wire2 library
*/
/**************************************************************************/
inline void Adafruit_ADXL345_Unified_SI2C::i2cwrite(uint8_t x) {
  #if ARDUINO >= 100
  Wire2.write((uint8_t)x);
  #else
  Wire2.send(x);
  #endif
}

/**************************************************************************/
/*!
    @brief  Writes 8-bits to the specified destination register
*/
/**************************************************************************/

static uint8_t spixfer(uint8_t clock, uint8_t miso, uint8_t mosi, uint8_t data);
void Adafruit_ADXL345_Unified_SI2C::writeRegister(uint8_t reg, uint8_t value) {
  if (_i2c) {
    Wire2.beginTransmission((uint8_t)_i2caddr);
    i2cwrite((uint8_t)reg);
    i2cwrite((uint8_t)(value));
    Wire2.endTransmission();
  } else {
    digitalWrite(_cs, LOW);
    spixfer(_clk, _di, _do, reg);
    spixfer(_clk, _di, _do, value);
    digitalWrite(_cs, HIGH);
  }
}

/**************************************************************************/
/*!
    @brief  Reads 8-bits from the specified register
*/
/**************************************************************************/
uint8_t Adafruit_ADXL345_Unified_SI2C::readRegister(uint8_t reg) {
  if (_i2c) {
    Wire2.beginTransmission((uint8_t)_i2caddr);
    i2cwrite(reg);
    Wire2.endTransmission();
    Wire2.requestFrom((uint8_t)_i2caddr, 1);
    return (i2cread());
  } else {
    reg |= 0x80; // read byte
    digitalWrite(_cs, LOW);
    spixfer(_clk, _di, _do, reg);
    uint8_t reply = spixfer(_clk, _di, _do, 0xFF);
    digitalWrite(_cs, HIGH);
    return reply;
  }  
}

/**************************************************************************/
/*!
    @brief  Reads 16-bits from the specified register
*/
/**************************************************************************/
int16_t Adafruit_ADXL345_Unified_SI2C::read16(uint8_t reg) {
  if (_i2c) {
    Wire2.beginTransmission((uint8_t)_i2caddr);
    i2cwrite(reg);
    Wire2.endTransmission();
    Wire2.requestFrom((uint8_t)_i2caddr, 2);
    return (uint16_t)(i2cread() | (i2cread() << 8));  
  } else {
    reg |= 0x80 | 0x40; // read byte | multibyte
    digitalWrite(_cs, LOW);
    spixfer(_clk, _di, _do, reg);
    uint16_t reply = spixfer(_clk, _di, _do, 0xFF)  | (spixfer(_clk, _di, _do, 0xFF) << 8);
    digitalWrite(_cs, HIGH);
    return reply;
  }    
}

/**************************************************************************/
/*!
    @brief  Setups the HW (reads coefficients values, etc.)
*/
/**************************************************************************/
bool Adafruit_ADXL345_Unified_SI2C::begin(int SDA_, int SCL_, uint8_t i2caddr) {
  _i2caddr = i2caddr;
  
  if (_i2c)
	  Wire2.begin(SDA_, SCL_);
  else {
    pinMode(_cs, OUTPUT);
    digitalWrite(_cs, HIGH);
    pinMode(_clk, OUTPUT);
    digitalWrite(_clk, HIGH);
    pinMode(_do, OUTPUT);
    pinMode(_di, INPUT);
  }

  /* Check connection */
  uint8_t deviceid = getDeviceID();
  if (deviceid != 0xE5)
  {
    /* No ADXL345 detected ... return false */
    return false;
  }
  
  // Enable measurements
  writeRegister(ADXL345_REG_POWER_CTL, 0x08);  
    
  return true;
}
