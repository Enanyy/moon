local moon = require("moon")
local cmd_handler = require("cmd_handler")

local M = setmetatable({}, cmd_handler)

M.CREATE_COPY = function ( sender,header,responseid, name, ... )
    print("begin create copy :"..name)
    local data = {...}
    copymgr:create(name,  function(copyid) 
       
        print("create copy success:",copyid)
       
         --设置副本数据
         moon.co_call("lua", copyid, "INIT",  table.unpack(data))

        --返回给logic
        moon.response('lua',sender,responseid, copyid)

    end, ...)
end
--通过game_service发送已经编码的消息
M.SEND = function (sender,header,responseid, sessionid, buffer )
    netmgr:sendbuffer(sessionid, buffer)
end


return M