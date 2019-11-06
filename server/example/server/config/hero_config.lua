local M = {
    data = {
        [1]= {id = 1, name = "hero1", hp = 100,movespeed = 6,attackduration = 2, searchdistance = 50,attackdistance = 20,attackvalue = 5,defensevalue = 2,radius = 0.5},
        [2]= {id = 2, name = "hero2", hp = 200,movespeed = 6,attackduration = 2, searchdistance = 50,attackdistance = 8,attackvalue = 10,defensevalue = 2,radius = 1.2},
        [3]= {id = 3, name = "hero3", hp = 300,movespeed = 6,attackduration = 2, searchdistance = 50,attackdistance = 4,attackvalue = 20,defensevalue = 2,radius = 1},
        [4]= {id = 4, name = "hero4", hp = 400,movespeed = 6,attackduration = 2, searchdistance = 50,attackdistance = 20,attackvalue = 30,defensevalue = 2,radius = 0.5},
    }
}
function M.get( id )
    return M.data[id]
end

function M.count()
    return #M.data
end

return M