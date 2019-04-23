local moon = require("moon")

local M =
{
     copies = {}
}


function M:create(data, callback )
    
    moon.async(function()
        
        local copyid = moon.co_new_service("lua",
        {
                unique = false,
                name = data.copyname,
                file = "copy.lua",
        })

        table.insert( self.copies, {id = copyid, data = data} )
 
        if callback then
            callback(copyid)
        end
   end)
end

function M:get(copyid)
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
        
    end)

end


return M