local moon = require("moon")
local cmd = require("game.game_cmd")
require("server")
require("config")
require("netmgr")
require("def")
--注册全局的
moon.exports.copymgr = require("game.copy.copymgr")


moon.init(function(cfg) 
    config.load_config(cfg)
    return true
end)

moon.start(function ()   
    local msg_handler = require("game.game_msg")
    netmgr:init(msg_handler)
end)




moon.dispatch('lua',function(msg, p) 

    cmd.oncommand(cmd,msg,p)
   
end)
