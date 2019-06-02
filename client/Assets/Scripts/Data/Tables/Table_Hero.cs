using System.Collections.Generic;

public class TB_Hero { 
	public int id;
	public string name;
	public int type;
	public int configid;
	public int hp;
	public int attack;
	public int defense;
	public int movespeed;
	public int direction;
	public int attackdistance;
	public int searchdistance;
	public float radius;
	public float height;
}

public class Table_Hero : IDataTable
{
    public DataTableID name
    {
        get { return DataTableID.TB_Hero; }
    }

    public readonly Dictionary<int, TB_Hero> data = new Dictionary<int, TB_Hero>();

    public void Read(SQLiteDataTable table)
    {
        while (table.Read())
        {
           	TB_Hero o = new TB_Hero();
			o.id = table.GetByColumnName("id",0);
			o.name = table.GetByColumnName("name","");
			o.type = table.GetByColumnName("type",0);
			o.configid = table.GetByColumnName("configid",0);
			o.hp = table.GetByColumnName("hp",0);
			o.attack = table.GetByColumnName("attack",0);
			o.defense = table.GetByColumnName("defense",0);
			o.movespeed = table.GetByColumnName("movespeed",0);
			o.direction = table.GetByColumnName("direction",0);
			o.attackdistance = table.GetByColumnName("attackdistance",0);
			o.searchdistance = table.GetByColumnName("searchdistance",0);
			o.radius = table.GetByColumnName("radius",0);
			o.height = table.GetByColumnName("height",0);
			data.Add(o.id,o);

        }          
    }
    
    public static TB_Hero Get(int id)
    {
        var table = DataTableManager.Instance.Get<Table_Hero>(DataTableID.TB_Hero);
        if(table.data.ContainsKey(id))
        {
            return table.data[id];
        }
        return null;
    }
    
}