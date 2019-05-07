#pragma once

#define PacketCommandID_Com 1
#define PacketCommandID_FB 2
#define PacketCommandID_Debug 90

#define ProtocolError_Unknown 0
#define ProtocolError_UsernameOrPasswordWrong 1
#define ProtocolError_ServerUnreachable 2
#define ProtocolError_NoInternet 3
#define ProtocolError_None 4
#define ProtocolError_AlreadyConnected 5
#define ProtocolError_CookieMismatch 6
#define ProtocolError_ReadTimeout 7
#define ProtocolError_NodeUnavaiable 8
#define ProtocolError_PairRefused 9
#define ProtocolError_CheckSumMismatch 10
#define ProtocolError_BufferOverFlow 15
