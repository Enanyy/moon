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
        print("logic_msg.ondispatch->can not find def or func:" ..id)
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
    print("logic_msg.login_request->user login:".. msg.name..","..msg.password)
    moon.async(function() 
         
        print("logic_msg.login_request->request db login ")
        local data = server.call(serverdef.DB, "LOGIN",msg)
        print("logic_msg.login_request->login db result:",table.tostring(data))
        if data then
            
            --print(table.tostring(data))
            if type(data.id) == "number" and data.id > 0 then
                print("logic_msg.login_request->db login result:"..tostring(data.id))

                local u = user.new(sessionid,data.id)
                usermgr:adduser(u)
                u:onlogin()
                local userdata = {
                    id = data.id,
                    name = data.name 
                }
                u:send(msgid.LOGIN_RETURN,{result = errordef.SUCCESS, userdata = userdata})
            else
                print("logic_msg.login_request->login db error:",data.id)
                netmgr:send(sessionid,msgid.LOGIN_RETURN,{result = data.id})        
            end
        else
            print("logic_msg.login_request->can not find db service")
            netmgr:send(sessionid,msgid.LOGIN_RETURN,{result = errordef.SYSTEM})
        end
    end)
end

function M.battle_begin_request(sessionid, u, msg )

     print("logic_msg.battle_begin_request->request battle begin:",u.id)
     moon.async(function() 
        
        local data ={
            copyname = "copy_test",
            users ={
                {
                    userid = u.id,
                    camp = 1,
                    type = userdef.USER,
                    heros = {
                        {config = 1, count = 1},
                        {config = 2, count = 1},
                    }
                },
                {
                    userid = 1000,
                    camp = 2,
                    type = userdef.MONSTER,
                    heros = {
                        {config = 1, count = 1},
                        {config = 2, count = 1},
                    }
                },
            }
            
        }
        local result = errordef.SYSTEM 
        print("logic_msg.battle_begin_request->request game create copy")
        local ret = server.call(serverdef.GAME, "CREATE_COPY", data  )
        print("logic_msg.battle_begin_request->request game create copy return:",ret)
        if type(ret)=="table" then
            result = ret.result
            if result == errordef.SUCCESS then
                local game_config = config.get_service("game")
                if game_config and game_config.network then
                    print("logic_msg.battle_begin_request->return login game")
                    u:send(msgid.LOGIN_GAME_NOTIFY,{ip = game_config.network.ip,port = game_config.network.port})
                else
                    result = errordef.SYSTEM
                end
            end
        end
    
        u:send(msgid.BATTLE_BEGIN_RETURN,{result = result})
    end)
    
 
end

return M

