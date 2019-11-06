
local math_floor = math.floor

function checknumber(value, base)
    return tonumber(value, base) or 0
end

function checkint(value)
    value = tonumber(value) or 0
    return math_floor(value + 0.5)
end

function checkbool(value)
    return (value ~= nil and value ~= false)
end

function checktable(value)
    if type(value) ~= "table" then value = {} end
    return value
end

function unused()
    -- body
end

function get_script_path()
    local info = debug.getinfo(2, "S")
    local path = info.source
    path = string.sub(path, 2, -1)
    return string.match(path, "^.*[/\\]")
end

--打印堆栈
function printtrace(...) 
    local out = {'[TRACE]'} 
    local n = select('#', ...) 
    for i=1, n, 1 do 
        local v = select(i,...) 
        out[#out+1] = tostring(v) 
    end 
    out[#out+1] = '\n' 
    out[#out+1] = debug.traceback ("", 2) 
    print(table.concat(out,' ')) 
end 
