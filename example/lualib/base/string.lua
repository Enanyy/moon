local string_find       = string.find
local string_sub        = string.sub
local string_gsub       = string.gsub
local string_len        = string.len
local string_upper      = string.upper
local table_insert      = table.insert

function string.split(input, delimiter)
    input = tostring(input)
    delimiter = tostring(delimiter)
    if (delimiter == "") then return false end
    local pos,arr = 1, {}
    for st, sp in function() return string_find(input, delimiter, pos, true) end do
        local str = string_sub(input, pos, st - 1)
        if str ~= "" then
            table_insert(arr, str)
        end
        pos = sp + 1
    end
    if pos <= string_len(input) then
        table_insert(arr, string_sub(input, pos))
    end
    return arr
end
---------------------------------------------------------
-- 分割字符串为数字,返回字符串table
---------------------------------------------------------
function string.splitnum(str, delim)
    local list = {}
    string.gsub(str, '[^'..delim..']+', function(w) table.insert(list, tonumber(w)) end)
    return list
end

local _TRIM_CHARS = " \t\n\r"

function string.ltrim(input, chars)
    chars = chars or _TRIM_CHARS
    local pattern = "^[" .. chars .. "]+"
    return string_gsub(input, pattern, "")
end

function string.rtrim(input, chars)
    chars = chars or _TRIM_CHARS
    local pattern = "[" .. chars .. "]+$"
    return string_gsub(input, pattern, "")
end

function string.trim(input, chars)
    chars = chars or _TRIM_CHARS
    local pattern = "^[" .. chars .. "]+"
    input = string_gsub(input, pattern, "")
    pattern = "[" .. chars .. "]+$"
    return string_gsub(input, pattern, "")
end

function string.ucfirst(input)
    return string_upper(string_sub(input, 1, 1)) .. string_sub(input, 2)
end
---传入时间戳 秒
function string.formattimestamp(t)
    return os.date("%Y/%m/%d %H:%M:%S",t)
end

---------------------------------------------------------
-- 将table转为字符串，打印调试用
---------------------------------------------------------
function t2string(table, t)

	---------------------------------------------------------
	-- 连接多个制表符(4个空格)
	---------------------------------------------------------
	function multipchar(t)
		local s = ''
		for i=1, t do
			s = s..'    '
		end
		return s
	end

	---------------------------------------------------------
	-- 在全局表中查找变量(table、function)的名字
	-- 此函数效率不咋滴，非调试情况下慎用
	---------------------------------------------------------
	function getname(value)
		for k, v in pairs(_G) do
			if v == value then
				return k
			end
		end
		return nil
	end

	t = t or 1
	if type(table) ~= 'table' then
		return 'this is not a table,'..type(table)..' it is.'
	end
	local s = '{\n'
	for k, v in pairs(table) do
		if k ~= '__index' and v ~= table then
			s = s..multipchar(t)
			s = s..'['
			if type(k) == 'string' then
				s = s..string.format("%q",k) 
			else
				s = s..tostring(k)
			end
			s = s..'] = '
			if type(v) == 'table' then
				s = s..t2string(v, t+1)
			elseif type(v) == 'string' then
				s = s..string.format("%q",v)
			else
				s = s..tostring(v)
			end
			s = s..',\n'
		end
	end

	local metatable = getmetatable(table)
	if metatable ~= nil and type(metatable) == 'table' and metatable ~= table then
		local parent = metatable.__index
		if parent ~= nil and type(parent) == 'table' and parent ~= table then
			s = s..multipchar(t)..'__index = '
			local parentname = getname(parent)
			if parentname ~= nil then
				s = s..parentname..':new'
			end
			s = s..t2string(parent, t+1).."\n"
		end
	end

	s = s..multipchar(t-1)
	s = s..'}'
	return s
end

string.tostring = t2string