local moon = require("moon")
local cluster = require("moon.cluster")
local M = {}

function  M.call(name, cmd, ...)
    local config = config.get_service(name)
    if config then 
        local data = { cluster.call(string.format( "server_%d", config.sid),name,cmd, ...) }
        return table.unpack(data)
    else
        error("server.call: can not service ",name)
    end
    return nil
end
--注册全局的server
moon.exports.server = M

