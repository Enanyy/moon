using UnityEngine;
using System;
using System.IO;

public class SQLite : IDisposable
{
	private static SQLite mInstance;
    private bool mIsDisposed = false;
    private SQLiteConnection mDbConnect = null;

	public static SQLite Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new SQLite();
            }

            return mInstance;
        }      
    }

	private SQLite()
    {

    }

	~SQLite()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
    }

    public void Close()
    {
        Dispose(true);
    }

    protected void Dispose(bool Diposing)
    {
        if (!mIsDisposed)
        {
            if (Diposing)
            {
                //Clean Up managed resources  
                if (mDbConnect != null)
                {
                    mDbConnect.Close();
                    mDbConnect = null;
                }
            }
            //Clean up unmanaged resources  
        }
        mIsDisposed = true;
    }

   
    public bool IsOpen()
    {

        if (mDbConnect != null)
        {
            return true;
        }

        return false;
    }


    /// <summary>
    /// 如果没有连接gamedb则连接gamedb.
    /// </summary>
    /// <param name="varDbFile">Variable db file.</param>
    public bool Open(byte[] bytes)
    {
        if (mDbConnect != null)
        {
            return true;
        }

        if (bytes == null)
        {
            return false;
        }
        MemoryStream ms = new MemoryStream(bytes);
        {
            try
            {
                // initialize database
                mDbConnect = new SQLiteConnection();
                mDbConnect.OpenStream("db", ms);
                Debug.Log("gamedb open sucess");

            }
            catch (Exception e)
            {
                ms.Dispose();
                Debug.Log("Open DB error==" + e.ToString());
            }
        }

        return true;
    }

    public bool Open(string fileName)
    {
        if (mDbConnect != null)
        {
            return true;
        }
        if (string.IsNullOrEmpty(fileName))
        {
            return false;
        }
        try
        {
            mDbConnect = new SQLiteConnection();

            mDbConnect.Open(fileName);
            Debug.Log("Open DB success!");
            return true;
        }
        catch (Exception e)
        {

            throw e;
        }
    }

    /// <summary>
    /// Gets the local item.
    /// </summary>
    /// <returns><c>true</c>, if local item was gotten, <c>false</c> otherwise.</returns>
    /// <param name="sql">Variable sql.</param>
    /// <param name="varOutItems">Variable out items.</param>
	public SQLiteTable GetTable(string sql)
    {
        SQLiteTable table = null;
        if (sql == string.Empty)
        {
            return table;
        }

        SQLiteQuery query = new SQLiteQuery(mDbConnect, sql);
        if (query != null)
        {
			table = new SQLiteTable(query);
            return table;
        }
        else
        {
            query.Release();
            return table;
        }
    }

    public bool Execute(string sql)
    {
        bool result = false;
        if (string.IsNullOrEmpty(sql))
        {
            return result;
        }

        SQLiteQuery query = new SQLiteQuery(mDbConnect, sql);
        if (query.Step() == false)
        {
            result = true;
        }
        query.Release();


        return result;
    }


    public SQLiteTable GetTables()
	{
		string sql = @"SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;";
		return GetTable (sql);
	}


	public SQLiteTable GetTableInfo(string tableName)
	{
		string sql = string.Format("pragma table_info([{0}]);", tableName);
		return GetTable (sql);
	}
}
/*
LocalConfig tempLocalConfig = LocalConfig.GetSingleton();


        bool b = tempLocalConfif(b == false)
        {
            Debuger.LogError("open local db failed");
        }
        else
        {
            Debuger.LogError("open local db success");
        }

        LocalItem tempItem = null;
//			m_LocalConfig.GetLocalItem("type_shop", 1001, out item);
        string tempSql = "select * from {0} where id = {1} ;";
        tempSql = Helper.Format(tempSql, "type_shop", 1001);
        tempLocalConfig.GetLocalItem(tempSql, out tempItem);

        if(b == false)
        {
            Debuger.Log("get table false");
            return ;
        }

        if(tempItem != null)
        {
            if (tempItem.Read())
            {
                string temname = tempItem.GetByColumnName("name", "unknow");

                Debuger.Log("column count:" + tempItem.pColumnCount);
                Debuger.Log(" pet name:" + temname);
            }

            tempItem.Close();
        }

        tempSql = "select * from type_shop ;";
        b = tempLocalConfig.GetLocalItem(tempSql, out tempItem);

        if(tempItem != null)
        {
            while(tempItem.Read())
            {
                int tempId = tempItem.GetByColumnName("id", -1);
                string tempName = tempItem.GetByColumnName("name", "unknow");

                Debuger.Log("id = " + tempId + " name = " + tempName);
            }

            tempItem.Close ();
        }

    }
}
*/
