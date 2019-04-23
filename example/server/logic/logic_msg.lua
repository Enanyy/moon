local moon = require("moon")
local msg_handler = require("msg_handler")

local M = setmetatable({}, msg_handler)
--注册要监听的消息
function M.init()
    msg_handler.init()
    M.register(msgid.LOGIN_REQUEST, M.login_request)
    M.register(msgid.BATTLE_BEGIN_REQUEST, M.battle_begin_request)
end
--重写ondispatch方法
function M.ondispatch(sessionid, id, msg )
    
    local def = M.get_def(id)
    if def ~= nil and def.func ~= nil then
        local u = usermgr:getuser_by_sessionid(sessionid)
        def.func(sessionid, u, msg)
    else
        print("can not find def or func:" ..id)
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

function M.login_request(sessionid, u, msg )
    -- body
    print("user login:".. msg.name..","..msg.password)
    moon.async(function() 
         
        print("request db login ")
        local data = server.call(serverdef.DB, "LOGIN",msg)
        print("login db result:",table.tostring(data))
        if data then
            
            --print(table.tostring(data))
            if type(data.id) == "number" and data.id > 0 then
                print("db login result:"..tostring(data.id))

                local u = user.new(sessionid,data.id)
                usermgr:adduser(u)
                u:onlogin()
                local userdata = {
                    id = data.id,
                    name = data.name 
                }
                u:send(msgid.LOGIN_RETURN,{result = errordef.SUCCESS, userdata = userdata})
            else
                print("login db error:",data.id)
                netmgr:send(sessionid,msgid.LOGIN_RETURN,{result = data.id})        
            end
        else
            print("can not find db service")
            netmgr:send(sessionid,msgid.LOGIN_RETURN,{result = errordef.SYSTEM})
        end
    end)
end

function M.battle_begin_request(sessionid, u, msg )

     moon.async(function() 
        
        local data ={
            copyname = "copy_test",
            users ={
                {
                    id = u.id,
                    camp = 1,
                    heros = {
                        {config = 1, count = 1},
                        {config = 2, count = 2},
                    }
                },
                {
                    id = 0,
                    camp = 2,
                    heros = {
                        {config = 1, count = 1},
                        {config = 2, count = 2},
                    }
                },
            }
            
        }

        local copyid = server.call(serverdef.GAME, "CREATE_COPY", data  )
       
    end)
    
    local result = errordef.SUCCESS
    local game_config = config.get_service("game")
    if game_config and game_config.network then
        print("return login game")
        u:send(msgid.LOGIN_GAME_NOTIFY,{ip = game_config.network.ip,port = game_config.network.port})
    else
        result = errordef.SYSTEM
    end

    u:send(msgid.BATTLE_BEGIN_RETURN,{result = result})
end

return M

