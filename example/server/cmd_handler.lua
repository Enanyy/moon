local M = {}
M.__index = M

function M.oncommand(self, msg, p)  
    local sender = msg:sender()
    local responseid = msg:responseid()
    M.docommand(self, sender,responseid,p.unpack(msg:bytes()))
end

function M.docommand(self, sender,responseid,CMD, ...)
    -- print("sender",sender)

    -- print("responseid",responseid)
    -- print("CMD",CMD)
    local func = self[CMD]
    if func ~= nil then
        func(sender,responseid,...)
    else
        print("can not find CMD:",CMD)
        for k,v in pairs(self) do
            print(k,v)
        end
    end
end
return M