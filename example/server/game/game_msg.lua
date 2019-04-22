local msg_handler = require("msg_handler")
local copymgr = require("game.copy.copymgr")

local M = setmetatable({}, msg_handler)
--注册要监听的消息
function M.init()
    M.register(msgid.LOGIN_GAME_REQUEST,{name = "LoginGameRequest",func = M.login_game_request})
    M.register(msgid.LOGIN_GAME_RETURN,{name = "LoginGameReturn",func = nil})
end
--重写ondispatch方法
function M.ondispatch(sessionid, id, msg )
    
    local def = M.get_def(id)
    if def ~= nil and def.func ~= nil then
        
        def.func(sessionid, msg)
    end
end

function M.onaccept( sessionid )

end

function M.onclose( sessionid )
    
end

function M.onerror( sessionid )
       
end

function M.login_game_request(sessionid, msg)

    
end

return M