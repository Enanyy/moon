local moon = require("moon")
local json = require("json")

local M = 
{
    current = nil, --当前服务的配置
    clusters = {}, --所有配置集群
    services = {}, --所有配置服务
}

function M.load_config(cfg)

    M.current = cfg

    local content = moon.get_env("server_config")
    local js = json.decode(content)
    for _,server in pairs(js) do
        for _, service in pairs(server.services) do
            if service.name == "clusterd" then
                local network =  { ip= service.network.ip, port = service.network.port}
                M.clusters[service.name] = {sid = server.sid, network = network} 
            else
                local network = nil
                if service.network ~= nil then
                    network = { ip= service.network.ip, port = service.network.port }
                end
                local mysql = nil
                if service.mysql ~= nil then
                     mysql = {
                        sid = server.sid,
                        ip = service.mysql.ip,
                        port = tonumber(service.mysql.port),
                        database = service.mysql.database,
                        user = service.mysql.user,
                        password = service.mysql.password,
                        timeout = tonumber(service.mysql.timeout)
                    }                  
                end
              
                M.services[service.name] = {sid = server.sid, network = network, mysql = mysql } 
               
            end
        end
    end
end

function M.get_cluster(name)
    return M.clusters[name]
end

function M.get_service(name)
    return M.services[name]
end


--注册全局表
moon.exports.config = M