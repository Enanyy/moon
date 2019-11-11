local M = {
    agents = {},
    removes = {}
}

function M:addagent(agent)
    if agent == nil then
        return false
    end
    if agent:init() then
        local camp = agent.camp 
        if self.agents[camp] == nil then
            self.agents[camp] = {}
        end
        table.insert( self.agents[camp], agent)
        print("agentmgr:addagent->camp agents:", #self.agents[camp])
        return true
    end

    return false
end

function M:removeagent(id)

    table.insert( self.removes, id )
end

function M:getagent(id)
    for camp,list in pairs(self.agents) do
        for j, v in ipairs(list) do
            if v.id == id then
                return v
            end
        end     
    end
    return nil
end

function M:getclosetagent(a)

    local distance = 0
    local agent = nil
    for camp, list in pairs(self.agents) do
        for i, v in ipairs(list) do
            if v.isdie == false and v.camp ~= a.camp and a ~= v  then
                local d = vector2.distance(a.position, v.position)
                if agent == nil or d < distance then
                    agent = v
                    distance = d
                end
            end
        end
    end
    return agent
end

function M:update(delta)
    for camp, list in pairs(self.agents) do
        for i, v in ipairs(list) do
            v:update(delta)
        end
    end

    local check =false
    for i,v in ipairs(self.removes) do
        for camp, list in pairs(self.agents) do
            table.removewhere(list,function(data) return data.id == v end)  
            print("agentmgr:update->camp:",camp," count:",#list)
        end       
        check = true
    end
    if check then
        table.removewhere(self.agents, function(list) return #list == 0 end)
        
        print("agentmgr:update->camp count:",#self.agents)

        if #self.agents <= 1 then
            copy:destroy()
        end
    end
    self.removes = {}
end

function M:destroy()

    self.agents = {}
    
end

return M