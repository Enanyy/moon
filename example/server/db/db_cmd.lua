local moon = require("moon")

local cmd_handler = require("cmd_handler")
local M = setmetatable({
    database = nil,
}, cmd_handler)

M.init = function(conn)
    M.conn = conn
    if M.isconnected() then

        local db_config = config.get_service("db")
        if db_config == nil or db_config.mysql == nil then
            return
        end
        M.database = db_config.mysql.database
        local  user_sql = string.format([[CREATE TABLE IF NOT EXISTS `%s`.`user` (
            `id` INT NOT NULL AUTO_INCREMENT,
            `name` VARCHAR(16) NOT NULL,
            `password` VARCHAR(45) NOT NULL,
            PRIMARY KEY (`id`),
            UNIQUE INDEX `id_UNIQUE` (`id` ASC) VISIBLE,
            UNIQUE INDEX `name_UNIQUE` (`name` ASC) VISIBLE)
            ENGINE = InnoDB
            DEFAULT CHARACTER SET = utf8;]],M.database)
        
        
        M.conn:execute(user_sql)
        
    end
end

M.isconnected = function() return M.conn ~= nil and M.conn:connected() end

M.LOGIN = function (sender,responseid, msg )

    local id = errordef.SYSTEM
    if M.isconnected() then
       
        local sql = string.format( "select * from %s.user where name = '%s';",M.database,msg.name)
        print("db_cmd.LOGIN->",sql)
        local result = M.conn:query(sql)

        print("db_cmd.LOGIN->select result:",table.tostring(result))
        if type(result) =="table" and #result > 0 then
            local row = result[1]
            print("db_cmd.LOGIN->select row:",table.tostring(row))

            if row[2] == msg.name and row[3] == msg.password then
                id = row[1]
            else
                id = errordef.LOGIN_PASSWORD_ERROR
            end
        else
            local sql = string.format("insert into %s.user (name,password) values('%s', '%s');",M.database,msg.name, msg.password)
            local result = M.conn:execute(sql)

            print("db_cmd.LOGIN->register user result:"..table.tostring(result))
            if result then
                local data = M.conn:query("select @@IDENTITY;")
                id = data[1][1]             
            end
        end

    end
    local data = {
        id = id,
        name = msg.name,
        password = msg.password
    }
    print("db_cmd.LOGIN->db login return:",table.tostring(data))
    moon.response('lua',sender,responseid, data)

end

return M