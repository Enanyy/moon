local action = require("game.battle.action.action")
local M = setmetatable({}, {__index = action})
M.super = action
M.__index = M
function M.new()
    local o = setmetatable(action.new(), M)
    o.type = actiondef.idle
    o.duration = 86400
    o.weight = 0
    o.senddata = {}
    return o
end

--
--overwrite
--
function M:isvalid()
    if self.agent.isdie == true then
        return false
    end
    return true
end

function M:enter()
    -- if self.agent.id == 1 then
    --print(self.agent.id ," enter idle:"..tostring(self.agent.position))
    --     end
    action.enter(self)

    self:broadcast()
end

function M:execute(delta)
    action.execute(self,delta)
end

function M:exit()
    action.exit(self)
    -- if self.agent.id == 1 then
    -- print(self.agent.id ," exit idle")
    -- end
end

function M:broadcast()
    self.senddata.id = self.agent.id
    self.senddata.copy = copy.copyid
    self.senddata.data = self.agent:get_send_data()
    
    copy:broadcast(msgid.BATTLE_ENTITY_IDLE,data)
end
return M