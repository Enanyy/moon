--user 基类
local M = {}
M.__index = M


function M.new(sessionid,id)
    local tb = {}
    tb.sessionid = sessionid
    tb.id = id --数据库id
    return setmetatable(tb,M)
end

function M:oninit()


end

function M:ondestroy()

end

function M:onlogin()

end

function M:onlogout()

end

function M:send(id,data)
    netmgr:send(self.sessionid, id,data)
end


return M


