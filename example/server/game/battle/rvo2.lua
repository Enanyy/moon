local core = require("moon.core")
local Vector2 = core.Vector2
local M = {
    DEFAULT_NEIGHBOR_DIST = 4,
    DEFAULT_MAX_NEIGHBOR = 5,
    DEFAULT_TIME_HORIZON = 5,
    DEFAULT_TIME_HORIZONOBST = 5,
    DEFAULT_RADIUS = 1,
    DEFAULT_MAX_SPEED = 20,

    simulator = nil,
}

function M:init(timestep, position)
    self.simulator = core.RVOSimulator.new(timestep,
        self.DEFAULT_NEIGHBOR_DIST,
        self.DEFAULT_MAX_NEIGHBOR,
        self.DEFAULT_TIME_HORIZON,
        self.DEFAULT_TIME_HORIZONOBST,
        self.DEFAULT_RADIUS,
        self.DEFAULT_MAX_SPEED,Vector2.new(position.x, position.y))

end


function M:addAgent(position,radius)
    if self.simulator == nil then
        return -1
    end
    return self.simulator:addAgent(Vector2.new(position.x,position.y),
                                    self.DEFAULT_NEIGHBOR_DIST,
                                    self.DEFAULT_MAX_NEIGHBOR,
                                    self.DEFAULT_TIME_HORIZON,
                                    self.DEFAULT_TIME_HORIZONOBST,
                                    radius,
                                    self.DEFAULT_MAX_SPEED,
                                    Vector2.new(0,0))
end


function M:getAgentPosition(sid)
    if self.simulator == nil then
        return vector2.zero()
    end
    local pos = self.simulator:getAgentPosition(sid)

    return vector2.new(pos:x(),pos:y())
end

function M:getAgentPrefVelocity(sid)
    if self.simulator == nil then
        return vector2.zero()
    end
    local vel = self.simulator:getAgentPrefVelocity(sid)
    return vector2.new(vel:x(), vel:y())
end

function M:setAgentPrefVelocity(sid, velocity)
    if self.simulator == nil then
        return 
    end
    self.simulator:setAgentPrefVelocity(sid, Vector2.new(velocity.x, velocity.y))
end


function M:getAgentVelocity(sid)
    if self.simulator == nil then
        return vector2.zero()
    end
    local vel = self.simulator:getAgentVelocity(sid)
    return vector2.new(vel:x(), vel:y())
end

function M:setAgentVelocity(sid, velocity)
    if self.simulator == nil then
        return 
    end
    self.simulator:setAgentVelocity(sid, Vector2.new(velocity.x, velocity.y))
end

function M:getAgentRadius( sid )
    if self.simulator == nil then
        return 0
    end
    return self.simulator:getAgentRadius(sid)
end

function M:setAgentRadius(sid, radius )
    if self.simulator == nil then
        return 
    end
    self.simulator:setAgentRadius(sid, radius)
end

function M:getTimeStep( )
    if self.simulator == nil then
        return 0
    end
    return self.simulator:getTimeStep()
end

function M:setTimeStep(timestep)
    if self.simulator == nil then
        return 
    end
    self.simulator:setTimeStep(timestep)
end

function M:doStep( )
    if self.simulator == nil then
        return 
    end
    self.simulator:doStep()
end

function M:addObstacle( obstacles )
    if self.simulator == nil then
        return -1
    end

    local rvoobstacles = core.RVOObstacle.new()
    for i,v in ipairs(obstacles) do
        rvoobstacles:push_back(Vector2.new(v.x, v.y))
    end
    
    return self.simulator:addRVOObstacle(rvoobstacles)
end

return M