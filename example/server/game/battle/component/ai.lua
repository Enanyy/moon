local component = require("game.battle.component.component")
local M = setmetatable({}, {__index = component})
M.super = component
M.__index = M
function M.new()
    local o = setmetatable(component.new(), M)
    o.type = componentdef.ai
    return o
end

function M:start()
   component.start(self)
end

function M:update(delta)
    component.update(self,delta)
   
end

function M:destroy()
    component.destroy(self)   
end


function M:enter( action )
    
end

function M:execute(action, delta)
 
    if action.type == actiondef.idle then
        if self.agent.target == nil then
            self.agent.target = agentmgr:getclosetagent(self.agent)
        end 
        if self.agent.target ~= nil then
            if self.agent.target.isdie then
                self.agent.target = nil
            end
        end
        if self.agent.target ~= nil then
            local destination = self.agent.target.position

            local distance = vector2.distance(self.agent.position,destination)

            if distance > self.agent.attackdistance then

                local run = self.agent:getfirst(actiondef.run)
                if run == nil then
                    run = actionrun.new()

                    run:setdestination(destination)

                    self.agent:play(run)

                else
                    run:setdestination(destination)

                end

            else
                local attack = actionattack.new()
                self.agent:play(attack)
            end
        end

    elseif action.type == actiondef.run then
        if self.agent.target ~= nil then
            if self.agent.target.isdie then
                self.agent.target = nil
            end
        end
        if self.agent.target ~= nil then
            local destination = self.agent.target.position

            local distance = vector2.distance(self.agent.position,destination)

            if distance > self.agent.attackdistance then

                action:setdestination(destination)

            else
                local attack = actionattack.new()
                self.agent:play(attack)
            end

        end
    end
end

function M:exit( action )
    if action.type == actiondef.attack then
        if self.agent.target ~= nil then
            if self.agent.target.isdie then
                self.agent.target = nil
            end
        end
        if self.agent.target ~= nil then
            local destination = self.agent.target.position

            local distance = vector2.distance(self.agent.position,destination)

            if distance > self.agent.attackdistance then

                local run = self.agent:getfirst(actiondef.run)
                if run == nil then
                    run = actionrun.new()

                    run:setdestination(destination)

                    self.agent:play(run)

                else
                    run:setdestination(destination)

                end

            else
                local attack = actionattack.new()
                self.agent:play(attack)
            end

        end
    end
end

function M:pasue( action )
    -- body
end

function M:resume(action )
    -- body
end

return M
