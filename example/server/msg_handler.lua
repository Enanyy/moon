local M = {msg_def = {}}

M.__index = M

function M.init()

   
end

function M.register(id, def )
    M.msg_def[id] = def
end

function M.get_def( id )
    return M.msg_def[id]
end

function M.onaccept( sessionid )

end

function M.onclose( sessionid )
   
end

function M.onerror( sessionid )
   
end

function M.ondispatch(sessionid, id, msg )
    
end



return M
