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

    return o
end

function M:setdestination(destination)

    self.destination = destination
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
    action.enter(self)
    -- if self.agent.id == 1 then
    --   print(self.agent.id.. ' enter run'.." position:"..tostring(self.agent.position).. "destination:"..tostring(self.destination))
    -- end

    if self.agent:needsync() then
        self.sync = true
    end
    self.send = false
    
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

    local vel = {}
    local precision = 100
    local x, mod = math.modf( velocity.x * 100 / 1 )
    if mod >= 0.5 then
        x = x + 1
    end
    vel.x = x / precision
    local y, mod = math.modf( velocity.y * precision / 1 )
    if mod >= 0.5 then
        y = y + 1
    end
    vel.y = y / precision

    if self.senddata == nil then
        self.senddata = {}
    end

    if self.senddata.velocity ~= nil and self.senddata.velocity.x == vel.x and self.senddata.velocity.y == vel.y then
        return
    end

    self.senddata.id = self.agent.id
    self.senddata.copy = copy.copyid
    self.senddata.position = {x = self.agent.position.x, y = self.agent.position.y}
    self.senddata.velocity ={x= velocity.x,y = velocity.y}
    self.senddata.movespeed = self.agent.movespeed * 100
    
    copy:broadcast(msgid.BATTLE_ENTITY_RUN_NOTIFY,self.senddata)

    self.senddata.velocity.x = vel.x
    self.senddata.velocity.y = vel.y
  
end

return M