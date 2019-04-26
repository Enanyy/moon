local statemachine = require("game.battle.state.statemachine")
local components = require("game.battle.component.components")
local hero_config = require("config.hero_config")
local M = setmetatable({
    id = 0,
    userid = 0,
    sid = -1,
    camp = 0,
    name = "",
    type = 0,
    config = 0,
    hp = 0,

    movespeed = 5,
    attackspeed = 2,

    searchdistance = 20,
    attackdistance = 2,
    radius = 1,

    attackvalue = 5, --攻击力
    defensevalue = 2, --防御力

    position = vector2.new(),
    direction = vector2.new(),

    isdie = false,
    target = nil,

    machine = nil,
    ispause = false,
}, {__index = components})

M.__index = M

function M.new()
    local o = setmetatable(components.new(), M)

    return o
end

function M:init()

    local cfg = hero_config.get(self.config)
    if cfg == nil then
        return false
    end
        self.name = cfg.name
        self.hp = cfg.hp
        self.movespeed = cfg.movespeed
        self.attackspeed = cfg.attackspeed
        self.searchdistance = cfg.searchdistance
        self.attackdistance = cfg.attackdistance
        self.radius = cfg.radius
        self.attackvalue = cfg.attackvalue
        self.defensevalue = cfg.defensevalue
    
    self.sid = rvo2:addAgent(self.position, self.radius)
    
    if self.sid < 0 then
        return false
    end

    self.machine = statemachine.new(self)
    

    return true
end

function M:setpause( pasue )
    self.ispause = pasue
    if  self.machine then
        self.machine:setpause(pasue)
    end
end

function M:play(action, first)
    if action == nil  then
        return
    end

    if self.machine ~= nil then
        if first then
            self.machine:addfirst(action)
        else
            self.machine:addlast(action)
        end
    end
end

function M:getfirst(type)
    if self.machine  then
        return self.machine:getfirst(type)
    end

    return nil
end

function M:getlast(type)
    if self.machine then
        return self.machine:getlast(type(v))
    end
    return nil
end

function M:update(delta)
    if self.ispause  then
        return 
    end

    components.update(self,delta)

    if self.machine then
        if self.machine.current  == nil then
            local idle = actionidle.new()
            self:play(idle)
        end
        self.machine:update(delta)

        self:checksync()
    end
end

function M:call( name, ... )
    for i,v in ipairs(self.components) do
        v:call(name, ...)
    end
end

function M:enter( action )
   self:call("enter",action)
end

function M:execute(action, delta)

    self:call("execute",action,delta)

end

function M:exit( action )
   
    self:call("exit",action)
    
end

function M:pasue( action )
  
    self:call("pasue",action)
    
end

function M:resume(action )
  
    self:call("resume",action)
    
end

function M:cancel(action)

    self:call("cancel",action)
    
end

function M:destroy(action )

    self:call("destroy",action)

end



--受伤
function M:hurtby( attacker )
    if attacker == nil then
        return
    end

    local hurtvalue = (attacker.attackvalue - self.defensevalue)
    self.hp = self.hp - hurtvalue

    --print(attacker.id, " attack ",self.id, " hurt:",hurtvalue," hp:",self.hp)

    local data = {
        id = self.id,
        copy = copy.copyid,
        hp = self.hp,
        value = hurtvalue
    }

    copy:broadcast(msgid.BATTLE_ENTITY_BLOOD_NOTIFY,data)

    if self.hp <= 0 then
        self.isdie = true
        rvo2:delAgent(self.sid)
        local die = actiondie.new()
        self:play(die)
    end
end

function M:needsync( )
    if self.sid >= 0 then
        local position = rvo2:getAgentPosition(self.sid)
        
        local result = (position-self.position):sqrmagnitude() > 0.1
        return result
    end

    return false
end


function M:checksync()

    if self.machine  == nil then
        return
    end
    if self:needsync() then
        local position = rvo2:getAgentPosition(self.sid)
        local run = nil 
        if self.machine.current ~= nil and self.machine.current.type == actiondef.run then
            run = self.machine.current
        end

        if run == nil then
            if self.machine.states:count() > 1 and self.machine.states.items[2].type == actiondef.run then
                run =  self.machine.states.items[2]
            end

        end

        if run ~= nil then
            run:setdestination(position)
        else
            run = actionrun.new()
            run:setdestination(position)
            self:play(run, true)
        end

    end
end

function M:get_send_agent()

    return {
        id = self.id,
        userid = self.userid,
        camp = self.camp,
        name = self.name,
        type = 1,
        config = self.config,
        searchdistance = self.searchdistance,
        attackdistance = self.attackdistance,
        radius = self.radius,
        data = self:get_send_data()
    }

end

function M:get_send_data()
    return {
        hp = self.hp,
        maxhp = self.maxhp,
        attack = self.attackvalue,
        defense = self.defensevalue,
        movespeed = self.movespeed,
        attackspeed = self.attackspeed,
        position = {x = self.position.x, y =self.position.y},
        direction = {x = self.direction.x, y =self.direction.y}
    }
end

return M