local moon = require("moon")
local cmd_handler = require("cmd_handler")
local M = setmetatable({}, cmd_handler)

M.INIT = function (sender,header,responseid, ... )
    local data = {...}


    local result = 0

    moon.response('lua',sender,responseid, result)
  
end

return M