using UnityEngine;
using UnityEditor;
using System.IO;
using Excel;
using System.Data;
using System;
using System.Text;

public static class DataTableTool
{
    static string database = Application.dataPath + "/r/database/data.bytes";

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
                    if (File.Exists(path) == false)
                    {
                        SQLiteTable info = SQLite.Instance.GetTableInfo(tableName);

                        string code = @"public class {0} : IDataTable
{{
    public DataTableID name
    {{
        get {{ return DataTableID.{1}; }}
    }}

    //public readonly Dictionary<int, {2}> data = new Dictionary<int, {3}>();

    public void Read(SQLiteTable table)
    {{
        while (table.Read())
        {{
//READ_START
           {4}
//READ_END
        }}          
    }}
    /*
    public static {5} Get(int id)
    {{
        var table = DataTableManager.Instance.Get<{6}>(DataTableID.{7});
        if(table.data.ContainsKey(id))
        {{
            return table.data[id];
        }}
        return null;
    }}
    */
}}";

                        if (info != null)
                        {
                            string tb = "using System.Collections.Generic;\n\npublic class " + className + "\n{\n//DEFINITION_START\n";
                            string read = "/*\t" + className + " o = new " + className + "();\n";

                            while (info.Read())
                            {
                                string columnName = info.GetByColumnName("name", "");
                                string columnType = GetType(info.GetByColumnName("type", ""));

                                tb += "\tpublic " + columnType + " " + columnName + ";\n";
                                read += "\t\t\to." + columnName + " = table.GetByColumnName(\"" + columnName + "\"," + (columnType == "string" ? "\"\"" : "0") + ");\n";
                            }

                            tb += "//DEFINITION_END\n";
                            read += "\t\t\tdata.Add(o.id,o);*/";
                            tb += "}";

                            code = string.Format(code,
                                fileName,
                                tableName,
                                className,
                                className,
                                read,
                                className,
                                fileName,
                                tableName
                                );
                            code = tb + "\n\n" + code;
                            Debug.Log(code);

                            File.WriteAllText(path, code);

                            info.Close();
                        }
                    }
                }
                table.Close();


                string pathFile = Application.dataPath + "/Scripts/Data/DataTableManager.cs";


                string content = File.ReadAllText(pathFile);

                int startIDIndex = content.IndexOf("//DATATABLE_ID_START");
                int endIDIndex = content.IndexOf("//DATATABLE_ID_END");

                string part1 = content.Substring(0, startIDIndex + "//DATATABLE_ID_START".Length + 1);
                string part2 = content.Substring(endIDIndex);

                content = part1 + IDString + part2;

                int startPathIndex = content.IndexOf("//DATATABLE_REGISTER_START");
                int endPathIndex = content.IndexOf("//DATATABLE_REGISTER_END");

                part1 = content.Substring(0, startPathIndex + "//DATATABLE_REGISTER_START".Length + 1);
                part2 = content.Substring(endPathIndex);

                content = part1 + registerString + part2;

                StreamWriter writer = new StreamWriter(pathFile);
                writer.Write(content);
                writer.Close();
                writer.Dispose();
            }
            else
            {

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

    [MenuItem("Assets/Export Excel", true)]
    static bool IsExcel()
    {
        if (Selection.activeObject == null)
        {
            return false;
        }
        string path = AssetDatabase.GetAssetPath(Selection.activeObject).ToLower();

        return path.EndsWith(".xlsx") || path.EndsWith(".xls");
    }
    [MenuItem("Assets/Export Excel")]
    static void ExportExcel()
    {
        if (Selection.activeObject == null)
        {
            return;
        }
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (path.EndsWith(".xlsx") || path.EndsWith(".xls"))
        {
            string fullpath = Application.dataPath + path.Substring("assets".Length);

            FileStream stream = File.Open(fullpath, FileMode.Open, FileAccess.Read);

            IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet dataSet = reader.AsDataSet();

            if (dataSet == null || dataSet.Tables == null || dataSet.Tables.Count <= 0)
            {
                Debug.Log("Excel表格没有Sheet:" + fullpath);
                return;
            }

            for (int i = 0; i < dataSet.Tables.Count; ++i)
            {
                ExportTable(Path.GetDirectoryName(fullpath), dataSet.Tables[i]);
            }

        }
    }
    static void ExportTable(string dir, DataTable table)
    {
        if (table == null)
        {
            return;
        }
        int rowCount = table.Rows.Count;
        int colCount = table.Columns.Count;

        if (rowCount < 7 || colCount < 2)
        {
            return;
        }
        StringBuilder createBuilder = new StringBuilder();
        createBuilder.AppendFormat("DROP TABLE IF EXISTS '{0}';\nCREATE TABLE {1} (", table.TableName, table.TableName);

        StringBuilder insertBuilder = new StringBuilder();
        insertBuilder.AppendFormat("INSERT INTO {0} (", table.TableName);

        StringBuilder uniqueBuilder = new StringBuilder();
        uniqueBuilder.Append("UNIQUE(");

        for (int i = 1; i < colCount; ++i)
        {
            if (table.Rows[1][i].ToString().Trim() != "*")
            {
                continue;
            }
            string type = table.Rows[2][i].ToString();
            if (string.IsNullOrEmpty(type))
            {
                continue;
            }

            if (IsValidDataType(type) == false)
            {
                Debug.LogError("数据类型错误:" + type);
                return;
            }

            string name = table.Rows[3][i].ToString();
            string isNull = table.Rows[4][i].ToString() == "1" ? "" : "NOT NULL";

            if (table.Rows[5][i].ToString() == "1")
            {
                uniqueBuilder.Append(name);
                uniqueBuilder.Append(",");
            }

            string defaultValue = table.Rows[6][i].ToString();

            if (type.Contains("CHAR") || type.Contains("TEXT"))
            {
                if (string.IsNullOrEmpty(defaultValue) == false)
                {
                    defaultValue = "DEFAULT " + defaultValue;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(defaultValue) == false)
                {
                    defaultValue = string.Format("DEFAULT ({0})", defaultValue);
                }
            }
            createBuilder.AppendFormat("{0} {1} {2} {3},", name, type, isNull, defaultValue);

            insertBuilder.AppendFormat("{0},", name);

        }
        if (uniqueBuilder[uniqueBuilder.Length - 1] == ',')
        {
            uniqueBuilder.Remove(uniqueBuilder.Length - 1, 1);
        }
        uniqueBuilder.Append(")");
        createBuilder.Append(uniqueBuilder.ToString());
        createBuilder.Append(");\n");

        if (insertBuilder[insertBuilder.Length - 1] == ',')
        {
            insertBuilder.Remove(insertBuilder.Length - 1, 1);
        }
        insertBuilder.Append(")");

        string insert = insertBuilder.ToString();
        insertBuilder.Clear();


        for (int i = 7; i < rowCount; i++)
        {
            if (table.Rows[i][0].ToString().Trim() != "*")
            {
                continue;
            }
            insertBuilder.Append(insert);
            insertBuilder.Append(" VALUES (");
            for (int j = 1; j < colCount; ++j)
            {
                string type = table.Rows[2][j].ToString();
                if (string.IsNullOrEmpty(type))
                {
                    continue;
                }
                string value = table.Rows[i][j].ToString().Replace("'", "''");

                if (type.Contains("CHAR") || type.Contains("TEXT"))
                {

                    insertBuilder.AppendFormat("'{0}',", value);
                }
                else
                {
                    insertBuilder.AppendFormat("{0},", value);
                }
            }
            if (insertBuilder[insertBuilder.Length - 1] == ',')
            {
                insertBuilder.Remove(insertBuilder.Length - 1, 1);
            }
            insertBuilder.Append(");\n");
        }

        createBuilder.Append(insertBuilder.ToString());

        string create = createBuilder.ToString();


        if (SQLite.Instance.Open(database))
        {
            SQLite.Instance.Execute(create);
        }
        dir += "/sql/";
        if (Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory(dir);
        }
        string file = string.Format("{0}{1}.sql", dir, table.TableName);

        StreamWriter writer = new StreamWriter(file);
        writer.Write(create);
        writer.Close();
        writer.Dispose();

        Debug.Log("导出成功:" + table.TableName);
    }

    static bool IsValidDataType(string type)
    {
        type = type.ToUpper().Trim();
        return type == "INT"
            || type == "BIGINT"
            || type == "BOOLEAN"
            || type == "DECIMAL"
            || type == "DOUBLE"
            || type == "INTEGER"
            || type == "STRING"
            || type == "TEXT"
            || type.Contains("VARCHAR");
    }
}