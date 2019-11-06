using UnityEngine;
using UnityEditor;
using System.IO;
/// <summary>
/// 根据数据表的字段属性导出相应到结构代码和读取数据代码
/// </summary>
public static class DataTableTool
{
    static string database = Application.dataPath + "/r/database/data.bytes";

    static string template = @"using System.Collections.Generic;
/// <summary>
/// 注释XXXX_BEGIN和XXXX_END为替换区域，这些注释不能删除否则自动生成代码会失败，并且自定义内容不能写在注释之间，否则下次自动生成内容时会覆盖掉。
/// </summary>
public class {classname}
{
//TABLE_DEFINITION_BEGIN
{definition}//TABLE_DEFINITION_END
}

public class {filename} : IDataTable
{
    public DataTableID name
    {
        get {{ return DataTableID.{tablename}; }}
    }

    public readonly Dictionary<{key}, {classname}> data = new Dictionary<{key}, {classname}>();
    /// <summary>
    /// 注释XXXX_BEGIN和XXXX_END为替换区域，这些注释不能删除否则自动生成代码会失败，并且自定义内容不能写在注释之间，否则下次自动生成内容时会覆盖掉。
    /// </summary>
    public void Read(SQLiteTable table)
    {
        while (table.Read())
        {
//TABLE_READ_BEGIN
           {read}
//TABLE_READ_END
        }        
    }
    
    public static {classname} Get({key} id)
    {
        var table = DataTableManager.Instance.Get<{filename}>(DataTableID.{tablename});
        if(table.data.ContainsKey(id))
        {
            return table.data[id];
        }
        return null;
    }
    
}";

    [MenuItem("Tools/Data/导出数据表代码")]
    static void GenDataTable()
    {
        if (SQLite.Instance.Open(database))
        {
            SQLiteTable table = SQLite.Instance.GetTables();
            if (table != null)
            {
                string IDString = "";
                string registerString = "";
                while (table.Read())
                {
                    string tableName = table.GetByColumnName("name", "");

                    string className = tableName.Replace("TB_", "TB");

                    string fileName = tableName.Replace("TB_", "DT");

                    IDString += "\t" + tableName + ",\n";

                    registerString += string.Format("\t\t\tRegister(new {0}());\n", fileName);

                    string path = string.Format("{0}/Scripts/Data/Tables/{1}.cs", Application.dataPath, fileName);
                    string definition = "";
                    string read = string.Format("\t{0} o = new {1}();\n", className, className);
                    string key = null;

                    SQLiteTable info = SQLite.Instance.GetTableInfo(tableName);

                    if (info != null)
                    {
                        while (info.Read())
                        {
                            string columnName = info.GetByColumnName("name", "");
                            string columnType = GetType(info.GetByColumnName("type", ""));

                            if (string.IsNullOrEmpty(key))
                            {
                                key = columnType;
                            }
                            definition += string.Format("\tpublic {0} {1};\n", columnType, columnName);
                            read += string.Format("\t\t\to.{0} = table.GetByColumnName(\"{1}\",{2});\n", columnName, columnName, (columnType == "string" ? "\"\"" : "0"));

                        }

                        read += "\t\t\tdata.Add(o.id,o);";



                        info.Close();
                    }
                    if (File.Exists(path) == false)
                    {
                        string code = template.Replace("{filename}", fileName)
                                              .Replace("{definition}", definition)
                                              .Replace("{classname}", className)
                                              .Replace("{tablename}", tableName)
                                              .Replace("{read}", read)
                                              .Replace("{key}", key);

                        Debug.Log(code);

                        File.WriteAllText(path, code);
                    }
                    else
                    {
                        string datatable = File.ReadAllText(path);
                        datatable = datatable.ReplaceEx("//TABLE_DEFINITION_BEGIN", "//TABLE_DEFINITION_END", definition);
                        datatable = datatable.ReplaceEx("//TABLE_READ_BEGIN", "//TABLE_READ_END", "\t\t"+ read+"\n");

                        File.WriteAllText(path, datatable);
                    }
                }
                table.Close();

                string pathFile = Application.dataPath + "/Scripts/Data/DataTableManager.cs";

                string content = File.ReadAllText(pathFile);

                content = content.ReplaceEx("//DATATABLE_ID_BEGIN", "//DATATABLE_ID_END", IDString);

                content = content.ReplaceEx("//DATATABLE_REGISTER_BEGIN", "//DATATABLE_REGISTER_END",registerString);

                File.WriteAllText(pathFile, content);
            }
        }
        else
        {
            Debug.LogError("Can't open database:" + database);

        }

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

            default: return "string";

        }
    }
    
}