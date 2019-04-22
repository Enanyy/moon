local sqrt = math.sqrt
local acos = math.acos
local max = math.max
local M = {
    x =0,
    y = 0,
}
M.__index = M
function M.new( x, y )
    return setmetatable({x = x or 0, y= y or 0}, M)
end


function M:sqrmagnitude()
	return self.x * self.x + self.y * self.y
end

function M:clone()
	return M.new(self.x, self.y)
end


function M:normalize()
	local x = self.x
	local y = self.y
	local magnitude = sqrt(x * x + y * y)

	if magnitude > 1e-05 then
		x = x / magnitude
		y = y / magnitude
    else
        x = 0
		y = 0
	end
    self.x = x
    self.y = y
end

function M:normalized()

    local x = self.x
	local y = self.y
	local magnitude = sqrt(x * x + y * y)

	if magnitude > 1e-05 then
		x = x / magnitude
		y = y / magnitude
    else
        x = 0
		y = 0
    end
    
    return setmetatable({x = x, y = y}, M) 
end

function M.dot(lhs, rhs)
    return lhs.x * rhs.x + lhs.y * rhs.y
end

function M.angle(from, to)
	local x1,y1 = from.x, from.y
	local d = sqrt(x1 * x1 + y1 * y1)

	if d > 1e-5 then
		x1 = x1/d
		y1 = y1/d
	else
		x1,y1 = 0,0
	end

	local x2,y2 = to.x, to.y
	d = sqrt(x2 * x2 + y2 * y2)

	if d > 1e-5 then
		x2 = x2/d
		y2 = y2/d
	else
		x2,y2 = 0,0
	end

	d = x1 * x2 + y1 * y2

	if d < -1 then
		d = -1
	elseif d > 1 then
		d = 1
	end

	return acos(d) * 57.29578
end

function M:magnitude()
	return sqrt(self.x * self.x + self.y * self.y)
end

function M.distance(a, b)
	return sqrt((a.x - b.x) ^ 2 + (a.y - b.y) ^ 2)
end

function M.lerp(a, b, t)
	if t < 0 then
		t = 0
	elseif t > 1 then
		t = 1
	end
    return M.new(a.x + (b.x - a.x) * t,  a.y + (b.y - a.y) * t)
end

function M.scale(a, b)
	return setmetatable({x = a.x * b.x, y = a.y * b.y}, M)
end

function M.movetowards(current, target, distance)
	local cx = current.x
	local cy = current.y
	local x = target.x - cx
	local y = target.y - cy
	local s = x * x + y * y

	if s  > distance * distance and s ~= 0 then
		s = distance / sqrt(s)
		return setmetatable({x = cx + x * s, y = cy + y * s}, M)
	end

    return setmetatable({x = target.x, y = target.y}, M)
end

function M.reflect(dir, normal)
	local dx = dir.x
	local dy = dir.y
	local nx = normal.x
	local ny = normal.y
	local s = -2 * (dx * nx + dy * ny)

	return setmetatable({x = s * nx + dx, y = s * ny + dy}, M)
end

M.__tostring = function(self)
	return string.format("(%f,%f)", self.x, self.y)
end

M.__div = function(va, d)
	return setmetatable({x = va.x / d, y = va.y / d}, M)
end

M.__mul = function(a, d)
	if type(d) == "number" then
		return setmetatable({x = a.x * d, y = a.y * d}, M)
	else
		return setmetatable({x = a * d.x, y = a * d.y}, M)
	end
end

M.__add = function(a, b)
	return setmetatable({x = a.x + b.x, y = a.y + b.y}, M)
end

M.__sub = function(a, b)
	return setmetatable({x = a.x - b.x, y = a.y - b.y}, M)
end

M.__unm = function(v)
	return setmetatable({x = -v.x, y = -v.y}, M)
end

M.__eq = function(a,b)
	return ((a.x - b.x) ^ 2 + (a.y - b.y) ^ 2) < 9.999999e-11
end

M.up 		= function() return setmetatable({x = 0, y = 1}, M) end
M.right	    = function() return setmetatable({x = 1, y = 0}, M) end
M.zero	    = function() return setmetatable({x = 0, y = 0}, M) end
M.one		= function() return setmetatable({x = 1, y = 1}, M) end

return M