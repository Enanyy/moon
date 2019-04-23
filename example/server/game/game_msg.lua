local msg_handler = require("msg_handler")


local M = setmetatable({}, msg_handler)
--注册要监听的消息
function M.init()
    msg_handler.init()
    M.register(msgid.LOGIN_GAME_REQUEST, M.login_game_request)
end
--重写ondispatch方法
function M.ondispatch(sessionid, id, msg )
    
    local def = M.get_def(id)
    if def ~= nil and def.func ~= nil then
        local u = usermgr:getuser_by_sessionid(sessionid)
        def.func(sessionid, u, msg)
    end
end

function M.onaccept( sessionid )

end

function M.onclose( sessionid )
    
end

function M.onerror( sessionid )
       
end

function M.login_game_request(sessionid, u, msg)

    print("登陆game:"..table.tostring(msg))
    local u = user.new(sessionid,msg.id)
    usermgr:adduser(u)
    
end

return M