local moon = require("moon")
local mysql = require("mysql")
local cmd = require("db.db_cmd")
require("server")
require("config")
require("def")

moon.init(function(cfg) 
    config.load_config(cfg)
    return true
end)

moon.start(function ()   
    local cfg = config.get_service("db")
    if cfg and cfg.mysql then
        local conn = mysql.create()
        local ret,errmsg = conn:connect(cfg.mysql.ip, cfg.mysql.port, cfg.mysql.user, cfg.mysql.password, cfg.mysql.database,cfg.mysql.timeout)
        if ret then
            print("connect mysql success:",cfg.mysql.ip, cfg.mysql.port, cfg.mysql.user, cfg.mysql.password, cfg.mysql.database)
            cmd.init(conn)
        else
            print(ret,errmsg)
           
            moon.removeself()
        end

    else
        moon.removeself()
    end
end)

moon.dispatch('lua',function(msg, p) 

    cmd.oncommand(cmd,msg,p)

end)