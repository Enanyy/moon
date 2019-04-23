local moon = require("moon")
local cmd_handler = require("cmd_handler")

local M = setmetatable({}, cmd_handler)

M.CREATE_COPY = function ( sender,header,responseid, data)
    print("begin create copy :"..data.copyname)

    data.sid = moon.sid()
    copymgr:create(data,  function(copyid) 
       
        print("create copy success:",copyid)
       
         --设置副本数据
        local result =  moon.co_call("lua", copyid, "INIT",  data)

        --返回给logic
        moon.response('lua',sender,responseid, copyid)

    end)
end
--通过game_service发送已经编码的消息
M.SEND = function (sender,header,responseid, sessionid, buffer )
    netmgr:sendbuffer(sessionid, buffer)
end


return M