using System;
using System.Collections;
using Community.CsharpSqlite;

public class SQLiteQuery {
	
	private SQLiteConnection         sqlDb;
	private Sqlite3.sqlite3  db;
	private Sqlite3.Vdbe     vm;
	private string[]         columnNames;
	private int[]            columnTypes;
	private int              bindIndex;
	
	public string[] Names { get {return columnNames;} }

	public int[] Types{ get { return columnTypes; } }
	public SQLiteQuery( SQLiteConnection sqliteDb, string query )
	{
		sqlDb = sqliteDb;
		bindIndex = 1;
		db = sqliteDb.Connection();
		if( Sqlite3.sqlite3_prepare_v2( db, query, query.Length, ref vm, 0 ) != Sqlite3.SQLITE_OK ) 
		{ 
			throw new Exception( "Error with prepare query! error:" + Sqlite3.sqlite3_errmsg(db) ); 
		};
		sqlDb.RegisterQuery(this);
	}
	
	public void Reset()
	{
		bindIndex = 1;
		
		if( Sqlite3.sqlite3_reset( vm ) != Sqlite3.SQLITE_OK ) 
		{ 
			throw new Exception( "Error with sqlite3_reset!" ); 
		};
	}
	
	public void Release()
	{
		sqlDb.UnregisterQuery(this);
		
		if( Sqlite3.sqlite3_reset( vm ) != Sqlite3.SQLITE_OK ) 
		{ 
			throw new Exception( "Error with sqlite3_reset!" ); 
		};
		
		if( Sqlite3.sqlite3_finalize( vm ) != Sqlite3.SQLITE_OK ) 
		{ 
			throw new Exception( "Error with sqlite3_finalize!" ); 
		};
	}
	
	public void Bind( string str ) {BindAt(str,-1);}
	public void BindAt( string str, int bindAt )
	{
		if( bindAt == -1 )
		{
			bindAt = bindIndex++;
		}
		if( Sqlite3.sqlite3_bind_text( vm, bindAt, str, -1, null ) != Sqlite3.SQLITE_OK ) 
		{ 
			throw new Exception( "SQLite fail to bind string with error: " + Sqlite3.sqlite3_errmsg(db) ); 
		};
	}
	
	public void Bind( int integer ) {BindAt(integer,-1);}
	public void BindAt( int integer, int bindAt )
	{
		if( bindAt == -1 )
		{
			bindAt = bindIndex++;
		}
		if( Sqlite3.sqlite3_bind_int( vm, bindAt, integer ) != Sqlite3.SQLITE_OK ) 
		{ 
			throw new Exception( "SQLite fail to bind integer with error: " + Sqlite3.sqlite3_errmsg(db) ); 
		};
	}
	
	public void Bind( double real ) {BindAt(real,-1);}
	public void BindAt( double real, int bindAt )
	{
		if( bindAt == -1 )
		{
			bindAt = bindIndex++;
		}
		if( Sqlite3.sqlite3_bind_double( vm, bindAt, real ) != Sqlite3.SQLITE_OK ) 
		{ 
			throw new Exception( "SQLite fail to bind double with error: " + Sqlite3.sqlite3_errmsg(db) ); 
		};
	}
	
	public void Bind( byte[] blob ) {BindAt(blob,-1);}
	public void BindAt( byte[] blob, int bindAt )
	{
		if( bindAt == -1 )
		{
			bindAt = bindIndex++;
		}
		if( Sqlite3.sqlite3_bind_blob( vm, bindAt, blob, blob.Length, null ) != Sqlite3.SQLITE_OK ) 
		{ 
			throw new Exception( "SQLite fail to bind blob with error: " + Sqlite3.sqlite3_errmsg(db) ); 
		};
	}
	
	public void BindNull() {BindNullAt(-1);}
	public void BindNullAt( int bindAt )
	{
		if( bindAt == -1 )
		{
			bindAt = bindIndex++;
		}
		if( Sqlite3.sqlite3_bind_null( vm, bindAt ) != Sqlite3.SQLITE_OK ) 
		{ 
			throw new Exception( "SQLite fail to bind null error: " + Sqlite3.sqlite3_errmsg(db) ); 
		};
	}

	
	public bool Step()
	{
		switch( Sqlite3.sqlite3_step( vm ))
		{
		case Sqlite3.SQLITE_DONE: return false;
		case Sqlite3.SQLITE_ROW: 
		{
			
			int columnCount = Sqlite3.sqlite3_column_count( vm );
			columnNames = new string[columnCount];
			columnTypes = new int[columnCount];

			try
			{
				// reads columns one by one
				for ( int i = 0; i < columnCount; i++ )
				{
					columnNames[i] = Sqlite3.sqlite3_column_name( vm, i );
					columnTypes[i] = Sqlite3.sqlite3_column_type( vm, i );
				}
			}
			catch
			{
				throw new Exception( "SQLite fail to read column's names and types! error: " + Sqlite3.sqlite3_errmsg(db));
			}
			
			return true; 
		}
		}	
		throw new Exception( "SQLite step fail! error: " + Sqlite3.sqlite3_errmsg(db));
	}



	
	public bool IsNULL( string field )
	{
		int i = GetFieldIndex( field );
		return Sqlite3.SQLITE_NULL == columnTypes[i];
	}
	
	public int GetFieldType( string field )
	{
		for( int i = 0; i < columnNames.Length; i++ )
		{
			if( columnNames[i] == field )
				return columnTypes[i];
		}
		throw new Exception( "SQLite unknown field name: " + field);
	}	


	public int GetFieldType( int field )
	{
		if ( columnTypes==null || field < 0 || field >= columnTypes.Length) 
		{
			return -1;
		}
		return columnTypes[field];


	//	throw new Exception( "SQLite unknown field name: " + field);
	}	



	public  int GetFieldIndex(string field)
	{
		for( int i = 0; i < columnNames.Length; i++ )
		{
			if( columnNames[i] == field )
				return i;
		}

		return -1;
		//throw new Exception( "SQLite unknown field name: " + field);
	}
	
	public string GetString( string field )
	{
		int i = GetFieldIndex( field );
		if (i < 0)
		{
			return string.Empty;
		}
		if( Sqlite3.SQLITE_TEXT == columnTypes[i])
		{
			return Sqlite3.sqlite3_column_text( vm, i );
		}
		return string.Empty;
		//throw new Exception( "SQLite wrong field type (expecting String) : " + field);
	}



	public string  GetString(int  filedindex)
	{
		if (filedindex < 0 || filedindex >= Names.Length) 
		{
			return string.Empty;
		}

		if( Sqlite3.SQLITE_TEXT == columnTypes[filedindex])
		{
			return Sqlite3.sqlite3_column_text( vm, filedindex);
		}

		return string.Empty;

	}


    public long GetLongeger(int index)
    {
        if (index < 0 || index >= Names.Length)
        {
            throw new Exception("SQLite wrong field type (expecting Longeger) : " + index);
        }

        if (Sqlite3.SQLITE_INTEGER == columnTypes[index])
        {
            return Sqlite3.sqlite3_column_int64(vm, index);
        }


        throw new Exception("SQLite wrong field type (expecting Longeger) : " + index);
    }


    public int GetInteger( string field )
	{
		int i = GetFieldIndex( field );

		if (i < 0  || i>=Names.Length)
		{
			throw new Exception( "SQLite wrong field type (expecting Integer) : " + field);
	
		}
		if (Sqlite3.SQLITE_INTEGER == columnTypes [i]) 
		{
						return Sqlite3.sqlite3_column_int (vm, i);
		} 
		
		throw new Exception( "SQLite wrong field type (expecting Integer) : " + field);
	}



	public int GetInteger( int index )
	{
		if (index < 0 || index >= Names.Length) {
			throw new Exception( "SQLite wrong field type (expecting Integer) : " + index);
				}

		//int i = GetFieldIndex( field );
		if( Sqlite3.SQLITE_INTEGER == columnTypes[index])
		{
			return Sqlite3.sqlite3_column_int( vm, index );
		}


		throw new Exception( "SQLite wrong field type (expecting Integer) : " + index);
	}




	
	public float  GetFloat( string field )
	{
		int i = GetFieldIndex( field );
		if (i < 0||i>=Names.Length) 
		{
			throw new Exception( "SQLite wrong field type (expecting Double) : " + field);
		}
		if (Sqlite3.SQLITE_FLOAT == columnTypes [i])
		{
						return  (float )Sqlite3.sqlite3_column_double (vm, i);
		} 
		
	throw new Exception( "SQLite wrong field type (expecting Double) : " + field);
	}



	public float GetFloat(int fieldindex )
	{

		if (fieldindex < 0 || fieldindex>=Names.Length) 
		{
			throw new Exception( "SQLite wrong field type (expecting FLOAT) : " + fieldindex);
		}
		//if (Sqlite3.SQLITE_FLOAT == columnTypes [fieldindex])
		{
			return (float)Sqlite3.sqlite3_column_double (vm, fieldindex);
		} 
		
//		throw new Exception( "SQLite wrong field type (expecting FLOAT) : " + fieldindex);
	}








	public byte[] GetBlob( string field )
	{
		int i = GetFieldIndex( field );
		if (i < 0) {
			return null;
				}
		if( Sqlite3.SQLITE_BLOB == columnTypes[i])
		{
			return Sqlite3.sqlite3_column_blob( vm, i );
		}

		return null;
		
	//	throw new Exception( "SQLite wrong field type (expecting byte[]) : " + field);
	}

	
	
}
