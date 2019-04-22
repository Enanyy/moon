local moon = require("moon")
local user = require("logic.user")

local M = 
{
    users = {}
}

function  M:adduser(sessionid,id)
    local u = user.new(sessionid,id)
   
    u:oninit()

    self.users[u.id] = u

    return u
end

function M:removeuser(id)

    local u = self.users[id]
    if u ~= nil then
        u:ondestroy()
        table.removewhere(self.users, function(data) return data == u end)
    end

end

function M:removeuser_by_sessionid(sessionid)
    local u = self:getuser_by_sessionid(sessionid)
    if u ~= nil then
        u:ondestroy()
        table.removewhere(self.users, function(data) return data == u end)
    end
end

function M:getuser(id)
    if self.users[id] ~= nil then
        return self.users[id]
    end
    return nil
end

function M:getuser_by_sessionid(sessionid)
    for k,v in pairs(self.users) do
        if v.sessionid == sessionid then
            return v
        end
    end
    return nil
end

function M:broadcast(id, data, condition)

    for k,v in pairs(self.users) do
        if condition == nil or condition(v) == true then
            v:send(id, data)
        end
    end

end




--注册全局的usermgr
moon.exports.usermgr = M


