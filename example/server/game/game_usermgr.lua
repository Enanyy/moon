local usermgr = require("user.usermgr")
local M= setmetatable({}, usermgr)

M.__index = M

return M