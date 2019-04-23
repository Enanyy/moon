local moon = require("moon")
local cmd_handler = require("cmd_handler")

local M = setmetatable({}, cmd_handler)

M.CREATE_COPY = function ( sender,header,responseid,... )
    
end
--通过game_service发送已经编码的消息
M.SEND = function (sender,header,responseid, sessionid, buffer )
    netmgr:sendbuffer(sessionid, buffer)
end


return M