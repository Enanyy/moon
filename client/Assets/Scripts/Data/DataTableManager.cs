using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ע��XXXX_BEGIN��XXXX_ENDΪ�滻������Щע�Ͳ���ɾ�������Զ����ɴ����ʧ�ܣ������Զ������ݲ���д��ע��֮�䣬�����´��Զ���������ʱ�Ḳ�ǵ���
/// </summary>
public enum DataTableID
{
//DATATABLE_ID_BEGIN	TB_Hero,
	TB_Role,
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
        if(mDataTables.ContainsKey(id))
        {
            return mDataTables[id] as T;
        }
        return null;
    }
}