#include "Adafruit_ADXL345_U.h"
#include "..\SoftWire\SoftwareI2C.h"
class Adafruit_ADXL345_Unified_SI2C : public Adafruit_ADXL345_Unified {
public:
	Adafruit_ADXL345_Unified_SI2C(int32_t sensorID = -1);
	Adafruit_ADXL345_Unified_SI2C(uint8_t clock, uint8_t miso, uint8_t mosi, uint8_t cs, int32_t sensorID = -1);

	inline uint8_t  i2cread(void) override;
	inline void     i2cwrite(uint8_t x)  override;
	bool       begin(int SDA_, int SCL_, uint8_t addr = ADXL345_DEFAULT_ADDRESS);
	void       writeRegister(uint8_t reg, uint8_t value) override;
	uint8_t    readRegister(uint8_t reg) override;
	int16_t    read16(uint8_t reg) override;
private:
	SoftwareI2C Wire2;

};
