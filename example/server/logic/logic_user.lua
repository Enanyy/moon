local user = require("user.user")
local M= setmetatable({}, {__index = user})
M.super = user
M.__index = M

function  M.new(sessionid, id)
    local o = setmetatable(user.new(sessionid, id),M)

    return o
end


return M