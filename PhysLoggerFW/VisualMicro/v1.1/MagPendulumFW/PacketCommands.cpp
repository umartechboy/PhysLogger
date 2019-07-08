#include "PacketCommands.h"

uint8_t PacketCommand_FromStream(PacketCommandMini& command, Stream &stream, uint16_t timeOut, bool useExistingBuffer)
{
	unsigned long start = millis();

	bool hasAA = false;
	while (!hasAA && (millis() - start) < timeOut)
	{
		if (stream.available() < 3)
			continue;
		if (stream.read() == 0xAA)
		{
			hasAA = true;
			break;
		}
	}
	if (!hasAA)
		return ProtocolError_ReadTimeout;
	stream.readBytes(command.comData_, 2);
	uint8_t dlen = command.PayLoadLength();
	int i = 0;
	while (i < dlen && (millis() - start) < timeOut)
	{
		if (stream.available())
		{
			command.PayLoad()[i++] = stream.read();
		}
	}
	if (i < dlen) // not all of the bytes were received
	{
		return ProtocolError_ReadTimeout;
	}
	// calculate checksum
	if (command.TrueCheckSum() != command.ActualCheckSum())
	{
		return ProtocolError_CheckSumMismatch;
	}

	// we got the command. letes return
	return ProtocolError_None;
}

void PacketCommandMini::SendCommand(Stream& serial_)
{
	serial_.write(0xAA);
	uint8_t sum = 0x55 ^ comData_[0];
	for (int i = 0; i < PayLoadLength(); i++)
		sum ^= data_[i];
	comData_[1] = sum;
	serial_.write(comData_[0]);
	serial_.write(comData_[1]);
	if (PayLoadLength())
		serial_.write(data_, PayLoadLength());
}
PacketCommandMini::PacketCommandMini()
{
	PayLoadLength(FixedPayloadLength);
}
PacketCommandMini::PacketCommandMini(uint8_t commandID, uint8_t* payload, uint8_t length)
{
	PacketID(commandID);
	PayLoad(payload, length);
}
uint8_t PacketCommandMini::TrueCheckSum()
{
	return comData_[1];
}
void PacketCommandMini::PayLoadLength(uint8_t len)
{
	comData_[0] &= 0b00000111;
	comData_[0] |= len << 3;
}
uint8_t PacketCommandMini::PayLoadLength()
{
	return comData_[0] >> 3;
}
void PacketCommandMini::PacketID(uint8_t packetID)
{
	comData_[0] &= 0b11111000;
	comData_[0] |= (packetID & 0b00000111);
}
uint8_t PacketCommandMini::PacketID()
{
	return (comData_[0] & 0b00000111);
}
uint8_t PacketCommandMini::ActualCheckSum()
{
	uint8_t sum = 0x55 ^ comData_[0];
	for (int i = 0; i < PayLoadLength(); i++)
		sum ^= data_[i];
	return sum;
}
uint8_t* PacketCommandMini::PayLoad()
{
	return data_;
}
void PacketCommandMini::PayLoad(uint8_t* payload, uint8_t length)
{
	PayLoadLength(length);
	if (length == 0)
		return;
	Common_BlockCopy(payload, 0, PayLoad(), 0, length);
}