#pragma once
#include <Arduino.h>

#define Stepper_Pin1 4
#define Stepper_Pin2 5
#define Stepper_Pin3 7
#define Stepper_Pin4 6

class Stepper2
{
public :
	Stepper2();
	void begin();
	void SetSpeedTarget(int rpm);
	void SetSpeed(int rpm);
	void SetMaximumSteps(int steps = -1);
	int Speed();
	void SetAngle(uint16_t angle);
	void Step();
	uint16_t GetAngle();
private:
};