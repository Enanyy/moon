local M = {}
M.__index = M
function M.new()
    return setmetatable({components = {}}, M)
end

function M:addcomponent(component)
    if component == nil then
        return
    end
    component.agent = self
    component:start()
    table.insert( self.components, component )
end
function M:removecomponent(type)
    table.removewhere(self.components,function(data)
        if data.type ==type then
            data:destroy()
            return true
        end
    end)
end

function M:update(delta)
    for i,v in ipairs(self.components) do
        v:update(delta)
    end
end

return M