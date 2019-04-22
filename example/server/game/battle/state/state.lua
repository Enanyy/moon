local M = {}
M.__index = M

function M.new()
    local o = {
        machine = nil,
        type = 0,
        agent = nil,
        weight = 0,
        duration = 0,
        time = 0,
        speed = 1,
        isdone =false,
        iscancel = false,
        ispause = false,
    }
    return setmetatable(o, M)
end
function M:setmachine(machine)
    self.machine = machine
    self.agent = machine.agent
end
function M:setpause(pause)
    if self.ispause and pause ==false then
        self:resume()
    elseif self.ispause ==false and pause then
        self:pause()
    end

    self.ispause = pause
end

function M:done()
    self.isdone = true
    self.iscancel = self.time < self.duration
end
function M:isvalid()
    return true
end

function M:call(name,...)

    if self.agent ~= nil then
        local func = self.agent[name]
        if func ~= nil then
            func(self.agent, self, ...)
        end
    end
end

function M:enter()
   
    self:call("enter")
end

function M:execute(delta)

    if self.time < self.duration then
        self.time = self.time + delta * self.speed
        if self.time >= self.duration then
            self:done()
        end
    end

    self:call("execute",delta)

end

function M:exit()
   self:call("exit")
end


function M:pause()
    self:call("pause")
end

function M:resume()
    self:call("resume")
end

function M:cancel()
    self:call("cancel")
end

function M:destroy()
    self:call("destroy")
end

return M
