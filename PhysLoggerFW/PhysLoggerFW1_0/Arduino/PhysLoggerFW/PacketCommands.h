#pragma once
#include <Arduino.h>
#include "commonMethods.h"
#include "Enums.h"
#define DefaultUARTNodeID 0
#define Common_BlockCopy BlockCopy
#define FixedPayloadLength 13
#define MaximumPayloadLength 31

class PacketCommandMini
{

protected:
	uint8_t data_ [MaximumPayloadLength + 1];

public:
	uint8_t comData_[2];
	uint8_t PacketID();
	void PacketID(uint8_t value);
public:
	PacketCommandMini();
	PacketCommandMini(uint8_t commandID, uint8_t* Payload, uint8_t length);
	uint8_t ActualCheckSum();
	uint8_t TrueCheckSum();
	void SendCommand(Stream& serial_);
	uint8_t* PayLoad();
	void PayLoad(uint8_t* payload, uint8_t length);
	uint8_t PayLoadLength();
	void PayLoadLength(uint8_t);
};

uint8_t PacketCommand_FromStream(PacketCommandMini& command, Stream &stream, uint16_t timeOut = 3000, bool useExistingBuffer = false);
