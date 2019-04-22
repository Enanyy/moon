#pragma  once
#include "service.h"
#include "common/log.hpp"
#include "luabind/lua_bind.h"
#include "components/tcp/tcp.h"
#include "common/buffer.hpp"
#include "config.h"
class lua_service :public moon::service
{
public:
	
    using lua_state_ptr_t = std::unique_ptr<lua_State, void(*)(lua_State*)>;
    /*
    http://sol2.readthedocs.io/en/latest/safety.html
    http://sol2.readthedocs.io/en/latest/api/protected_function.html
    http://sol2.readthedocs.io/en/latest/errors.html
    */
    using sol_function_t = sol::function;

    lua_service();

    ~lua_service();

    size_t memory_use();

    void set_callback(char c, sol_function_t f);

    moon::tcp* get_tcp(const std::string& protocol);

    uint32_t prepare(const moon::buffer_ptr_t & buf);

    void send_prepare(uint32_t receiver, uint32_t cacheid, const  moon::string_view_t& header, int32_t responseid, uint8_t type) const;
private:
    bool init(moon::string_view_t config) override;

    void start()  override;

    void exit() override;

    void destroy() override;

    void dispatch(moon::message* msg) override;

    void on_timer(uint32_t timerid, bool remove) override;

    void error(const std::string& msg);

    static void* lalloc(void * ud, void *ptr, size_t osize, size_t nsize);
public:
    size_t mem = 0;
    size_t mem_limit = 0;
    size_t mem_report = 8 * 1024 * 1024;
private:
    sol::state lua_;
    sol_function_t init_;
    sol_function_t start_;
    sol_function_t dispatch_;
    sol_function_t exit_;
    sol_function_t destroy_;
    sol_function_t on_timer_;
};