#pragma once
#include <string>
#include "sol.hpp"
#include "common/noncopyable.hpp"

class lua_service;

namespace moon
{
    class server;
    class log;
}

class lua_bind :public moon::noncopyable
{
public:
    explicit lua_bind(sol::table& lua);
    ~lua_bind();

    const lua_bind& bind_timer(lua_service* s) const;

    const lua_bind& bind_util() const;

    const lua_bind& bind_log(moon::log* logger) const;

    const lua_bind& bind_message() const;

    const lua_bind& bind_service(lua_service* s) const;

    const lua_bind& bind_socket()const;

	const lua_bind& bind_rvo2()const;

    static void registerlib(lua_State *L, const char *name, lua_CFunction f);

    static void registerlib(lua_State *L, const char *name, const sol::table&);
private:
    sol::table& lua;
};

const char* lua_traceback(lua_State* _state);

extern "C"
{
    int luaopen_rapidjson(lua_State* L);

    int luaopen_fs(lua_State* L);

    int luaopen_http(lua_State* L);

	int	LUAMOD_API luaopen_protobuf_c(lua_State *L);

	int LUAMOD_API luaopen_aoi(lua_State *L);

	int LUAMOD_API luaopen_bit32(lua_State *L);
	
	int LUAMOD_API luaopen_mysql(lua_State *L);
}

