local moon = require("moon")
local cmd_handler = require("cmd_handler")

local M = setmetatable({}, cmd_handler)
--通过game_service发送消息
M.SEND = function (sender,responseid,  sessionid, id, data )
    netmgr:send(sessionid,id,data)
    moon.response("lua",sender,response,errordef.SUCCESS)
end

M.CREATE_COPY = function ( sender,responseid, data)
    print("game_cmd:CREATE_COPY->begin create copy :"..data.copyname)

    data.sid = moon.sid()
    copymgr:create(data,  function(result, copyid) 
       
        if result == errordef.SUCCESS then
            print("game_cmd:CREATE_COPY->create copy success:",copyid)
       
        else
            print("game_cmd:CREATE_COPY->create copy failed:",copyid)
        end
        --返回给logic
        moon.response('lua',sender,responseid, { result = result,copyid = copyid})
        
    end)
end


M.DESTROY_COPY = function(sender, responseid, id)
    copymgr:remove(id)
end


return M