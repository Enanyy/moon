local moon = require("moon")

local msgid = 
{
    LOGIN_REQUEST = 1000,
    LOGIN_RETURN = 1001,
    LOGIN_GAME_NOTIFY = 1002,
    LOGIN_GAME_REQUEST = 1003,
    LOGIN_GAME_RETURN = 1004,
}
--注册全局表
moon.exports.msgid = msgid
