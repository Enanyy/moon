local moon = require("moon")
local tcp = require("moon.net.tcpserver")
local protobuf = require("protobuf")
local protoloader = require("protoloader")


local encode = protobuf.encode
local decode = protobuf.decode
local pack = string.pack
local unpack = string.unpack

require("msgid")

local M = 
{
    handler = nil
}



function M:init(handler)

    self.handler = handler
    if self.handler ~= nil then
        self.handler.init()
    end

    protoloader.loadall()

    tcp.settimeout(300)

    tcp.on("accept",function(sessionid, msg)
        print("accept ", sessionid, msg:bytes())
       
        if self.handler then
            self.handler.onaccept(sessionid)
        end
    end)

    tcp.on("message",function(sessionid, msg)
        print("message ", sessionid, msg:bytes())

        if self.handler ~= nil then
            local size = msg:size()
            if size >= 4 then
            
                --id 4个字节
                local bytes = msg:substr(0, 4)
                local data = msg:substr(4, size - 4)

                --id小端编码，有符号
                local id = unpack("<i",bytes)
                
                local def = self.handler.get_def(id)

                if def ~= nil  then
                    local msg = decode(def.name, data)
                    --这里派发消息
                    self.handler.ondispatch(sessionid, id, msg)
                end
            end
        end
    end)

    tcp.on("close",function(sessionid, msg)
        print("close ", sessionid, msg:bytes())
        if self.handler ~= nil then
            self.handler.onclose(sessionid)
        end
    end)

    tcp.on("error",function(sessionid, msg)
        print("error ", sessionid, msg:bytes())

        if self.handler ~= nil then
            self.handler.onerror(sessionid)
        end
    end)
end

function M:encode(id, data)

    if self.handler == nil then
        return
    end
    local def = self.handler.get_def(id) 

    if def == nil then
        print("can not find msg def:"..id)
        return
    end

    local pbdata = encode(def.name,data)
    --id小端编码 4个字节,有符号
    local buffer = pack("<i", id)..pbdata

    return buffer
end


function M:send(sessionid, id, data)

    local buffer = self:encode(id,data)
    --发送str
    tcp.send(sessionid, buffer)
end

function M:send_buffer(sessionid,buffer)

    if buffer == nil then
        return
    end
    tcp.send(sessionid, buffer)
end

moon.exports.netmgr = M

