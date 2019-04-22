local M = {}
M.__index = M

function M.new()
    local o = {}
    o.id = 1
    o.name = "hero"
    o.hp = 100
    o.movespeed = 6
    o.attackspeed = 2

    return setmetatable(o, M)
end

return M