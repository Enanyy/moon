local M = {
    agents = {},
    removes = {}
}

function M:addagent(agent)
    if agent == nil then
        return
    end
    if agent:init() then
        table.insert( self.agents, agent)
        print("agents:"..#self.agents)
    end
end

function M:removeagent(id)

    table.insert( self.removes, id )
end

function M:getagent(id)
    for i,v in ipairs(self.agents) do
        if v.id == id then
            return v
        end
    end
    return nil
end

function M:getclosetagent(a)

    local distance = 0
    local agent = nil
    for i,v in ipairs(self.agents) do
        if v.camp ~= a.camp and a ~= v  then
            local d = vector2.distance(a.position, v.position)
            if agent == nil or d < distance then
                agent = v
                distance = d
            end
        end
    end
    return agent
end

function M:update(delta)
    for i,v in ipairs(self.agents) do
        v:update(delta)
    end

    for i,v in ipairs(self.removes) do
        table.removewhere(self.agents,function(data) return data.id == v end)    
    end
    self.removes = {}
end

function M:destroy()

    self.agents = {}
    
end

return M