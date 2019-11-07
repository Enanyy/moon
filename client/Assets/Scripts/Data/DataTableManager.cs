﻿using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 注释XXXX_BEGIN和XXXX_END为替换区域，这些注释不能删除否则自动生成代码会失败，并且自定义内容不能写在注释之间，否则下次自动生成内容时会覆盖掉。
/// </summary>
public enum DataTableID
{
//DATATABLE_ID_BEGIN	TB_Hero,
	TB_Role,
	TB_Language,
//DATATABLE_ID_END
}

public interface IDataTable
{
    DataTableID name { get; }

    void Read(SQLiteTable table);
}
public class DataTableManager 
{
    private DataTableManager() { }
    private static DataTableManager _instance;
    public static DataTableManager Instance
    {
        get
        {
            if (_instance==null)
            {
                _instance = new DataTableManager();
            }
            return _instance;
        }
    }

    private readonly Dictionary<DataTableID, IDataTable> mDataTables = new Dictionary<DataTableID, IDataTable>();

    /// <summary>
    /// 注释XXXX_BEGIN和XXXX_END为替换区域，这些注释不能删除否则自动生成代码会失败，并且自定义内容不能写在注释之间，否则下次自动生成内容时会覆盖掉。
    /// </summary>
    public bool Init(byte[] bytes)
    {
        if(bytes== null)
        {
            return false;
        }

        if(SQLite.Instance.Open(bytes))
        {
//DATATABLE_REGISTER_BEGIN			Register(new DTHero());
			Register(new DTRole());
			//Register(new DTLanguage());
//DATATABLE_REGISTER_END
            SQLite.Instance.Close();

            return true;
        }
        else
        {
            Debug.LogError("Open database Error!");

        }
        return false;
    }

    private void Register(IDataTable data)
    {
        if(data == null)
        {
            return;
        }

        if(mDataTables.ContainsKey(data.name)==false)
        {
            string sql = string.Format("select * from {0}", data.name.ToString());

            SQLiteTable table = SQLite.Instance.GetTable(sql);
            if(table!=null)
            {
                mDataTables.Add(data.name, data);
                data.Read(table);
                table.Close();
            }
            else
            {
                Debug.LogError("Can't find table:" + data.name);
            }
        }
    }
    public T Get<T>(DataTableID id) where T:class, IDataTable
    {
        mDataTables.TryGetValue(id, out IDataTable data);
        return data as T;
    }
}