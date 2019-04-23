local protobuf = require("protobuf")
local protoloader = require("protoloader")
local encode = protobuf.encode
local decode = protobuf.decode
local pack = string.pack
local unpack = string.unpack

require("msgid")

local M = {
    msg_def = {}
}

M.__index = M

function M.init()
    protoloader.loadall()
    M.msg_def[msgid.LOGIN_REQUEST]           = {name = "LoginRequest",func = nil}
    M.msg_def[msgid.LOGIN_RETURN]            = {name = "LoginReturn",func = nil}
    M.msg_def[msgid.LOGIN_GAME_NOTIFY]       = {name = "LoginGameNotify",func = nil}
    M.msg_def[msgid.LOGIN_GAME_REQUEST]      = {name = "LoginGameRequest",func = nil}
    M.msg_def[msgid.LOGIN_GAME_RETURN]       = {name = "LoginGameReturn",func = nil}

    M.msg_def[msgid.BATTLE_BEGIN_REQUEST]    = {name = "BattleBeginRequest",func = nil}
    M.msg_def[msgid.BATTLE_BEGIN_RETURN]     = {name = "BattleBeginReturn",func = nil}
    M.msg_def[msgid.BATTLE_BEGIN_NOTIFY]     = {name = "BattleBeginNotify",func = nil}
    M.msg_def[msgid.BATTLE_ENTITY_IDLE]      = {name = "BattleEntityIdle",func = nil}
    M.msg_def[msgid.BATTLE_ENTITY_RUN]       = {name = "BattleEntityRun",func = nil}
    M.msg_def[msgid.BATTLE_ENTITY_ATTACK]    = {name = "BattleEntityAttack",func = nil}
    M.msg_def[msgid.BATTLE_ENTITY_DIE]       = {name = "BattleEntityDie",func = nil}
    M.msg_def[msgid.BATTLE_END_NOTIFY]       = {name = "BattleEndNotify",func = nil}

end

function M.register(id, func )
    local def =  M.msg_def[id] 
    if def then
        def.func = func
    else
        error("Can not find msgid:"..tostring(id))
    end
end

function M.get_def( id )
    return M.msg_def[id]
end

function M.onaccept( sessionid )

end

function M.onclose( sessionid )
   
end

function M.onerror( sessionid )
   
end

function M.ondispatch(sessionid, id, msg )
    
end

function M.encode(id, data)

    local def = M.get_def(id) 

    if def == nil then
        error("can not find msg def:"..tostring(id))
        return nil
    end

    local pbdata = encode(def.name,data)
    --id小端编码 4个字节,有符号
    local buffer = pack("<i", id)..pbdata

    return buffer
end

function M.decode(buffer)

    local size = #buffer
    if size >= 4 then
        --id 4个字节
        local bytes = string.sub(buffer, 1, 5 )
        local pbdata = string.sub(buffer, 5 )

        --id小端编码，有符号
        local id = unpack("<i",bytes)
        
        local def = M.get_def(id)
        if def == nil then
            error("can not find msg def:"..tostring(id))
            return nil
         end

        local data = decode(def.name, pbdata)
        return {id = id, msg = data}
        
    end
    return nil
end


return M
