local M = {}
M.__index = M

function M.oncommand(self, msg, p)
    
    local sender = msg:sender()
    local header = msg:header()
    local responseid = msg:responseid()
    M.docommand(self, sender, header,responseid,p.unpack(msg:bytes()))
end

function M.docommand(self, sender,header,responseid,CMD, ...)
    print("sender",sender)
    print("header",header)
    print("responseid",responseid)
    print("CMD",CMD)
    local func = self[CMD]
    if func ~= nil then
        func(sender,header,responseid,...)
    else
        print("can not find CMD:",CMD)
        for k,v in pairs(self) do
            print(k,v)
        end
    end
end
return M