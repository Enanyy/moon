local protobuf = require("protobuf")
local fs = require("fs")
local register_file      = protobuf.register_file

local M={}

function M.load(pbfile)
    local filepath = fs.working_directory()
    filepath = filepath.."/pb"..pbfile
    register_file(filepath)
end

function M.loadall()
    local curdir = fs.working_directory()
    fs.traverse_folder(curdir .. "/pb", 0, function(filepath, isdir)
        if isdir == false then
            if fs.extension(filepath) == ".pb" then
                --printf("LoadProtocol:%s", filepath)
                register_file(filepath)
            end
        end
        return true
    end)
end

return M
