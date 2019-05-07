#pragma once
#include "PID.h"


PID::PID(float dt, float max, float min, float Kp, float Kd, float Ki)
{
	_dt = dt;
	_max = max;
	_min = min;
	_Kp = Kp;
	_Ki = Ki;
	_Kd = Kd;
}

float PID::calculate(float setpoint, float pv)
{

	// Calculate error
	float error = setpoint - pv;

	// Proportional term
	float Pout = _Kp * error;

	// Integral term
	_integral += error * _dt;
	float Iout = _Ki * _integral;

	// Derivative term
	float derivative = (error - _pre_error) / _dt;
	float Dout = _Kd * derivative;

	// Calculate total output
	float output = Pout + Iout + Dout;

	// Restrict to max/min
	if (output > _max)
		output = _max;
	else if (output < _min)
		output = _min;

	// Save error to previous error
	_pre_error = error;

	return output;
}
