local moon = require("moon")
local cmd = require("logic.logic_cmd")
require("server")
require("config")
require("def")
require("netmgr")
--注册全局的
moon.exports.user =  require("logic.logic_user")
moon.exports.usermgr = require("logic.logic_usermgr")



moon.init(function(cfg) 
    config.load_config(cfg)
    return true
end)

moon.start(function ()   
    local msg_handler = require("logic.logic_msg")
    netmgr:init(msg_handler)
    -- moon.async(function() 
    
    --     server.call(serverdef.GAME, "CREATE_COPY", table.unpack({"AAA","abc"}))
       
    -- end)
    
end)

moon.dispatch('lua',function(msg, p) 

    cmd.oncommand(cmd,msg,p)

end)