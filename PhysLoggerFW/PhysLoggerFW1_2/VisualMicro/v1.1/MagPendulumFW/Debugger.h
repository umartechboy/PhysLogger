#pragma once
#include "PacketCommands.h"
#include "DebuggerUserSetup.h"	  
#include "DebuggerCommandIDs.h"

#ifdef ESP8266
#define __SPIFFS__
#else
//#define __SD__
#endif 

#ifdef __SPIFFS__
#include <ESP8266WiFi.h>   	 
#define fStr(str) (str)
#define __SPIFFS__
#define FileOpen(path, args) SPIFFS.open(path, args)
#define MediaRemove(path) SPIFFS.remove(path)
#define FOpen_r ("r")
#define FOpen_w ("w")
#define YIELD yield();
#endif
#ifdef __SD__		 	 
#define fStr(str) F(str)
#include <Arduino.h>
#include <SD.h>
#define FileOpen(path, args) SD.open((char*)path.c_str(), args)
#define MediaRemove(path) SD.remove((char*)path.c_str())
#define FOpen_r FILE_READ
#define FOpen_w FILE_WRITE
#define YIELD 
#endif

#define IsDirectory(path) (path.indexOf(F(".")) < 0)
#define IsFile(path) (!IsDirectory(path))


#ifdef __SPIFFS__
#include <ESP8266WiFi.h>
#define __SPIFFS__
#define FileOpen(path, args) SPIFFS.open(path, args)
#define MediaRemove(path) SPIFFS.remove(path)
#define FOpen_r ("r")
#define FOpen_w ("w")
#define YIELD yield();
#endif
#ifdef __SD__
#include <Arduino.h>
#include <SD.h>
#define FileOpen(path, args) SD.open((char*)path.c_str(), args)
#define MediaRemove(path) SD.remove((char*)path.c_str())
#define FOpen_r FILE_READ
#define FOpen_w FILE_WRITE
#define YIELD 
#endif

#define IsDirectory(path) (path.indexOf(F(".")) < 0)
#define IsFile(path) (!IsDirectory(path))


class DebuggerClass :public Stream
{
public:
	DebuggerClass();
	void begin(Stream* defaultStream);
	DebuggerClass(Stream* writeStream);
	Stream* writeStream;
	size_t write(const uint8_t *buffer, size_t size) override;
	size_t write(uint8_t data) override;

	int available() override;
	int read() override;
	int peek() override;
	void flush() override;
	String ReadString(int timeout = 1000);
};
// a call to this will parse commands on the passed streams.
void DebuggerCheckForEvents(Stream* inStream, Stream* outStream = 0, int sender = -1);

// implemented in the project code
void DebuggerServiceCommand(String command, Stream& stream);

// implemented in the project code
void DebuggerServiceCommand(PacketCommand& command, Stream& stream);

// implemented by the debugger
bool DebuggerServiceFSECommand(PacketCommand& command, Stream& stream);

extern DebuggerClass PCDebugger;