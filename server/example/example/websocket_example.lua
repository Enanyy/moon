-------------------WEBSOCKET------------------------

do
    local websocket = require("moon.net.websocket")

    websocket.settimeout(10)

    websocket.on("accept",function(sessionid, msg)
        print("wsaccept ", sessionid, msg:bytes())
    end)

    websocket.on("message",function(sessionid, msg)
        --websocket.send(sessionid, msg)
        websocket.send_text(sessionid, msg:bytes())
    end)

    websocket.on("close",function(sessionid, msg)
        print("wsclose ", sessionid, msg:bytes())
    end)

    websocket.on("error",function(sessionid, msg)
        print("wserror ", sessionid, msg:bytes())
    end)
end