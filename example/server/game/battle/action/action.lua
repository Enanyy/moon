require("game.battle.action.def")
local state = require("game.battle.state.state")

local M = setmetatable({}, {__index = state})
M.super = state
M.__index = M
function M.new()
    local o = setmetatable(state.new(), M)
    o.type = actiondef.idle
    return o
end


function M:enter()
    state.enter(self)
end

function M:execute(deltatime)
    state.execute(self,deltatime)
end

function M:exit()
    state.exit(self)
end


function M:pause()
    state.pause(self)
end

function M:resume()
    state.resume(self)
end

function M:cancel()
    state.cancel(self)
end

function M:destroy()
    state.destroy(self)
end


return M

