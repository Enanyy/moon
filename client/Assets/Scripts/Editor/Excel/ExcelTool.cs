using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Excel导入到sqlite数据库，当然也可以导出到别的数据文件
/// </summary>
public class ExcelTool
{
    static string database { get { return Application.dataPath + "/r/database/data.bytes"; } }

    const int ROW_INDEX_SERVER          = 0; //服务器导出标志行
    const int ROW_INDEX_CLIEANT         = 1; //客户端导出标志行   
    const int ROW_INDEX_DECS            = 2; //描述行
    const int ROW_INDEX_NAME            = 3; //变量名行
    const int ROW_INDEX_TYPE            = 4; //变量类型行
    const int ROW_INDEX_NULL            = 5; //是否可为空标志行
    const int ROW_INDEX_UNIQUE          = 6; //是否唯一值标志行
    const int ROW_INDEX_DEFAULTVALUE    = 7; //默认值行

    const int ROW_HEADER_COUNT = 8; //文件头行数

    const int MAX_VARCHAR_LENGTH = 1024; //可变长度字符串最大长度
    const int MAX_CHAR_LENGTH = 1024;    //固定长度字符串最大长度

    enum ExportTo
    {
        Database,
        Lua,
    }

    [MenuItem("Tools/Excel/导出Excel数据表(Database)")]
    static void ExportAllToDatabase()
    {
        ExportAllExcel(ExportTo.Database);
    }

    [MenuItem("Tools/Excel/导出Excel数据表(Lua)")]
    static void ExportAllToLua()
    {
        ExportAllExcel(ExportTo.Lua);
    }
    static void ExportAllExcel(ExportTo to)
    {
        string dir = Application.dataPath + "/Excel/";
        string[] files = Directory.GetFiles(dir);
        try
        {
            int count = files.Length;
            for (int i = 0; i < files.Length; ++i)
            {
                string path = files[i];

                EditorUtility.DisplayProgressBar("导出Excel", string.Format("开始导出:{0}", path.Replace(Application.dataPath,"Assets")), (i + 1) * 1f / count);
               
                ExportExcel(path, to);
               
            }
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

    [MenuItem("Assets/Export Excel To Database", true)]
    [MenuItem("Assets/Export Excel To Lua", true)]
    static bool IsExcel()
    {
        if (Selection.activeObject == null)
        {
            return false;
        }
        string path = AssetDatabase.GetAssetPath(Selection.activeObject).ToLower();

        return path.EndsWith(".xlsx") || path.EndsWith(".xls");
    }
    [MenuItem("Assets/Export Excel To Database")]
    static void ExportToDatabase()
    {
        ExportExcel(ExportTo.Database);
    }
    [MenuItem("Assets/Export Excel To Lua")]
    static void ExportToLua()
    {
        ExportExcel(ExportTo.Lua);
    }
    static void ExportExcel(ExportTo to)
    {
        if (Selection.activeObject == null)
        {
            return;
        }
        try
        {
            int count = Selection.objects.Length;
            for (int i = 0; i < count; ++i)
            {
                string path = AssetDatabase.GetAssetPath(Selection.objects[i]);

                EditorUtility.DisplayProgressBar("导出Excel", string.Format("开始导出:{0}", path), (i + 1) * 1f / count);

                string fullpath = Application.dataPath + path.Substring("Assets".Length);

                ExportExcel(fullpath,to);
            }  
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
       
    }
    static void ExportExcel(string path,ExportTo to)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        if (path.EndsWith(".xlsx") || path.EndsWith(".xls"))
        {
            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);

            IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet dataSet = reader.AsDataSet();

            if (dataSet == null || dataSet.Tables == null || dataSet.Tables.Count <= 0)
            {
                stream.Close();
                Debug.Log("Excel表格没有Sheet:" + path);
                return;
            }
            string dir = Path.GetDirectoryName(path);
            try 
            { 
                for (int i = 0; i < dataSet.Tables.Count; ++i)
                {
                    DataTable table = dataSet.Tables[i];
                   
                    if(to == ExportTo.Database)
                    {
                        ExportToDatabase(dir, table);
                    }
                    else
                    {
                        ExportToLua(dir, table);
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            finally{
                stream.Close();
            }
        }
    }
    static void ExportToDatabase(string dir, DataTable table)
    {
        if (table == null)
        {
            return;
        }
        int rowCount = table.Rows.Count;
        int colCount = table.Columns.Count;

        if (rowCount < ROW_HEADER_COUNT || colCount < 2)
        {
            Debug.LogError("Excel表格式有错误:" + table.TableName);
            return;
        }
     
        //新建表的SQL语句
        StringBuilder createBuilder = new StringBuilder();
        createBuilder.AppendFormat("DROP TABLE IF EXISTS '{0}';\nCREATE TABLE {1} (", table.TableName, table.TableName);
        //插入数据的SQL语句
        StringBuilder insertBuilder = new StringBuilder();
        insertBuilder.AppendFormat("INSERT INTO {0} (", table.TableName);
        //值是否唯一SQL语句
        StringBuilder uniqueBuilder = new StringBuilder();
        uniqueBuilder.Append("UNIQUE(");

        bool hasUnique = false;

        for (int i = 1; i < colCount; ++i)
        {
            string flag = table.Rows[ROW_INDEX_CLIEANT][i].ToString();
            if (string.IsNullOrEmpty(flag) || flag.Trim() != "*")
            {
                continue;
            }
            string type = table.Rows[ROW_INDEX_TYPE][i].ToString().Trim();
            if (string.IsNullOrEmpty(type))
            {
                continue;
            }

            if (IsValidDataType(type) == false)
            {
                Debug.LogError("数据类型错误:" + type + " Sheet:" + table.TableName);
                return;
            }

            string name = table.Rows[ROW_INDEX_NAME][i].ToString();
            string isNull = table.Rows[ROW_INDEX_NULL][i].ToString() == "1" ? "" : "NOT NULL";

            if (table.Rows[ROW_INDEX_UNIQUE][i].ToString() == "1")
            {
                uniqueBuilder.Append(name);
                uniqueBuilder.Append(",");

                hasUnique = true;
            }

            string defaultValue = table.Rows[ROW_INDEX_DEFAULTVALUE][i].ToString();

            if (IsStringType(type))
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
        if (hasUnique)
        {
            if (uniqueBuilder[uniqueBuilder.Length - 1] == ',')
            {
                uniqueBuilder.Remove(uniqueBuilder.Length - 1, 1);
            }
            uniqueBuilder.Append(")");
            createBuilder.Append(uniqueBuilder.ToString());
        }
        else
        {
            if (createBuilder[createBuilder.Length - 1] == ',')
            {
                createBuilder.Remove(createBuilder.Length - 1, 1);
            }
        }
        createBuilder.Append(");\n");

        if (insertBuilder[insertBuilder.Length - 1] == ',')
        {
            insertBuilder.Remove(insertBuilder.Length - 1, 1);
        }
        insertBuilder.Append(")");

        string insert = insertBuilder.ToString();
        insertBuilder.Clear();

        for (int i = ROW_HEADER_COUNT; i < rowCount; i++)
        {
            //这一行是否需要导出？
            string flag = table.Rows[i][0].ToString();
            if ( string.IsNullOrEmpty(flag) || flag.Trim() != "*")
            {
                continue;
            }

            //插入数据语句
            insertBuilder.Append(insert);
            insertBuilder.Append(" VALUES (");
            for (int j = 1; j < colCount; ++j)
            {
                //这一列是否需要导出？
                flag = table.Rows[ROW_INDEX_CLIEANT][j].ToString();
                if (string.IsNullOrEmpty(flag) || flag.Trim() != "*")
                {
                    continue;
                }
                string type = table.Rows[ROW_INDEX_TYPE][j].ToString();
                if (string.IsNullOrEmpty(type))
                {
                    continue;
                }
                //替换单引号
                string value = table.Rows[i][j].ToString().Replace("'", "''");
                //字符串添加单引号
                if (IsStringType(type))
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
        //添加事务，因为要执行多条命令
        string create = string.Format("PRAGMA foreign_keys = off;\nBEGIN TRANSACTION;\n{0}COMMIT TRANSACTION;\nPRAGMA foreign_keys = on; ", createBuilder.ToString());

        try
        {
            if (SQLite.Instance.Open(database))
            {
                SQLite.Instance.Execute(create);
                Debug.Log("成功导出:" + table.TableName);
                
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        finally
        {
            string file = string.Format("{0}/sql/{1}.sql", dir, table.TableName);

            FileEx.SaveFile(file, create);
        }
    }
    /// <summary>
    /// 是否是字符串类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static bool IsStringType(string type)
    {
        bool result = type == "STRING" || type == "TEXT";
        if (result == false)
        {
            result = IsVarChar(type);
        }
        if (result == false)
        {
            result = IsChar(type);
        }
        return result;
    }
    /// <summary>
    /// 是否是合法类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static bool IsValidDataType(string type)
    {
        type = type.ToUpper().Trim();
        bool result = type == "INT"
            || type == "BIGINT"
            || type == "BOOLEAN"
            || type == "DECIMAL"
            || type == "DOUBLE" //浮点型
            || type == "INTEGER"
            || type == "STRING"
            || type == "TEXT"
            || type == "DATE"
            || type == "DATETIME"
            || type == "TIME"
            || type == "BLOB"; //二进制大数据

        if (result == false)
        {
            result = IsVarChar(type);
        }
        if (result == false)
        {
            result = IsChar(type);
        }
        return result;

    }
    /// <summary>
    /// 是否是可变长度字符串
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static bool IsVarChar(string type)
    {
        bool result = false;
        //可变长字符串
        if (type.Contains("VARCHAR"))
        {
            string s = type.Replace("VARCHAR", "");
            if (s.Length >= 3 && s[0] == '(' && s[s.Length - 1] == ')')
            {
                string n = s.Substring(1, s.Length - 2);
                if (int.TryParse(n, out int number))
                {
                    result = number > 0 && number <= MAX_VARCHAR_LENGTH; //限制长度
                    if (result == false)
                    {
                        Debug.LogError(string.Format( "可变长字符串长度区间:(0,{0}]",MAX_VARCHAR_LENGTH));
                    }
                }
                //return Regex.IsMatch(n, @"^[0-9]*[1-9][0-9]*$"); //可变长字符串
            }
        }
        return result;
    }
    /// <summary>
    /// 是否固定长度字符串
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static bool IsChar(string type)
    {
        bool result = false;

        if (type.Contains("CHAR"))
        {
            string s = type.Replace("CHAR", "");
            if (s.Length >= 3 && s[0] == '(' && s[s.Length - 1] == ')')
            {
                string n = s.Substring(1, s.Length - 2);
                if (int.TryParse(n, out int number))
                {
                     result = number > 0 && number <= MAX_CHAR_LENGTH; //限制长度
                    if (result == false)
                    {
                        Debug.LogError(string.Format("字符串长度区间:(0,{0}]",MAX_CHAR_LENGTH));
                    }
                }
                //return Regex.IsMatch(n, @"^[0-9]*[1-9][0-9]*$"); //固定长度字符串
            }
        }
        return result;
    }

    public static void ExportToLua(string dir, DataTable table)
    {
        if (table == null)
        {
            return;
        }
        int rowCount = table.Rows.Count;
        int colCount = table.Columns.Count;
        if (rowCount < ROW_HEADER_COUNT || colCount < 2)
        {
            return;
        }
        const string template = @"
--FUNCTION_CODE_BRGIN
{0}
--FUNCTION_CODE_END
local M =
{{
    Data=
    {{
--DATA_CODE_BEGIN
{1}
--DATA_CODE_END
    }}
}}

function M.Get(id)
    return M.Data[id]
end

return M";

        string names = "";
        string nameValues = "";
        for (int i = 1; i < colCount; ++i)
        {
            string flag = table.Rows[ROW_INDEX_CLIEANT][i].ToString();
            if (string.IsNullOrEmpty(flag) || flag.Trim() != "*")
            {
                continue;
            }
            string type = table.Rows[ROW_INDEX_TYPE][i].ToString().Trim();
            if (string.IsNullOrEmpty(type))
            {
                continue;
            }

            if (IsValidDataType(type) == false)
            {
                Debug.LogError("数据类型错误:" + type + " Sheet:" + table.TableName);
                return;
            }
            string name = table.Rows[ROW_INDEX_NAME][i].ToString();

            names += name + ",";
            nameValues += string.Format("{0} = {1},", name, name);
        }
        if (names.Length > 0 && names[names.Length - 1] == ',')
        {
            names = names.Substring(0, names.Length - 1);
        }
        if (nameValues.Length > 0 && nameValues[nameValues.Length - 1] == ',')
        {
            nameValues = nameValues.Substring(0, nameValues.Length - 1);
        }

        StringBuilder insertBuilder = new StringBuilder();

        for (int i = ROW_HEADER_COUNT; i < rowCount; i++)
        {
            //这一行是否需要导出？
            string flag = table.Rows[i][0].ToString();
            if (string.IsNullOrEmpty(flag) || flag.Trim() != "*")
            {
                continue;
            }
            bool setkey = false;

            for (int j = 1; j < colCount; ++j)
            {
                //这一列是否需要导出？
                flag = table.Rows[ROW_INDEX_CLIEANT][j].ToString();
                if (string.IsNullOrEmpty(flag) || flag.Trim() != "*")
                {
                    continue;
                }
                string type = table.Rows[ROW_INDEX_TYPE][j].ToString();
                if (string.IsNullOrEmpty(type))
                {
                    continue;
                }
                //替换单引号
                string value = table.Rows[i][j]
                               .ToString()
                               .Replace("\"", "/\"")
                               .Replace("\'", "\\'");

                //字符串添加单引号
                if (IsStringType(type))
                {
                    if (setkey == false)
                    {
                        setkey = true;
                        insertBuilder.AppendFormat("\t\t['{0}'] = f(", value);
                    }
                    insertBuilder.AppendFormat("'{0}',", value);

                }
                else
                {
                    if (setkey == false)
                    {
                        setkey = true;
                        insertBuilder.AppendFormat("\t\t[{0}] = f(", value);
                    }
                    insertBuilder.AppendFormat("{0},", value);
                }
            }
            if (insertBuilder[insertBuilder.Length - 1] == ',')
            {
                insertBuilder.Remove(insertBuilder.Length - 1, 1);
            }
            insertBuilder.Append("),\n");
        }
        string file = string.Format("{0}/lua/{1}.lua", dir, table.TableName);
        string function = string.Format("local function f({0}) return {{{1}}} end ", names, nameValues);
        if (File.Exists(file))
        {
            string content = File.ReadAllText(file);
            content.ReplaceEx("--FUNCTION_CODE_BRGIN", "--FUNCTION_CODE_END", function);
            content.ReplaceEx("--DATA_CODE_BEGIN", "--DATA_CODE_END", insertBuilder.ToString());

            FileEx.SaveFile(file, content);
        }
        else
        {
            string content = string.Format(template, function, insertBuilder.ToString());

            FileEx.SaveFile(file, content);
        }
    }

}

