--副本
local moon = require("moon")
local core = require("moon.core")

require("config")
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

moon.init(function(cfg) 
    config.load_config(cfg)
    
    return true
end)

moon.start(function ()   
    
    rvo2:init(delta/ 1000,vector2.new(0,0))
   
    for i=1,1 do
        local  a = agent.new()
        a.id = i
        a.config = 1
        a.camp = 1
       
        a.position = vector2.new(i*5, 0)
        a.direction = vector2.new(1, 0)

        a:addcomponent(ai.new())

        agentmgr:addagent(a)

    end

    for i=100,100 do
        local  a = agent.new()
        a.id = i
        a.config = 2
        a.camp = 2
       
        a.position = vector2.new(i/100*5, 20)
        a.direction = vector2.new(-1, 0)

        a:addcomponent(ai.new())
       
        agentmgr:addagent(a)

    end

   

end)

moon.dispatch('lua',function(msg, p) 


end)

moon.exit(function() 
    agentmgr:destroy()
end)

moon.destroy(function() 
    agentmgr:destroy()
end)


moon.repeated(delta, -1, function() 
    rvo2:doStep()
    agentmgr:update(delta / 1000)
end)