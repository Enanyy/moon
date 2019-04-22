local moon = require("moon")

local M =
{
     copies = {}
}


function M:create(name, callback )
    
    moon.async(function()
        local copy_name = "copy_"..name
        local serviceid = moon.co_new_service("lua",
        {
                unique = false,
                name = copy_name,
                file = "copy.lua",
                id = i
        })
        
        table.insert( self.copies, {id = serviceid, name = copy_name} )

        if callback then
            callback(serviceid)
        end
   end)
end

function M:remove(serviceid)
    moon.async(function() 

        local ret =  moon.remove_service(serviceid,true)
        table.removewhere(self.copies, function (data) return serviceid == data.id end)
        
    end)

end


return M