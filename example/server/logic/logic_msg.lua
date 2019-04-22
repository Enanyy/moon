local moon = require("moon")
local msg_handler = require("msg_handler")

local M = setmetatable({}, msg_handler)
--注册要监听的消息
function M.init()

    M.register(msgid.LOGIN_REQUEST,{name = "LoginRequest",func = M.login_request})
    M.register(msgid.LOGIN_RETURN,{name = "LoginReturn",func = nil})
    M.register(msgid.LOGIN_GAME_NOTIFY,{name = "LoginGameNotify",func = nil})

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
    usermgr:removeuser_by_sessionid(sessionid)
end

function M.onerror( sessionid )
    usermgr:removeuser_by_sessionid(sessionid)    
end

function M.login_request(sessionid, user, msg )
    -- body
    print("user login:".. msg.name..","..msg.password)
    moon.async(function() 
         
        print("request db login ")
        local data = server.call(serverdef.DB, "LOGIN",msg.name,msg.password)
        
        if data then
            local id = data[1]
            local name = data[2]
            local password =data[3]
            --print(table.tostring(data))
            if id > 0 then
                print("db login result:"..id)
                local u =  usermgr:adduser(sessionid,id)
                u:onlogin()
                local userdata = {
                    id = id,
                    name = name 
                }
                u:send(msgid.LOGIN_RETURN,{result = errordef.SUCCESS, userdata = userdata})

                local game_config = config.get_service("game")
                if game_config and game_config.network then
                    print("return login game")
                    u:send(msgid.LOGIN_GAME_NOTIFY,{ip = game_config.network.ip,port = game_config.network.port})
                end
            end
        else
            print("can not find db service")
            netmgr:send(sessionid,msgid.LOGIN_RETURN,{result = errordef.SYSTEM})
        end
    end)
end

return M

