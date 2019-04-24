local cmd_handler = require("cmd_handler")
local M = setmetatable({}, cmd_handler)

--通过逻辑服务器发送给客户端

M.SEND = function ( sender,responseid, userid, id, data )
    local u = usermgr:getuser(userid)

    if u ~= nil then
        u:send(id, data)
    end
end


return M