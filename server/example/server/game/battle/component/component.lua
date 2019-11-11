local M = {
    agent = nil,
    type = 0,
}
M.__index = M
function M.new()
    return setmetatable({}, M)
end

function M:start()
    
end

function M:update(delta)
   
end

function M:destroy()
end

function M:call(name, ...)
    local func = self[name]
    if func ~= nil then
        func(self, ...)
    end
end



return M