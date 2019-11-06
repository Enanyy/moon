local moon = require("moon")
local cmd_handler = require("cmd_handler")
local M = setmetatable({}, cmd_handler)

M.INIT = function (sender,responseid, data )
    
    print("copy_cmd:INIT ->begin init copy data:",data.copyname)

    copy:init(data)

    local result = errordef.SUCCESS

    moon.response('lua',sender,responseid, result)
  
end

M.LOGIN_COPY = function(sender,responseid, data )

    print("copy_cmd:LOGIN_COPY ->login copy:",table.tostring(data))

    local result = errordef.SUCCESS
    if copy:login(data.userid,data.sessionid) == false then
        result = errordef.LOGIN_COPY_ERROR
    end
    moon.response('lua',sender,responseid, result)
end

return M