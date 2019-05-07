#pragma once
class PID
{
public:
	// Kp -  proportional gain
	// Ki -  Integral gain
	// Kd -  derivative gain
	// dt -  loop interval time
	// max - maximum value of manipulated variable
	// min - minimum value of manipulated variable
	PID(float dt, float max, float min, float Kp, float Kd, float Ki);

	float _dt;
	float _max;
	float _min;
	float _Kp;
	float _Kd;
	float _Ki;
	float _pre_error;
	float _integral;

	// Returns the manipulated variable given a setpoint and current process value
	float calculate(float setpoint, float pv);

private:
};
