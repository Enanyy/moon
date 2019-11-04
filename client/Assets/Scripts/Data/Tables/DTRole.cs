using System.Collections.Generic;

public class TBRole
{
//DEFINITION_START
	public int id;
	public string name;
	public int type;
	public string config;
	public int hp;
	public int attack;
	public int defense;
	public int movedistance;
	public int movedirection;
	public int attackdistance;
	public int searchdistance;
	public float radius;
	public float height;
//DEFINITION_END
}

public class DTRole : IDataTable
{
    public DataTableID name
    {
        get { return DataTableID.TB_Role; }
    }

    //public readonly Dictionary<int, TBRole> data = new Dictionary<int, TBRole>();

    public void Read(SQLiteTable table)
    {
        while (table.Read())
        {
//READ_START
           /*	TBRole o = new TBRole();
			o.id = table.GetByColumnName("id",0);
			o.name = table.GetByColumnName("name","");
			o.type = table.GetByColumnName("type",0);
			o.config = table.GetByColumnName("config","");
			o.hp = table.GetByColumnName("hp",0);
			o.attack = table.GetByColumnName("attack",0);
			o.defense = table.GetByColumnName("defense",0);
			o.movedistance = table.GetByColumnName("movedistance",0);
			o.movedirection = table.GetByColumnName("movedirection",0);
			o.attackdistance = table.GetByColumnName("attackdistance",0);
			o.searchdistance = table.GetByColumnName("searchdistance",0);
			o.radius = table.GetByColumnName("radius",0);
			o.height = table.GetByColumnName("height",0);
			data.Add(o.id,o);*/
//READ_END
        }          
    }
    /*
    public static TBRole Get(int id)
    {
        var table = DataTableManager.Instance.Get<DTRole>(DataTableID.TB_Role);
        if(table.data.ContainsKey(id))
        {
            return table.data[id];
        }
        return null;
    }
    */
}