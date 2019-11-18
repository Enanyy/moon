using System.Collections.Generic;
/// <summary>
/// 注释XXXX_BEGIN和XXXX_END为替换区域，这些注释不能删除否则自动生成代码会失败，并且自定义内容不能写在注释之间，否则下次自动生成内容时会覆盖掉。
/// </summary>
public class TBRole
{
//TABLE_DEFINITION_BEGIN	public int id;
	public string name;
	public int type;
	public float height;
	public string desc;
	public double weigth;
	public int config;
	public int y;
//TABLE_DEFINITION_END
}

public class DTRole : IDataTable
{
    public DataTableID name
    {
        get { return DataTableID.TB_Role; }
    }
    /// <summary>
    /// 用于快速查找
    /// </summary>
    public readonly Dictionary<int, TBRole> dic = new Dictionary<int, TBRole>();
    /// <summary>
    /// 用于获取列表
    /// </summary>
    public readonly List<TBRole> list = new List<TBRole>();

    /// <summary>
    /// 注释XXXX_BEGIN和XXXX_END为替换区域，这些注释不能删除否则自动生成代码会失败，并且自定义内容不能写在注释之间，否则下次自动生成内容时会覆盖掉。
    /// </summary>
    public void Read(SQLiteTable table)
    {
        if(table== null)
        {
            return;
        }
        dic.Clear();
        list.Clear();
        while (table.Read())
        {
//TABLE_READ_BEGIN			TBRole o = new TBRole();
			o.id = table.GetByColumnName("id", 0);
			o.name = table.GetByColumnName("name", "");
			o.type = table.GetByColumnName("type", 0);
			o.height = table.GetByColumnName("height", 0f);
			o.desc = table.GetByColumnName("desc", "");
			o.weigth = table.GetByColumnName("weigth", 0f);
			o.config = table.GetByColumnName("config", 0);
			o.y = table.GetByColumnName("y", 0);
			dic.Add(o.id,o);
			list.Add(o);
//TABLE_READ_END
        }        
    }
    private static DTRole mTable;
    private static DTRole table
    {
        get
        {
            if(mTable == null)
            {
                mTable = DataTableManager.Instance.Get<DTRole>(DataTableID.TB_Role);
            }
            return mTable;
        }
    }
    
    public static TBRole Get(int key)
    {
        if (table != null && table.dic!= null)
        {
            table.dic.TryGetValue(key, out TBRole data);
            return data;
        }
        return null;
    }
    
}