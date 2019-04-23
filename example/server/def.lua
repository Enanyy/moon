--全局定义
local moon = require("moon")
--错误定义
local error = 
{
    SUCCESS = 0, --成功
    SYSTEM = -1, --系统错误

    LOGIN_USERNAME_ERROR = -100, --登陆用户名错误
    LOGIN_PASSWORD_ERROR = -101, --登陆密码错误


}


moon.exports.errordef = error

local server=
{
    LOGIC = "logic", --逻辑服务器
    GAME = "game",   --游戏服务器
    DB = "db",       --数据服务器
}
moon.exports.serverdef = server