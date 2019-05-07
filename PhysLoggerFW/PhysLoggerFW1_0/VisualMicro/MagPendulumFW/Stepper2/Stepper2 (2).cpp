#include "Stepper2.h"
#include "..\TimerThree\TimerThree.h"
#include "Arduino.h"



Stepper2::Stepper2()
{
}
int speed = 0;
uint8_t thisStep = 0;
int target = 0;
void tIsr();
void setSpeedRaw(int rpm);
void analogWrite9(uint8_t val);
void setupTimer2();
uint8_t DecSubSample = 0;
uint16_t StepperAngle = 0;
void Stepper2::begin()
{
	pinMode(Stepper_Pin1, 1);
	pinMode(Stepper_Pin2, 1);
	pinMode(Stepper_Pin3, 1);
	pinMode(Stepper_Pin4, 1);

	digitalWrite(Stepper_Pin1, 0);
	digitalWrite(Stepper_Pin2, 0);
	digitalWrite(Stepper_Pin3, 0);
	digitalWrite(Stepper_Pin4, 0);
	Timer3.initialize();
	Timer3.attachInterrupt(tIsr, 1000);
	setupTimer2();
	pinMode(9, 1);
}
int Stepper2::Speed()
{
	return speed;
}
void Stepper2::SetSpeedTarget(int rpm)
{
	target = rpm;
}
void Stepper2::SetSpeed(int rpm)
{
	target = rpm;
	setSpeedRaw(rpm);
}
void setSpeedRaw(int rpm)
{
	if (rpm > 0)
		Timer3.setPeriod(150000 / rpm);
	else
		Timer3.setPeriod(1000000);
	analogWrite9(150 + map(rpm, 30, 300, 0, 85));
	if (rpm == 0)
	{
		analogWrite9(0);
		digitalWrite(Stepper_Pin1, 0);
		digitalWrite(Stepper_Pin2, 0);
		digitalWrite(Stepper_Pin3, 0);
		digitalWrite(Stepper_Pin4, 0);
	}
	speed = rpm;
}
void analogWrite9(uint8_t val)
{
	OCR2B = val;
}
void setupTimer2() {
	// Clear registers
	TCCR2A = 0b10100011;
	TCCR2B = 0b00000001;
}
void Stepper2::SetAngle(uint16_t angle)
{
	StepperAngle = angle;
}
uint16_t Stepper2::GetAngle()
{
	return StepperAngle;
}
int MaxSteps = -1;
void Stepper2::SetMaximumSteps(int steps)
{
	MaxSteps = steps;
}
void Stepper2::Step()
{
	tIsr();
}
void tIsr()
{
	if (speed == 0)
		return;
	StepperAngle = (StepperAngle + 1) % 400;
	if (MaxSteps == 0)
	{
		if (speed > 0)
			setSpeedRaw(0);
		return;
	}
	else if (MaxSteps > 0)
		MaxSteps--;
	if (target > speed)
	{
		if (DecSubSample++ > 5)
		{
			setSpeedRaw(speed + 1);
			DecSubSample = 0;
		}
	}
	else if (target < speed)

		if (DecSubSample++ > 5)
		{
			setSpeedRaw(speed - 1);
			DecSubSample = 0;
		}
	thisStep = (thisStep + 1) % 4;
	switch (thisStep) 
	{
	case 0:  // 1010
		digitalWrite(Stepper_Pin1, HIGH);
		digitalWrite(Stepper_Pin2, LOW);
		digitalWrite(Stepper_Pin3, HIGH);
		digitalWrite(Stepper_Pin4, LOW);
		break;
	case 1:  // 0110
		digitalWrite(Stepper_Pin1, LOW);
		digitalWrite(Stepper_Pin2, HIGH);
		digitalWrite(Stepper_Pin3, HIGH);
		digitalWrite(Stepper_Pin4, LOW);
		break;
	case 2:  //0101
		digitalWrite(Stepper_Pin1, LOW);
		digitalWrite(Stepper_Pin2, HIGH);
		digitalWrite(Stepper_Pin3, LOW);
		digitalWrite(Stepper_Pin4, HIGH);
		break;
	case 3:  //1001
		digitalWrite(Stepper_Pin1, HIGH);
		digitalWrite(Stepper_Pin2, LOW);
		digitalWrite(Stepper_Pin3, LOW);
		digitalWrite(Stepper_Pin4, HIGH);
		break;
	}
}