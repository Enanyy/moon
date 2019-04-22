local action = require("game.battle.action.action")
local M = setmetatable({}, {__index = action})
M.super = action
M.__index = M
function M.new()
    local o = setmetatable(action.new(), M)
    o.type = actiondef.attack
    o.duration = 2
    o.weight = 2
    return o
end

function M:enter()
    action.enter(self)
    -- if self.agent.id == 1 then
    print(self.agent.id ," enter attack:"..tostring(self.agent.position))
    -- end
    if self.agent.target ~= nil then

        self.agent.target:hurtby(self.agent)

    end

end

function M:execute(delta)
    action.execute(self,delta)
end

function M:exit()
    action.exit(self)
    -- if self.agent.id == 1 then
    -- print(self.agent.id ," exit attack")
    -- end
end

return M