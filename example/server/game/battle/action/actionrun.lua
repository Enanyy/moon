local action = require("game.battle.action.action")
local pi = math.pi

local M = setmetatable({}, {__index = action})
M.super = action
M.__index = M
function M.new()
    local o = setmetatable(action.new(), M)
    o.type = actiondef.run
    o.duration = 86400
    o.weight = 1

    o.destination = vector2.zero()

    o.sync = false

    o.senddata = {}
    return o
end

function M:setdestination(destination)

    self.destination = destination
end


function M:enter()
    action.enter(self)
        -- if self.agent.id == 1 then
        --    print(self.agent.id.. ' enter run'.." position:"..tostring(self.agent.position).. "destination:"..tostring(self.destination))
        -- end

    if self.agent:needsync() then
        self.sync = true
    end
    
end

function M:execute(delta)

    action.execute(self,delta)

    if self.agent.sid >= 0 then

        if self.sync then
            rvo2:setAgentPrefVelocity(self.agent.sid, vector2.new(0,0))
            local position = rvo2:getAgentPosition(self.agent.sid)

            local direction = position - self.agent.position
            local displacement = delta * self.agent.movespeed
            
            if direction:magnitude() >= displacement then
                
                self.agent.direction = direction:normalized()
                local velocity = self.agent.direction * displacement
                self.agent.position = self.agent.position + velocity
    
                self:broadcast(velocity)
            else
                self.sync = false
            end
        else
           
            local direction = (self.destination - self.agent.position):normalized() 

            local vel = vector2.new(direction.x , direction.y)* self.agent.movespeed
            local angle = math.random( 0.0, 1.0 ) * 2.0 * pi
            local dist = math.random( 0.0,1.0 ) *  0.0001
            local offset = vector2.new(math.cos( angle ),math.sin( angle )) * dist
            local velocity = vel + offset
            
            rvo2:setAgentPrefVelocity(self.agent.sid, velocity)

            self.agent.position = rvo2:getAgentPosition(self.agent.sid)
         
            local v =  rvo2:getAgentPrefVelocity(self.agent.sid)
                  
            if math.abs( v.x ) < 0.01 and math.abs( v.y ) < 0.01 then
                self:done()
            else
                self.agent.direction = v
            end
            self:broadcast(v)
        end
    
    else
        self:done()
    end
--[[ 
    local direction = self.destination - self.agent.position
    local distance = direction:magnitude()
    local displacement = delta * self.agent.movespeed
    self.agent.direction = direction:normalized() 
    if distance >= displacement then
        self.agent.position = self.agent.position + self.agent.direction  * displacement
    else
        self.agent.position = self.destination
        self:done()
    end
--]]
    --if self.agent.type == userdef.USER then
      --  print(self.agent.id.. ' execute run:'..tostring(self.agent.position).." destination:"..tostring(self.destination).." distance:"..tostring((self.destination - self.agent.position):magnitude()))
    --end
end

function M:exit()
    action.exit(self)
    -- if self.agent.id == 1 then
    --      print(self.agent.id.. ' exit run')
    -- end

    self.sync = false
    if self.agent.sid >= 0 then
        rvo2:setAgentPrefVelocity(self.agent.sid, vector2.new(0,0))
    end
end

function M:pause()
    action.pause(self)
    if self.agent.sid >= 0 then
        rvo2:setAgentPrefVelocity(self.agent.sid, vector2.new(0,0))
    end
end


function M:broadcast(velocity)

    if velocity == nil then
        return
    end

    if self.senddata.velocity ~= nil and (self.senddata.velocity.x == velocity.x and self.senddata.velocity.y == velocity.y) then
        return
    end
    
    self.senddata.id = self.agent.id
    self.senddata.copy = copy.copyid
    self.senddata.velocity = {x =velocity.x, y = velocity.y }
    self.senddata.data = self.agent:get_send_data()
    
    copy:broadcast(msgid.BATTLE_ENTITY_RUN,self.senddata)
end

return M