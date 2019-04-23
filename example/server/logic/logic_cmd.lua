local cmd_handler = require("cmd_handler")
local M = setmetatable({}, cmd_handler)

--通过逻辑服务器发送给客户端

M.SEND = function ( sender,header,responseid, sessionid, buffer )
    netmgr:sendbuffer(sessionid, buffer)
end


return M