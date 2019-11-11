#pragma once
#include "common/macro_define.hpp"

namespace moon
{
    constexpr int32_t WORKER_ID_SHIFT = 24;
    constexpr int64_t UPDATE_INTERVAL = 10; //ms
    constexpr int32_t BUFFER_HEAD_RESERVED = 10;//max : websocket header  max  len

    DECLARE_UNIQUE_PTR(message);
    DECLARE_SHARED_PTR(buffer);
    DECLARE_UNIQUE_PTR(service);

    constexpr uint8_t PTYPE_UNKNOWN = 0;
    constexpr uint8_t PTYPE_SYSTEM = 1;
    constexpr uint8_t PTYPE_TEXT = 2;
    constexpr uint8_t PTYPE_LUA = 3;
    constexpr uint8_t PTYPE_SOCKET = 4;
    constexpr uint8_t PTYPE_ERROR = 5;
    constexpr uint8_t PTYPE_SOCKET_WS = 6; //websocket

    //network
    using message_size_t = uint16_t;
    constexpr message_size_t MAX_NET_MSG_SIZE = 0x7FFF;
    constexpr size_t WARN_NET_SEND_QUEUE_SIZE = 200;
    constexpr size_t MAX_NET_SEND_QUEUE_SIZE = 300;

    constexpr  string_view_t STR_LF = "\n"sv;
    constexpr  string_view_t STR_CRLF = "\r\n"sv;
    constexpr  string_view_t STR_DCRLF = "\r\n\r\n"sv;

    enum class buffer_flag :uint8_t
    {
        pack_size = 1 << 0,
        close = 1 << 1,
        framing = 1 << 2,
        broadcast = 1 << 3,
        ws_text = 1 << 4,
        ws_binary = 1 << 5,
        buffer_flag_max,
    };

	static constexpr std::string_view LUA_DEFAULT_PATH = "./lualib"sv;

	static constexpr std::string_view LUA_DEFAULT_CPATH = "./clib"sv;

	static constexpr std::string_view LUA_PATH_STR = "/?.lua;"sv;

#if TARGET_PLATFORM == PLATFORM_WINDOWS
	static constexpr std::string_view LUA_CPATH_STR = "/?.dll;"sv;
#else
	static constexpr std::string_view LUA_CPATH_STR = "/?.so;"sv;
#endif
}





