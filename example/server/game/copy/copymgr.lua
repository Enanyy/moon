local moon = require("moon")

local M =
{
     copies = {}
}


function M:create(data, callback )
    
    moon.async(function()
        
        local copyid = moon.co_new_service("lua",
        {
            name = data.copyname,
            file = "copy.lua",
        })

        print("copymgr:create->begin init copy data")
        --设置副本数据
        local ret,err =  moon.co_call("lua", copyid, "INIT",  data)
        
        print("copymgr:create->init copy data return:",ret,err)

        local result = errordef.SUCCESS
        
        if type(ret)=="number" and ret == errordef.SUCCESS then
            table.insert( self.copies, {id = copyid, data = data} ) 
        else
            moon.remove_service(copyid, true)
            copyid = -1
            result = errordef.COPY_CREATE_ERROR
        end
        if callback then
            callback(result, copyid)
        end
   end)
end

function M:getcopy(copyid)
    for i,v in ipairs(self.copies) do
        if v.id == copyid then
            return v
        end
    end
    return nil
end

function M:remove(copyid)
    moon.async(function() 

        local ret =  moon.remove_service(copyid,true)
        table.removewhere(self.copies, function (data) return copyid == data.id end)
        print("copymgr:remove->",copyid)
    end)

end

function M:getcopy_by_userid(userid)

    for i,v in ipairs(self.copies) do
        for j, u in ipairs(v.data.users) do
            if u.userid == userid then
                return v
            end
        end
    end

    return nil
end


return M