--副本
local moon = require("moon")
local core = require("moon.core")
local cmd = require("game.copy.copy_cmd")
require("config")
require("def")
require("msgid")
moon.exports.vector2 = require("base.vector2")
moon.exports.agent = require("game.battle.agent")
moon.exports.agentmgr = require("game.battle.agentmgr")

moon.exports.actiondef = require("game.battle.action.def")
moon.exports.actionidle = require("game.battle.action.actionidle")
moon.exports.actionrun = require("game.battle.action.actionrun")
moon.exports.actionattack = require("game.battle.action.actionattack")
moon.exports.actiondie = require("game.battle.action.actiondie")

moon.exports.componentdef = require("game.battle.component.def")
moon.exports.ai  = require("game.battle.component.ai")

moon.exports.rvo2 = require("game.battle.rvo2")

local delta = 100


local M = {
    sid = -1,
    copyname = "",
    users = nil,
    isdestroy = false,
}
function M:init(data)
    print("copy:init-> copy",table.tostring(data))
    self.sid = data.sid
    self.copyid = moon.sid()
    self.copyname = data.copyname
    self.users = data.users
    self.isdestroy = false
end
function M:login(userid,sessionid)

    local result = false

    if self.users ~= nil then
        for i,v in ipairs(self.users) do
            if v.userid == userid then
                v.sessionid = sessionid
                result = true
            end
        end
    end

    if result then 
        print("copy:login->copy ", self.copyname, " start")
        self:start()
    end

    return result
    
end

function M:getuser(userid)
    if self.users ~= nil then
        for i,v in ipairs(self.users) do
            if v.userid == userid then
               return v
            end
        end
    end
    return nil
end


function M:send(userid, id, data)
    if self.sid > 0  then
        local user = self:getuser(userid)
        if user and user.sessionid then
            moon.async(function ()
                local result,err = moon.co_call("lua", self.sid, "SEND", user.sessionid, id, data)            
            end)
        end
    end
end

function M:start()
    rvo2:init(delta/ 1000,vector2.new(0,0))
   
    local data = {
        copy = self.copyid,
        list = {}
    }

    for i,user in ipairs(self.users) do
        local index = 1
        for j, hero in ipairs(user.heros) do
           for k = 1,hero.count do
                local  a = agent.new()
                a.id = user.userid * 100 + index
                a.userid = user.userid
                a.config = hero.config
                a.camp = user.camp
                a.type = user.type
            
                a.position = vector2.new(k*5, (i-1)*100 +j*5)
                a.direction = vector2.new(1, 0)
        
                a:addcomponent(ai.new())
        
                if agentmgr:addagent(a) then
                    table.insert( data.list, a:get_send_agent() )
                end


                index = index + 1
           end
        end
    end


    self:broadcast(msgid.BATTLE_BEGIN_NOTIFY,data)

end

function M:broadcast(id, data)
    for i,user in ipairs(self.users) do
        if user.type == userdef.USER then
            self:send(user.userid,id,data)
        end
    end
    
end

function M:destroy()
    if self.isdestroy ==true then
        return
    end
    agentmgr:destroy()
    local data = {
        copy = self.copyid
    }
    self:broadcast(msgid.BATTLE_END_NOTIFY,data)
    self.isdestroy = true
    moon.async(function ()
        moon.co_call("lua", self.sid, "DESTROY_COPY", self.copyid)            
    end)
end

function M:update(delta)
    if self.isdestroy then
        return
    end
    rvo2:doStep()
    agentmgr:update(delta )
end

moon.exports.copy = M


moon.init(function(cfg) 
    config.load_config(cfg)
    
    return true
end)

moon.start(function ()   
    
end)

moon.dispatch('lua',function(msg, p) 
    cmd.oncommand(cmd,msg,p)
end)

moon.exit(function() 
    copy:destroy()
end)

moon.destroy(function() 
    copy:destroy()
end)


moon.repeated(delta, -1, function() 
   copy:update(delta / 1000)
end)