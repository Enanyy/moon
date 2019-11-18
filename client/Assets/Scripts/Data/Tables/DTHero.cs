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

    /// <summary>
    /// 用于快速查找
    /// </summary>
    public readonly Dictionary<int, TBHero> dic = new Dictionary<int, TBHero>();
    /// <summary>
    /// 用于获取列表
    /// </summary>
    public readonly List<TBHero> list = new List<TBHero>();

    /// <summary>
    /// 注释XXXX_BEGIN和XXXX_END为替换区域，这些注释不能删除否则自动生成代码会失败，并且自定义内容不能写在注释之间，否则下次自动生成内容时会覆盖掉。
    /// </summary>

    public void Read(SQLiteTable table)
    {
        if (table == null)
        {
            return;
        }
        dic.Clear();
        list.Clear();
        while (table.Read())
        {
//TABLE_READ_BEGIN			TBHero o = new TBHero();
			o.id = table.GetByColumnName("id", 0);
			o.name = table.GetByColumnName("name", "");
			o.type = table.GetByColumnName("type", 0);
			o.config = table.GetByColumnName("config", "");
			o.hp = table.GetByColumnName("hp", 0);
			o.attack = table.GetByColumnName("attack", 0);
			o.defense = table.GetByColumnName("defense", 0);
			o.movedistance = table.GetByColumnName("movedistance", 0);
			o.movedirection = table.GetByColumnName("movedirection", 0);
			o.attackdistance = table.GetByColumnName("attackdistance", 0);
			o.searchdistance = table.GetByColumnName("searchdistance", 0);
			o.radius = table.GetByColumnName("radius", 0f);
			o.height = table.GetByColumnName("height", 0f);
			dic.Add(o.id,o);
			list.Add(o);
//TABLE_READ_END
        }          
    }
    private static DTHero mTable;
    private static DTHero table
    {
        get
        {
            if (mTable == null)
            {
                mTable = DataTableManager.Instance.Get<DTHero>(DataTableID.TB_Hero);
            }
            return mTable;
        }
    }
    public static TBHero Get(int id)
    {
       if (table != null && table.dic != null)
        {
            table.dic.TryGetValue(id, out TBHero data);
            return data;
        }
        return null;
    }

    public static TBHero Get(string name)
    {
        if (table != null && table.dic != null)
        {
            var it = table.dic.GetEnumerator();
            while (it.MoveNext())
            {
                if (it.Current.Value.name == name)
                {
                    return it.Current.Value;
                }
            }
        }
        return null;
    }

}