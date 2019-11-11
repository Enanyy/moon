local moon = require("moon")
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
        def.func(sessionid, msg)
    end
end

function M.onaccept( sessionid )

end

function M.onclose( sessionid )
    
end

function M.onerror( sessionid )
       
end

function M.login_game_request(sessionid,  msg)
    local userid = msg.id
    print("game_msg:login_game_request->login game service:"..tostring(msg.id))

    if copymgr:userlogin(userid, sessionid) == false then
        
        print("game_msg:login_game_request->login copymgr failed:")

        netmgr:send_then_close(sessionid,msgid.LOGIN_GAME_RETURN,{result = errordef.SYSTEM})
        return
    end

    local copy = copymgr:getcopy_by_userid(userid)
    if copy then
        moon.async(function() 
        
            print("game_msg:login_game_request->login copy:",copy.id)
            --登陆副本
            local result =  moon.co_call("lua", copy.id, "LOGIN_COPY",  {userid = userid,sessionid = sessionid})
            
            print("game_msg:login_game_request->login copy return:",result)

            if result == errordef.SUCCESS then
                netmgr:send(sessionid, msgid.LOGIN_GAME_RETURN,{result = errordef.SUCCESS})
            else
                netmgr:send_then_close(sessionid,msgid.LOGIN_GAME_RETURN,{result = errordef.SYSTEM})
            end

        end)
        
    else
        netmgr:send_then_close(sessionid,msgid.LOGIN_GAME_RETURN,{result = errordef.SYSTEM})
    end
end

return M