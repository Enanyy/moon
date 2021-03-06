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
    attackduration = 2,

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
        self.attackduration = cfg.attackduration
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

    hurtvalue = math.random(hurtvalue - 5, hurtvalue + 5)

    if hurtvalue < 1 then
        hurtvalue = 1
    end

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
    if self.sid >= 0 and self.isdie == false then
        local position = rvo2:getAgentPosition(self.sid)
        local distance = vector2.distance (position,self.position)
      
        local result = distance > 0.5
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
            -- if self.machine.current ~= nil and self.machine.c 3f urrent.type == actiondef.attack then
            --     local attack = self.machine.current
            --     if attack ~= nil then
            --         attack.duration = 0

            --         local data = 
            --         {
            --             id = self.id,
            --             copy = copy.copyid,
            --             duration = attack.duration,
            --             speed = attack.speed,
            --         }
            --         copy:broadcast(msgid.BATTLE_ENTITY_ATTACK_CHANGE_NOTIFY,data)
            --     end
            -- end

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
        position = {x = self.position.x, y = self.position.y},
        direction = {x = self.direction.x, y = self.direction.y},
        properties = self:properties()
    }

end

function M:properties()
    local data  = {
        {key = userpro.PRO_HP, value = self.hp,ratio = 1},
        {key = userpro.PRO_MAX_HP, value = self.hp,ratio = 1},
        {key = userpro.PRO_ATTACK, value = self.attackvalue,ratio = 1},
        {key = userpro.PRO_DEFENSE, value = self.defensevalue,ratio = 1},
        {key = userpro.PRO_MOVE_SPEED, value = self.movespeed * 100,ratio = 0.01},
        {key = userpro.PRO_ATTACK_DURATION, value = self.attackduration * 100,ratio = 0.01},
        {key = userpro.PRO_SEARCH_DISTANCE, value = self.searchdistance * 100,ratio = 0.01},
        {key = userpro.PRO_ATTACK_DISTANCE, value = self.attackdistance * 100,ratio = 0.01},
        {key = userpro.PRO_RADIUS, value = self.radius * 100,ratio = 0.01},
    }
    return data
end

return M