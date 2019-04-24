local moon = require("moon")
local tcp = require("moon.net.tcpserver")
local M = 
{
    handler = nil
}


function M:init(handler)

    self.handler = handler
    if self.handler ~= nil then
        self.handler.init()
    end

    tcp.settimeout(300)

    tcp.on("accept",function(sessionid, msg)
        print("accept ", sessionid, msg:bytes())
       
        if self.handler then
            self.handler.onaccept(sessionid)
        else
            print("msg_handler is nil")
        end
    end)

    tcp.on("message",function(sessionid, msg)
        print("message ", sessionid, msg:bytes())

        if self.handler ~= nil then
           
            local data = self.handler.decode(msg:bytes())
            if data ~= nil then
                --这里派发消息
                self.handler.ondispatch(sessionid, data.id, data.msg)   
            end
        else
            print("msg_handler is nil")
        end
    end)

    tcp.on("close",function(sessionid, msg)
        print("close ", sessionid, msg:bytes())
        if self.handler ~= nil then
            self.handler.onclose(sessionid)
        else
            print("msg_handler is nil")
        end
    end)

    tcp.on("error",function(sessionid, msg)
        print("error ", sessionid, msg:bytes())

        if self.handler ~= nil then
            self.handler.onerror(sessionid)
        else
            print("msg_handler is nil")
        end
    end)
end

--sessionid 会话id
--id msgid
--data msgdata
function M:send(sessionid, id, data)

    if self.handler == nil then
        error("netmgr:send: do not register msg_handler")
        return
    end
    local buffer = self.handler.encode(id,data)
    --发送str
    tcp.send(sessionid, buffer)

    print("netmgr:send sessionid:",sessionid, " id:",id," size:",#buffer)
end

--sessionid 会话id
--buffer encoded msg
function M:sendbuffer(sessionid,buffer)

    if buffer == nil then
        return
    end
    tcp.send(sessionid, buffer)
end

function M:send_then_close(sessionid,id, data)
    if self.handler == nil then
        error("netmgr:send: do not register msg_handler")
        self:close(sessionid)
        return
    end
    local buffer = self.handler.encode(id,data)
    --发送str
    tcp.send_then_close(sessionid, buffer)

    print("netmgr:send sessionid:",sessionid, " id:",id," size:",#buffer)
end

function M:close(sessionid)

    tcp:close(sessionid)

end

moon.exports.netmgr = M

