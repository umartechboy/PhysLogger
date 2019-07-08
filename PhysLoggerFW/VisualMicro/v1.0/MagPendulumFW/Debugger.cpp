#include "Debugger.h"

#ifdef __SD__

void removeRecursively(String path)
{
	if (IsDirectory(path))
	{
		File dir = SD.open((char*)path.c_str());
		File sub = dir.openNextFile();
		while (sub)
		{
			String name = String(sub.name());
			if (IsDirectory(name))
				removeRecursively(name);
			else
			{
				sub.close();
				SD.remove((char*)name.c_str());
			}
			sub = dir.openNextFile();
		}
	}
	else
	{
		if (SD.exists((char*)path.c_str()))
			MediaRemove(path);
	}
}
#endif
void DebuggerCheckForEvents(Stream* inStream, Stream* outStream, int sender)
{
	if (!(inStream->available())) return;
	PacketCommand command;
	if (PacketCommand_FromStream(command, (*inStream), 100) != ProtocolError_None)
	{
		DebuggerCheckForEvents(inStream, outStream);
		return;
	}

	command.PayLoad()[command.PayLoadLength()] = 0;
	if (command.TargetID() == ThisNodeIndex) // this node
	{
		if (command.PacketID() == UART_Debug || command.PacketID() == UART_Debugln)
		{
			DebuggerServiceCommand(String(command.PayLoadString()), (*inStream));
		}
		else if (!DebuggerServiceFSECommand(command, (*inStream)))
		{
			DebuggerServiceCommand(command, (*inStream));
		}
	}
	else
	{
		if (outStream)
		{
			command.SendCommand((*outStream));
		}
	}
}
bool DebuggerServiceFSECommand(PacketCommand& command, Stream& stream)
{

	if (command.PacketID() == UART_FSE_GetContents)
	{
		String path = String((char*)command.PayLoad());
		if (path == fStr("/"))
			path = fStr("");

		bool exists = false;
#ifdef __SPIFFS__

		Dir dir = SPIFFS.openDir("");
		bool hasBegun = false;
		while (dir.next())
		{
			String name = dir.fileName();
			if (name.startsWith(path) || path == "")
			{
				exists = true;
				if (path != "")
				{
					//trim path till this dir
					name = name.substring(name.indexOf(path) + path.length() + 1);
				}
				if (name.indexOf("/") >= 0) // has more dirs
				{
					name = name.substring(0, name.indexOf("/"));
				}
				name = path + ":" + name;
				PacketCommand(
					(!hasBegun) ? UART_FSE_Resp_GetContents : UART_FSE_Resp_GetContents_EXT,
					(uint8_t*)name.c_str(),
					name.length(),
					ThisNodeIndex,
					command.SourceID()
				).SendCommand(stream);
				hasBegun = true;
			}
		}
#endif
#ifdef __SD__
		exists = SD.exists((char*)path.c_str());
		if (IsFile(path)) // is a file
			exists = false;
		if (path == F("") || path == F("/"))
			exists = true;
		if (exists)
		{
			File main = FileOpen(path, FOpen_r);
			bool hasBegun = false;
			while (true)
			{
				File sub = main.openNextFile();
				if (sub)
				{
					String name = path + F(":") + String(sub.name());
					PacketCommand(
						(!hasBegun) ? UART_FSE_Resp_GetContents : UART_FSE_Resp_GetContents_EXT,
						(uint8_t*)name.c_str(),
						name.length(),
						ThisNodeIndex,
						command.SourceID()).SendCommand(stream);
					hasBegun = true;
				}
				if (!sub)
					break;
				sub.close();
			}
			main.close();
		}
#endif // __SD__

		if (!exists)
		{
			PacketCommand(
				UART_FSE_Resp_DoesntExist,
				command.PayLoad(),
				command.PayLoadLength(),
				ThisNodeIndex,
				command.SourceID()).SendCommand(stream);
			true;
		}
	}
	else if (command.PacketID() == UART_FSE_CreateFolder)
	{
#ifdef __SPIFFS__
		PacketCommand(
			UART_FSE_Resp_Failed,
			String(fStr("This file system doesn't support directories. Use / in the file names")),
			ThisNodeIndex,
			command.SourceID()).SendCommand(stream);
#endif
#ifdef __SD__
		String path = String((char*)command.PayLoad());
		if (IsFile(path)) // is a filename
		{
			PacketCommand(
				UART_FSE_Resp_Failed,
				String(fStr("A directory name must not contain \".\"")),
				ThisNodeIndex,
				command.SourceID()).SendCommand(stream);
			return true;
		}
		if (SD.exists((char*)path.c_str()))
		{
			PacketCommand(
				UART_FSE_Resp_Failed,
				String(fStr("A directory with this name already exists.")),
				ThisNodeIndex,
				command.SourceID()).SendCommand(stream);
			return true;
		}
		SD.mkdir((char*)path.c_str());
#endif
	}
	else if (command.PacketID() == UART_FSE_Remove)
	{
		String path = String((char*)command.PayLoad());
		if (IsFile(path)) // is a file
		{
			File f = FileOpen(path, FOpen_r);
			if (!f)
				PacketCommand(
					UART_FSE_Resp_DoesntExist,
					command.PayLoad(),
					command.PayLoadLength(),
					ThisNodeIndex,
					command.SourceID()).SendCommand(stream);
			else
			{
				f.close();
				MediaRemove(path);
			}

		}
		else
		{
			if (path == fStr("/"))
				path = fStr("");
			String files = "";
			bool exists = false;

#ifdef __SPIFFS__
			Dir dir = SPIFFS.openDir("");
			while (dir.next())
			{
				String name = dir.fileName();
				if (name.startsWith(path))
				{
					exists = true;
					SPIFFS.remove(name);
				}
			}
#endif
#ifdef __SD__
			if (SD.exists((char*)path.c_str()))
			{
				removeRecursively(path);
			}
#endif
			if (!exists)
			{
				PacketCommand(
					UART_FSE_Resp_DoesntExist, command.PayLoad(),
					command.PayLoadLength(),
					ThisNodeIndex,
					command.SourceID()).SendCommand(stream);
				return true;
			}
		}
	}
	else if (command.PacketID() == UART_FSE_CreateFile)
	{
		String path = String((char*)command.PayLoad());
		if (IsDirectory(path)) // not a valid file name;
		{
			PacketCommand(
				UART_FSE_Resp_CreateFile_Failure,
				String(fStr("The file name must contain an extension.")),
				ThisNodeIndex,
				command.SourceID()).SendCommand(stream);
			return true;
		}
		File f = FileOpen(path, FOpen_w);
#ifdef __SD__
		if (f)
		{
			f.close();
			MediaRemove(path);
			f = FileOpen(path, FOpen_w);
		}
#endif 
		if (!f)
		{
			PacketCommand(
				UART_FSE_Resp_CreateFile_Failure,
				String(fStr("File creation failed on SPIFFS.")),
				ThisNodeIndex,
				command.SourceID()).SendCommand(stream);
			return true;
		}
		PacketCommand(
			UART_FSE_Resp_CreateFile_ACK,
			(uint8_t)ThisNodeIndex,
			command.SourceID()
		).SendCommand(stream);
		PacketCommand action;
		if (PacketCommand_FromStream(action, stream, 2000) != ProtocolError_None)
		{
			PacketCommand(
				UART_FSE_Resp_Failed,
				String(fStr("File header not recieved.")),
				ThisNodeIndex,
				command.SourceID()).SendCommand(stream);
			f.close();
			MediaRemove(path);
			return true;
		}
		if (action.PacketID() != UART_FSE_Resp_GetFile_Header)
		{
			PacketCommand(
				UART_FSE_Resp_Failed,
				String(fStr("Debugger out of sync. (ESP)")),
				ThisNodeIndex,
				command.SourceID()).SendCommand(stream);
			f.close();
			MediaRemove(path);
			return true;
		}

		uint32_t size = 0;
		uint16_t bytesPerPacket = 0;
		for (int i = 0; i < 4; i++)
		{
			size += action.PayLoad()[i] << (8 * i);
		}for (int i = 0; i < 2; i++)
		{
			bytesPerPacket += action.PayLoad()[i + 4] << (8 * i);
		}
		long start = millis();

		uint16_t totalPackets = (uint16_t)(((long)size - 1) / bytesPerPacket) + 1;
		if (size == 0) totalPackets = 0;

		PacketCommand data;
		PacketCommand recACKCom =
			PacketCommand(
			(uint8_t)UART_FSE_Resp_CreateFile_ACK,
				(uint8_t)ThisNodeIndex,
				command.SourceID());
		recACKCom.PayLoadLength(2);
		data.PayLoadLength(bytesPerPacket);


		while (totalPackets)
		{
			YIELD;
			if (PacketCommand_FromStream(data, stream, 5000) != ProtocolError_None)
			{
				PacketCommand(
					UART_FSE_Resp_Failed,
					String(fStr("File transfer failed (ESP)")),
					ThisNodeIndex,
					command.SourceID()).SendCommand(stream);
				f.close();
				MediaRemove(path);
				return true;
			}
			YIELD;
			f.write(data.PayLoad(), data.PayLoadLength());
			YIELD;

			totalPackets--;
			recACKCom.PayLoad()[0] = totalPackets;
			recACKCom.PayLoad()[1] = totalPackets >> 8;
			recACKCom.SendCommand(stream);
		}
	}
	else if (command.PacketID() == UART_FSE_GetFile)
	{
		String path = String((char*)command.PayLoad());
		File f = FileOpen(path, FOpen_r);
		if (!f)
		{
			PacketCommand(
				UART_FSE_Resp_DoesntExist,
				command.PayLoad(),
				command.PayLoadLength(),
				ThisNodeIndex,
				command.SourceID()).SendCommand(stream);
			return true;
		}
		PacketCommand(
			UART_FSE_Resp_GetFile,
			command.PayLoad(),
			command.PayLoadLength(),
			ThisNodeIndex,
			command.SourceID()).SendCommand(stream);

		long sz = f.size();

		PacketCommand header = PacketCommand(
			UART_FSE_Resp_GetFile_Header,
			(uint8_t)ThisNodeIndex,
			command.SourceID());
		header.PayLoadLength(4);
		for (int i = 0; i < 4; i++)
			header.PayLoad()[i] = sz >> (8 * i);
		header.SendCommand(stream);
		PacketCommand buffer = PacketCommand(
			UART_FSE_Resp_GetFile_DataPart,
			(uint8_t)ThisNodeIndex,
			command.SourceID());
		buffer.PayLoadLength(64);
		uint32_t csum = 0;
		while (sz)
		{
			if (sz < 64) // last
				buffer.PayLoadLength(sz);
			int tread = f.readBytes((char*)buffer.PayLoad(), min(sz, 64));
			sz -= tread;
			// wait for the request first
			PacketCommand q;
			bool closeUp = false;
			if (PacketCommand_FromStream(q, stream, 500) != ProtocolError_None)
				closeUp = true;
			else if (q.PacketID() != UART_FSE_GetFile_NextPart)
				closeUp = true;
			if (closeUp)
			{
				f.close();
				return true;
			}

			buffer.SendCommand(stream);
			YIELD;
			for (int i = 0; i < tread; i++)
				csum += buffer.PayLoad()[i];
		}
		f.close();
		PacketCommand footer = PacketCommand(
			UART_FSE_Resp_GetFile_Footer,
			(uint8_t)ThisNodeIndex,
			command.SourceID());
		footer.PayLoadLength(4);
		for (int i = 0; i < 4; i++)
			footer.PayLoad()[i] = csum >> (8 * i);
		footer.SendCommand(stream);
	}

	else
	{
		return false;
	}
	return true;
}
DebuggerClass::DebuggerClass()
{}
DebuggerClass::DebuggerClass(Stream* writeStream)
{
	this->writeStream = writeStream;
}
void DebuggerClass::begin(Stream* writeStream)
{
	this->writeStream = writeStream;
}
size_t DebuggerClass::write(const uint8_t *buffer, size_t size)
{
	return 	writeStream->write(buffer, size);
	//PacketCommand com;
	//com.PacketID(UART_Debug);
	//com.PayLoad((uint8_t*)buffer, size);
	//com.SourceID(ThisNodeIndex);
	//com.TargetID(DebugServerNodeIndex);
	//com.SendCommand((*writeStream));
	//return size;
}
size_t DebuggerClass::write(uint8_t data)
{

	return write(&data, 1);
}

int DebuggerClass::available()
{
	return 0;
}
int DebuggerClass::read()
{
	return 0;
}
int DebuggerClass::peek()
{
	return 0;
}
void DebuggerClass::flush()
{
}

String DebuggerClass::ReadString(int timeout)
{
	long st = millis();
	while (true)
	{
		int _timeout_ = timeout - (millis() - st);
		if (_timeout_ <= 0) break;
		PacketCommand c;
		if (PacketCommand_FromStream(c, (*writeStream), _timeout_) != ProtocolError_None)
			continue;
		if (c.PacketID() != UART_Debug)
			continue;
		return c.PayLoadString();
	}
	return String();
}
DebuggerClass PCDebugger;