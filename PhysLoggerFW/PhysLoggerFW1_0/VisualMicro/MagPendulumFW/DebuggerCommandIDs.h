#pragma once


#define UART_Debug 90				 
#define UART_Debugln 91 
#define UART_Debug_SwitchToSecondary 92
#define UART_Debug_SwitchToDefault 93
#define UART_Debug_PrintBuffer 99

#define UART_FSE_GetContents 101
#define UART_FSE_GetSize 102
#define UART_FSE_GetFile 103
#define UART_FSE_Remove 104
#define UART_FSE_CreateFile 105
#define UART_FSE_CreateFolder 106
#define UART_FSE_Rename 107
#define UART_FSE_CheckExist 108

#define UART_FSE_Resp_GetContents 109
#define UART_FSE_Resp_GetSize 110
#define UART_FSE_Resp_GetFile 111
#define UART_FSE_Resp_Success 112
#define UART_FSE_Resp_Failed 113
#define UART_FSE_Resp_DoesntExist 114
#define UART_FSE_Resp_Exists 115
#define UART_FSE_Resp_GetFile_Header 116
#define UART_FSE_Resp_GetFile_Footer 117
#define UART_FSE_Resp_CreateFile_Failure 118
#define UART_FSE_Resp_CreateFile_ACK 119
#define UART_FSE_Resp_CreateFile_DataPacket 120	 
#define UART_FSE_Resp_GetContents_EXT 122
#define UART_FSE_Resp_GetFile_DataPart 123
#define UART_FSE_GetFile_NextPart 124

#define UART_ChangeDebugChannel 130