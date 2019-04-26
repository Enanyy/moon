--全局定义
local moon = require("moon")
--错误定义
local errorid = 
{
    SUCCESS = 0, --成功
    SYSTEM = -1, --系统错误

    LOGIN_USERNAME_ERROR = -100, --登陆用户名错误
    LOGIN_PASSWORD_ERROR = -101, --登陆密码错误

    COPY_CREATE_ERROR = -102,
    COPY_INIT_ERROR = -103,

    LOGIN_COPY_ERROR = -104,
}


moon.exports.errordef = errorid
--错误描述
local errordes =
{
    [errorid.SUCCESS] = "成功",
    [errorid.SYSTEM] = "系统错误",
    [errorid.LOGIN_USERNAME_ERROR] = "登陆用户名错误",
    [errorid.LOGIN_PASSWORD_ERROR] = "登陆密码错误",
    [errorid.COPY_CREATE_ERROR] = "创建副本错误",
    [errorid.COPY_INIT_ERROR] = "初始化副本错误",
    [errorid.LOGIN_COPY_ERROR] = "登陆副本错误",
}

moon.exports.errordes = errordes

local server=
{
    LOGIC = "logic", --逻辑服务器
    GAME = "game",   --游戏服务器
    DB = "db",       --数据服务器
}
moon.exports.serverdef = server

--玩家类型
local userdef ={
    USER = 1,
    MONSTER = 2,
}

moon.exports.userdef = userdef

--协议属性类型
local userpro = {

    HP = 1,
    MAX_HP = 2,
    ATTACK = 3,
    DEFENSE = 4,
    MOVE_SPEED = 5,
    ATTACK_SPEED = 6,
    SEARCH_DISTANCE = 7,
    ATTACK_DISTANCE = 8,
    RADIUS = 9,
}

moon.exports.userpro = userpro