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
--
--overwrite
--
function M:isvalid()
    if self.agent.isdie == true then
        return false
    end
    return self.agent.target ~= nil
end

function M:enter()
    action.enter(self)
    -- if self.agent.id == 1 then
    --    print(self.agent.id ," enter attack:"..tostring(self.agent.position))
    -- end
    if self.agent.target ~= nil then

        self.agent.target:hurtby(self.agent)

    end

    self:broadcast()
    
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

function M:broadcast()
    local data = {
        id = self.agent.id,
        copy = copy.copyid,
        skill = 1,
        attackspeed = self.agent.attackspeed,
        target = self.agent.target.id,
        data = self.agent:get_send_data()
    }

    copy:broadcast(msgid.BATTLE_ENTITY_ATTACK_NOTIFY,data)
end

return M