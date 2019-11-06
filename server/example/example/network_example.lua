--Echo Server Example

do

	local core = require("moon.core")
	local Vector2 = core.Vector2
    local v = Vector2.new(20.0, 12.8)
	
	local protobuf = require("protobuf")
    local protoloader = require("protoloader")
    protoloader.loadall()

	local pb_data = protobuf.encode("Person",{
		header = {
			cmd = 132,
			seq = 291
		},
		id = 546430015121,
		name = "Enanyy",
		age = 26,
		email = "742216502@qq.com",
		array = {
			2,
			5,
			8
		}

	})
	local data = protobuf.decode("Person",pb_data)
    print(data.name)
    
    local fs = require("fs")
    print(fs.working_directory())
    	
    -------------------2 bytes len (big endian) protocol------------------------
    local tcpserver = require("moon.net.tcpserver")

    tcpserver.settimeout(300)

    tcpserver.on("accept",function(sessionid, msg)
        print("accept ", sessionid, msg:bytes())
    end)

    tcpserver.on("message",function(sessionid, msg)
        --tcpserver.send(sessionid, msg:bytes())
        local size = msg:size()

        if size >= 4 then
           
            local bytes = msg:substr(0, 4)
            local data = msg:substr(4, size - 4)

            local id = string.unpack("<I",bytes)

            print(id == 1361311)

            print("size = "..size)
            print("id = "..id)
            print("data = "..data)
        end
        
        local senddata = "aaasaa"
        local id = 2210161
        local sendmsg = string.pack("<I",id)..senddata
        local len = #sendmsg
     
        print(len)

		--print("recv:"..sessionid .." data:"..msg:bytes())
        tcpserver.send(sessionid, sendmsg)
    end)

    tcpserver.on("close",function(sessionid, msg)
        print("close ", sessionid, msg:bytes())
    end)

    tcpserver.on("error",function(sessionid, msg)
        print("error ", sessionid, msg:bytes())
    end)
end


