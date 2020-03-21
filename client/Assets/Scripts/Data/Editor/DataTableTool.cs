using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
/// <summary>
/// 根据数据表的字段属性导出相应到结构代码和读取数据代码
/// </summary>
public class DataTableTool : EditorWindow
{
    static string database { get { return Application.dataPath + "/r/database/data.bytes"; } }
    static string manager { get { return Application.dataPath + "/Scripts/Data/DataTableManager.cs"; } }

    static string template = @"using System.Collections.Generic;
/// <summary>
/// 注释XXXX_BEGIN和XXXX_END为替换区域，这些注释不能删除否则自动生成代码会失败，并且自定义内容不能写在注释之间，否则下次自动生成内容时会覆盖掉。
/// </summary>
public class {classname}
{
//TABLE_DEFINITION_BEGIN
{definition}//TABLE_DEFINITION_END
}
[DataTable]
public class {filename} : IDataTable
{
    public DataTableID name
    {
        get { return DataTableID.{tablename}; }
    }
    /// <summary>
    /// 用于快速查找
    /// </summary>
    public readonly Dictionary<{key}, {classname}> dic = new Dictionary<{key}, {classname}>();
    /// <summary>
    /// 用于获取列表
    /// </summary>
    public readonly List<{classname}> list = new List<{classname}>();

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
           {read}
//TABLE_READ_END
        }        
    }
    private static {filename} mTable;
    private static {filename} table
    {
        get
        {
            if(mTable == null)
            {
                mTable = DataTableManager.Instance.Get<{filename}>(DataTableID.{tablename});
            }
            return mTable;
        }
    }
    
    public static {classname} Get({key} key)
    {
        if (table != null && table.dic!= null)
        {
            table.dic.TryGetValue(key, out {classname} data);
            return data;
        }
        return null;
    }
    
}";

    [MenuItem("Tools/Data/导出数据表代码(Single)")]
    static void OpenWindow()
    {
        DataTableTool tool =  GetWindow<DataTableTool>();
        tool.titleContent = new GUIContent("导出数据表代码");
    }

    private List<string> mTableNames = new List<string>();
    private int mSelectedIndex = 0;
    private void OnEnable()
    {
        mTableNames.Clear();
        if (SQLite.Instance.Open(database))
        {
            SQLiteTable table = SQLite.Instance.GetTables();
            mTableNames.Add("All");
            if (table != null)
            {
                while (table.Read())
                {
                    string tableName = table.GetByColumnName("name", "");
                    mTableNames.Add(tableName);
                }
                table.Close();
            }
        }
    }
    private void OnDisable()
    {
        SQLite.Instance.Close();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("数据库:", database.Replace(Application.dataPath,"Assets"));

        EditorGUILayout.Space();


        mSelectedIndex = EditorGUILayout.Popup("选择要导出的数据表:", mSelectedIndex, mTableNames.ToArray());

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (mTableNames.Count > 0 && mSelectedIndex >= 0 && mSelectedIndex < mTableNames.Count)
        {
            if (GUILayout.Button("导出"))
            {
                if (mSelectedIndex == 0)
                {
                    ExportDataTables(mTableNames);
                }
                else
                {
                    ExportDataTable(mTableNames[mSelectedIndex]);
                }
            }
        }
    }
    static void ExportDataTable(string tableName)
    {
        string IDString = "";
       
        string content = File.ReadAllText(manager);

        int beginIndex = content.IndexOf("//DATATABLE_ID_BEGIN");
        int endIndex = content.IndexOf("//DATATABLE_ID_END");

        if (beginIndex >= 0 && endIndex >= 0)
        {
            int beginIndexLength = "//DATATABLE_ID_BEGIN".Length;
            IDString = content.Substring(beginIndex + beginIndexLength, endIndex - beginIndex - beginIndexLength);
            if (IDString.Length > 0 && IDString[0] == '\r')
            {
                IDString = IDString.Substring(1);
            }

        }
       

        string className = tableName.Replace("TB_", "TB");

        string fileName = tableName.Replace("TB_", "DT");

        ExportDataTable(tableName, fileName, className);

        if (IDString.Contains(tableName) == false)
        {
            IDString += "\t" + tableName + ",\n";
        }

        content = content.ReplaceEx("//DATATABLE_ID_BEGIN", "//DATATABLE_ID_END", IDString);

        FileEx.SaveFile(manager, content);

        Debug.Log("成功替换注册代码！");
    }

    [MenuItem("Tools/Data/导出数据表代码(All)")]
    static void ExportDataTables()
    {
        if (SQLite.Instance.Open(database))
        {
            SQLiteTable table = SQLite.Instance.GetTables();
            if (table != null)
            {
                List<string> tableNames = new List<string>();
                while (table.Read())
                {
                    string tableName = table.GetByColumnName("name", "");

                    tableNames.Add(tableName);
                }
                table.Close();

                ExportDataTables(tableNames);
            }
            SQLite.Instance.Close();
        }

        else
        {
            Debug.LogError("Can't open database:" + database);
        }
    }
    static void ExportDataTables(List<string> tables)
    {
        if (tables != null)
        {
            string IDString = "";
            string registerString = "";
            try
            {
                int count = tables.Count;

                for(int i = 0; i < count; ++i)
                {
                    string tableName = tables[i];
                    if (tableName == "All")
                    {
                        continue;
                    }

                    EditorUtility.DisplayProgressBar("导出数据表代码", string.Format("正在导出:{0}", tableName), (i + 1) * 1f / count);

                    string className = tableName.Replace("TB_", "TB");

                    string fileName = tableName.Replace("TB_", "DT");

                    IDString += "\t" + tableName + ",\n";

                    registerString += string.Format("\t\t\tRegister(new {0}());\n", fileName);

                    ExportDataTable(tableName, fileName, className);

                }

                string content = File.ReadAllText(manager);

                content = content.ReplaceEx("//DATATABLE_ID_BEGIN", "//DATATABLE_ID_END", IDString);

                content = content.ReplaceEx("//DATATABLE_REGISTER_BEGIN", "//DATATABLE_REGISTER_END", registerString);

                FileEx.SaveFile(manager, content);

                Debug.Log("成功替换全部注册代码！");
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }

    static void ExportDataTable(string tableName, string fileName, string className)
    {
        if (string.IsNullOrEmpty(tableName))
        {
            return;
        }

        if (SQLite.Instance.IsOpen() == false)
        {
            if (SQLite.Instance.Open(database) ==false)
            {
                Debug.LogError("无法打开数据库:" + database);
                return;
            }
        }
        SQLiteTable info  = SQLite.Instance.GetTableInfo(tableName);

        if (info == null)
        {
            Debug.LogError("无法读取表" + tableName + "信息");
            return;
        }

        string path = string.Format("{0}/Scripts/Data/Tables/{1}.cs", Application.dataPath, fileName);
        string definition = "";
        string read = string.Format("\t{0} o = new {1}();\n", className, className);
        string keyName = null;
        string keyType = null;

        while (info.Read())
        {
            string columnName = info.GetByColumnName("name", "");
            string columnType = GetType(info.GetByColumnName("type", ""));

            if (string.IsNullOrEmpty(keyName))
            {
                keyName = columnName;
            }
            if (string.IsNullOrEmpty(keyType))
            {
                keyType = columnType;
            }
            
            definition += string.Format("\tpublic {0} {1};\n", columnType, columnName);
            read += string.Format("\t\t\to.{0} = table.GetByColumnName(\"{1}\", {2});\n", columnName, columnName, GetDefaultValue(columnType));

        }

        read += string.Format("\t\t\tdic.Add(o.{0},o);\n\t\t\tlist.Add(o);", keyName);

        info.Close();


        if (File.Exists(path) == false)
        {
            string code = template.Replace("{filename}", fileName)
                                  .Replace("{definition}", definition)
                                  .Replace("{classname}", className)
                                  .Replace("{tablename}", tableName)
                                  .Replace("{read}", read)
                                  .Replace("{key}", keyType);

            Debug.Log(code);

            FileEx.SaveFile(path, code);
        }
        else
        {
            string datatable = File.ReadAllText(path);
            datatable = datatable.ReplaceEx("//TABLE_DEFINITION_BEGIN", "//TABLE_DEFINITION_END", definition);
            datatable = datatable.ReplaceEx("//TABLE_READ_BEGIN", "//TABLE_READ_END", "\t\t" + read + "\n");

            FileEx.SaveFile(path, datatable);
        }

        Debug.Log("成功导出数据表:" + tableName);
    }

    static string GetType(string type)
    {
        switch (type.ToUpper())
        {
            case "INT":
            case "INTEGER": return "int";
            case "BIGINT":return "long";
            case "DECIMAL": return "float";
            case "DOUBLE":return "double";
            case "BLOB":return "byte[]";

            default: return "string";

        }
    }
    static string GetDefaultValue(string type)
    {
        switch(type)
        {
            case "int":return "0";
            case "float":return "0f";
            case "double":return "0f";
            case "long":return "0L";
            case "string":return "\"\"";
            case "byte[]":return "null";
        }
        return "0";
    }
    
}