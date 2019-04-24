local action = require("game.battle.action.action")
local M = setmetatable({}, {__index = action})
M.super = action
M.__index = M
function M.new()
    local o = setmetatable(action.new(), M)
    o.type = actiondef.die
    o.duration = 2
    o.weight = 10
    o.senddata = {}
    return o
end


function M:enter()
    print(self.agent.id ," enter die:"..tostring(self.agent.position))
    action.enter(self)

    self:broadcast()
end

function M:execute(delta)
    action.execute(self,delta)
end

function M:exit()
    action.exit(self)

    --print(self.agent.id ," exit die")

    agentmgr:removeagent(self.agent.id)
end

function M:broadcast()

    self.senddata.id = self.agent.id
    self.senddata.copy = copy.copyid
    
    copy:broadcast(msgid.BATTLE_ENTITY_DIE,self.senddata)
end

return M