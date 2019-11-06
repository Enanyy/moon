using System.Collections.Generic;
/// <summary>
/// 注释XXXX_BEGIN和XXXX_END为替换区域，这些注释不能删除否则自动生成代码会失败，并且自定义内容不能写在注释之间，否则下次自动生成内容时会覆盖掉。
/// </summary>
public class TBRole
{
//TABLE_DEFINITION_BEGIN
	public int id;
	public string name;
	public int type;
	public float height;
	public string desc;
	public double weigth;
	public int config;
//TABLE_DEFINITION_END

    public List<string> names = new List<string>();
}

public class DTRole : IDataTable
{
    public DataTableID name
    {
        get {{ return DataTableID.TB_Role; }}
    }

    public readonly Dictionary<int, TBRole> data = new Dictionary<int, TBRole>();

    public void Read(SQLiteTable table)
    {
        while (table.Read())
        {
//TABLE_READ_BEGIN			TBRole o = new TBRole();
			o.id = table.GetByColumnName("id",0);
			o.name = table.GetByColumnName("name","");
			o.type = table.GetByColumnName("type",0);
			o.height = table.GetByColumnName("height",0);
			o.desc = table.GetByColumnName("desc","");
			o.weigth = table.GetByColumnName("weigth",0);
			o.config = table.GetByColumnName("config",0);
			data.Add(o.id,o);
//TABLE_READ_END

            for (int i = 0; i < 10; ++i)
            {
                o.names.Add(o.name);
            }
        }        
    }
    
    public static TBRole Get(int id)
    {
        var table = DataTableManager.Instance.Get<DTRole>(DataTableID.TB_Role);
        if (table != null && table.data!= null)
        {
            table.data.TryGetValue(id, out TBRole data);
            return data;
        }
        return null;
    }
    
}