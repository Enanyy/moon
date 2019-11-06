using UnityEngine;
using System;
using System.IO;
using Community.CsharpSqlite;

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
        if (string.IsNullOrEmpty(sql) || IsOpen() == false)
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
    /// <summary>
    /// 执行一条sql语句或事务
    /// </summary>
    /// <param name="sql"></param>
    /// <returns></returns>
    public int Execute(string sql)
    {
        int result = 0;
        if (string.IsNullOrEmpty(sql) || IsOpen() == false)
        {
            return result;
        }

        string error = null;

        result = Sqlite3.sqlite3_exec(mDbConnect.Connection(), sql, null ,null, ref error);

        if (string.IsNullOrEmpty(error) == false)
        {
            Debug.LogError(error);
        }
        return result;
    }

    /// <summary>
    /// 获取数据库所以数据表名字
    /// </summary>
    /// <returns></returns>
    public SQLiteTable GetTables()
	{
		string sql = @"SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;";
		return GetTable (sql);
	}

    /// <summary>
    /// 获取某个数据表信息（字段名和字段类型）
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
	public SQLiteTable GetTableInfo(string tableName)
	{
		string sql = string.Format("pragma table_info([{0}]);", tableName);
		return GetTable (sql);
	}
}

/*Example 
 
   if(SQLite.Instance.Open(bytes)) //or SQLite.Instance.Open("D:/WorkSpace/moon/client/Assets/R/database/data.bytes");
    {
         string sql = string.Format("select * from {0}", "TableName");

         SQLiteTable table = SQLite.Instance.GetTable(sql);

         if(table!=null)
         {
            while(table.Read())
            {
                int id = table.GetByColumnName("id",0);
			    string name = table.GetByColumnName("name","");
            }
            table.Close()
         }

        string drop = @"DROP TABLE IF EXISTS 'TB_Role';";
        SQLite.Instance.Execute(drop);

        string create = @"
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

DROP TABLE IF EXISTS 'TB_Role';

CREATE TABLE TB_Role (
    id             INT           PRIMARY KEY
                                 NOT NULL
                                 DEFAULT (0),
    name           VARCHAR (256) NOT NULL,
    type           INT           NOT NULL,
    config         VARCHAR (256) NOT NULL,
    hp             INT           NOT NULL,
    attack         INT           NOT NULL,
    defense        INT           NOT NULL,
    movedistance   INT           NOT NULL,
    movedirection  INT           NOT NULL,
    attackdistance INT           NOT NULL,
    searchdistance INT           NOT NULL,
    radius         DECIMAL       NOT NULL,
    height         DECIMAL       NOT NULL,
    UNIQUE (
        id
    )
); 

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;

";

         SQLite.Instance.Execute(create);
            
         string name = "Hello";

         string insert = @"INSERT INTO TB_Role (id, name, type, config, hp, attack, defense, movedistance, movedirection, attackdistance, searchdistance, radius, height) VALUES ({0}, '{1}', 1, 'paoshou.txt', 900, 90, 10, 2, 2, 5, 10, 1, 2);";

         for (int i = 0; i < 10; ++i)
         {
            SQLite.Instance.Execute(string.Format(insert, i,name.Replace("'","''")));
         }
    }

 */

