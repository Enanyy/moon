using System;
using System.Collections;
using UnityEngine;
using Community.CsharpSqlite;

public class SQLiteDataTable : IDisposable
{
	private SQLiteQuery mLiteQuery;

	private bool mIsDisposed = false;


	public SQLiteDataTable(SQLiteQuery varLiteQuery)
	{
		mLiteQuery = varLiteQuery;
	}


	~SQLiteDataTable()
	{
		Dispose(false);
	}


	public void Close()
	{
		Dispose(true);
	}


	public void Dispose()
	{
		Dispose(true);
	}


	protected void Dispose(bool varDiposing)
	{
		if (!mIsDisposed)
		{
			if (varDiposing)
			{
				//Clean Up managed resources  
				if (mLiteQuery != null)
				{
					mLiteQuery.Release();
					mLiteQuery = null;
				}
			}
			//Clean up unmanaged resources  
		}
		mIsDisposed = true;
	}

	public int columnCount
	{

		get
		{
			if (mLiteQuery == null)
			{
				return 0;
			}

			if (mLiteQuery.Names == null)
			{
				return 0;
			}

			return mLiteQuery.Names.Length;
		}
	}

	public string GetByIndex(int varIndex, string varDefaultValue)
	{
		if (mLiteQuery == null || varIndex >= columnCount || varIndex < 0)
		{
			return string.Empty;
		}

		if (mLiteQuery.GetFieldType(varIndex) != Sqlite3.SQLITE_TEXT)
		{
			return varDefaultValue;
		}


		try
		{
			return mLiteQuery.GetString(varIndex);
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
			return varDefaultValue;
		}
	}



	public int GetByIndex(int varIndex, int varDefaultValue)
	{
		if (mLiteQuery == null || varIndex >= columnCount || varIndex < 0)
		{
			return varDefaultValue;
		}

		if (mLiteQuery.GetFieldType(varIndex) != Sqlite3.SQLITE_INTEGER)
		{
			return varDefaultValue;
		}

		try
		{
			return mLiteQuery.GetInteger(varIndex);
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
			return varDefaultValue;
		}
	}

	// 读取浮点型数据;
	public float GetByIndex(int varIndex, float varDefaultValue)
	{
		try
		{
			return mLiteQuery.GetFloat(varIndex);
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
			return varDefaultValue;
		}
	}

	/// <summary>
	/// 通过索引查找列名.
	/// </summary>
	/// <returns>The column name by index.</returns>
	/// <param name="varIndex">Variable index.</param>
	public string GetColumnNameByIndex(int varIndex)
	{
		if (mLiteQuery == null || varIndex < 0 || varIndex >= columnCount || mLiteQuery.Names == null)
		{
			return string.Empty;
		}
		return mLiteQuery.Names[varIndex];
	}

	/// <summary>
	/// 通过列名查找索引.
	/// </summary>
	/// <returns>The column index by name.</returns>
	/// <param name="varColumnName">Variable column name.</param>
	public int GetColumnIndexByName(string varColumnName)
	{

		if (mLiteQuery == null || string.IsNullOrEmpty(varColumnName))
		{
			return -1;
		}

		try
		{
			return mLiteQuery.GetFieldIndex(varColumnName);
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message);
		}

		return -1;
	}

	/// <summary>
	/// 通过列名查找对应的值，返回字符串.
	/// </summary>
	/// <returns>The by column name.</returns>
	/// <param name="varColumnName">Variable column name.</param>
	/// <param name="varDefaultValue">Variable default value.</param>
	public string GetByColumnName(string varColumnName, string varDefaultValue)
	{
		if (mLiteQuery == null || string.IsNullOrEmpty(varColumnName))
		{
			return null;
		}

		try
		{

			int tempColNumber = GetColumnIndexByName(varColumnName);
			if (tempColNumber < 0 || tempColNumber >= columnCount)
			{
				return varDefaultValue;
			}

			return mLiteQuery.GetString(tempColNumber);
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
			return varDefaultValue;
		}

	}

	/// <summary>
	/// 通过列名查找对应的值，返回浮点型.
	/// </summary>
	/// <returns>The by column name.</returns>
	/// <param name="varColumnName">Variable column name.</param>
	/// <param name="varDefaultValue">Variable default value.</param>
	public float GetByColumnName(string varColumnName, float varDefaultValue)
	{
		try
		{
			int tempIndex = GetColumnIndexByName(varColumnName);
			if (tempIndex < 0)
			{
				return varDefaultValue;
			}

			return GetByIndex(tempIndex, varDefaultValue);
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
			return varDefaultValue;
		}
	}

	/// <summary>
	/// 通过列名查找对应的值，返回整型.
	/// </summary>
	/// <returns>The by column name.</returns>
	/// <param name="varColumnName">Variable column name.</param>
	/// <param name="varDefaultValue">Variable default value.</param>
	public int GetByColumnName(string varColumnName, int varDefaultValue)
	{
		try
		{
			int tempFileldIndex = mLiteQuery.GetFieldIndex(varColumnName);
			if (tempFileldIndex < 0)
			{
				return -1;
			}

			return mLiteQuery.GetInteger(tempFileldIndex);
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
			return varDefaultValue;
		}
	}
	/// <summary>
	/// 通过列名查找对应的值，返回整型.
	/// </summary>
	/// <returns>The by column name.</returns>
	/// <param name="varColumnName">Variable column name.</param>
	/// <param name="varDefaultValue">Variable default value.</param>
	public long GetByColumnNameLong(string varColumnName, long varDefaultValue)
	{
		try
		{
			int tempFileldIndex = mLiteQuery.GetFieldIndex(varColumnName);
			if (tempFileldIndex < 0)
			{
				return -1;
			}

			return mLiteQuery.GetLongeger(tempFileldIndex);
		}
		catch (System.Exception e)
		{
			Debug.LogException(e);
			return varDefaultValue;
		}
	}

	/// <summary>
	/// go to next row .if this row is end return false 
	/// </summary>
	/// <returns></returns>
	public bool Read()
	{
		if (mLiteQuery == null)
		{
			return false;
		}

		return mLiteQuery.Step();

	}

}

