local moon = require("moon")
local cmd_handler = require("cmd_handler")
local M = setmetatable({}, cmd_handler)

M.INIT = function (sender,header,responseid, data )
    
    print("init copy data:",data.copyname)

    local result = 0

    moon.response('lua',sender,responseid, result)
  
end

return M