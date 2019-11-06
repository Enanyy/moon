using System.Collections.Generic;

public class TBHero
{
//TABLE_DEFINITION_BEGIN
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

public class DTHero : IDataTable
{
    public DataTableID name
    {
        get { return DataTableID.TB_Hero; }
    }

    public readonly Dictionary<int, TBHero> data = new Dictionary<int, TBHero>();

    public void Read(SQLiteTable table)
    {
        while (table.Read())
        {
//TABLE_READ_BEGIN			TBHero o = new TBHero();
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
			data.Add(o.id,o);
//TABLE_READ_END
        }          
    }
    
    public static TBHero Get(int id)
    {
        var table = DataTableManager.Instance.Get<DTHero>(DataTableID.TB_Hero);
        if(table.data.ContainsKey(id))
        {
            return table.data[id];
        }
        return null;
    }
    
}