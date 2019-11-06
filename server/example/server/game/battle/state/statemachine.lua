local queue = require("base.queue")
local M ={}

M.__index = M

function M.new( agent)
    local o = {
        previous = nil,
        current = nil,
        agent = agent,
        states = queue.new(),
        ispause = false
    }
    return setmetatable(o, M)
end

function M:setpause(pause)
    
    self.ispause = pause

    if self.current then
        self.current:setpause(pause)
    end
end


function M:addfirst( state )
    if state == nil then
        return
    end
    state:setmachine(self)
    if self.states:count() < 2 then
        self.states:enqueue(state)
    else
        table.insert( self.states.items, 2, state )
    end
    self:checknext()
end

function M:addlast( state )
    if state == nil then
        return 
    end
    state:setmachine(self)
    self.states:enqueue(state)
    self:checknext()
end

function M:checknext()
    if self.states:count() > 1 and self.states.items[2].weight > self.states.items[1].weight then
        if self.current.isdone ==false then
            self.current:done()
        end
    end
end

function M:getfirst(type)

    for i,v in ipairs(self.states.items) do
        if v.type == type then
            return v
        end
    end
    return nil
end

function M:getlast( type )
    
    for i = self.states:count(), 1, -1 do
        if self.states.items[i].type == type then
            return self.states.items[i]
        end
    end

    return nil
end

function M:donext()
    if self.current ~= nil then
        if self.current.iscancel then
            self.current:cancel()
        end
        self.current:exit()
        self.states:dequeue()
    end
    if self.previous ~= nil then
        self.previous:destroy()
    end
    self.previous = self.current

    self.current = nil

    while self.states:count() > 0 do
        if self.states:peek():isvalid() == false then
           local state = self.states:dequeue()
           state:destroy()
        else    
            break
        end
    end

    if self.states:count() > 0 then
       
        self.current = self.states:peek()
        self.current:enter()
    end
end

function M:update(delta)
   
    if self.ispause then
        return
    end
 
    if self.current == nil
     or self.current.isdone 
     or (self.states:count() > 1 and self.states.items[2].weight > self.current.weight) then
        self:donext()
    end
    if self.current ~= nil then
        self.current:execute(delta)
    end
end

function M:clear(  )
    -- body
end


return M
