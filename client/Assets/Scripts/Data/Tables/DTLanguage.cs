using System.Collections.Generic;
/// <summary>
/// 注释XXXX_BEGIN和XXXX_END为替换区域，这些注释不能删除否则自动生成代码会失败，并且自定义内容不能写在注释之间，否则下次自动生成内容时会覆盖掉。
/// </summary>
public class TBLanguage
{
//TABLE_DEFINITION_BEGIN
	public string key;
	public string ch;
	public string en;
//TABLE_DEFINITION_END
}

public class DTLanguage : IDataTable
{
    public DataTableID name
    {
        get { return DataTableID.TB_Language; }
    }
    /// <summary>
    /// 用于快速查找
    /// </summary>
    public readonly Dictionary<string, TBLanguage> dic = new Dictionary<string, TBLanguage>();
    /// <summary>
    /// 用于获取列表
    /// </summary>
    public readonly List<TBLanguage> list = new List<TBLanguage>();

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
//TABLE_READ_BEGIN
           	TBLanguage o = new TBLanguage();
			o.key = table.GetByColumnName("key","");
			o.ch = table.GetByColumnName("ch","");
			o.en = table.GetByColumnName("en","");
			dic.Add(o.key,o);
			list.Add(o);
//TABLE_READ_END
        }        
    }
    private static DTLanguage mTable;
    private static DTLanguage table
    {
        get
        {
            if(mTable == null)
            {
                mTable = DataTableManager.Instance.Get<DTLanguage>(DataTableID.TB_Language);
            }
            return mTable;
        }
    }
    
    public static TBLanguage Get(string key)
    {
        if (table != null && table.dic!= null)
        {
            table.dic.TryGetValue(key, out TBLanguage data);
            return data;
        }
        return null;
    }
    
}