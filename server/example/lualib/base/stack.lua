local M = {}
M.__index = M

function M.new(...)
    local o ={}
    o.items = {...}

    return setmetatable(o, M)
end

function M:peek()
    return self.items[1]
end

function M:push(item)
    table.insert(self.items, 1, item)
end

function M:pop()
    local item = self:peek()
	table.remove(self.items, 1)
	return item
end

function M:count()
    return #self.items
end

function M:clear()
    self.items = {}
end

return M