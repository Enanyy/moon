using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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

    [MenuItem("Tools/Excel/导出Excel数据表")]
    static void ExportAllExcel()
    {
        string dir = Application.dataPath + "/Excel/";
        string[] files = Directory.GetFiles(dir);
        for (int i = 0; i < files.Length; ++i)
        {
            ExportExcel(files[i]);
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
        for (int i = 0; i < Selection.objects.Length; ++i)
        {
            string path = AssetDatabase.GetAssetPath(Selection.objects[i]);

            string fullpath = Application.dataPath + path.Substring("Assets".Length);

            ExportExcel(fullpath);
        }

    }
    static void ExportExcel(string path)
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

            for (int i = 0; i < dataSet.Tables.Count; ++i)
            {
                ExportTable(dir, dataSet.Tables[i]);
            }

            stream.Close();
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
            //可变长字符串
            if (type.Contains("VARCHAR"))
            {
                string s = type.Replace("VARCHAR", "");
                if (s.Length >= 3 && s[0] =='(' && s[s.Length - 1] ==')')
                {
                    string n = s.Substring(1, s.Length - 2);
                    if (int.TryParse(n, out int number))
                    {
                        result = number > 0 && number <= 1024; //限制长度
                        if (result == false)
                        {
                            Debug.LogError("可变长字符串长度区间:(0,1024]");
                        }
                    }
                    //return Regex.IsMatch(n, @"^[0-9]*[1-9][0-9]*$"); //可变长字符串
                }
            }
        }
        return result;
           
    }

    /// <summary>
	/// 转换为Json
	/// </summary>
	/// <param name="JsonPath">Json文件路径</param>
	/// <param name="Header">表头行数</param>
	public static void ConvertToJson(DataTable sheet, string JsonPath)
    {
        if (sheet == null)
        {
            return;
        }
        //判断数据表内是否存在数据
        if (sheet.Rows.Count < 1)
            return;

        //读取数据表行数和列数
        int rowCount = sheet.Rows.Count;
        int colCount = sheet.Columns.Count;

        //准备一个列表存储整个表的数据
        List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();

        //读取数据
        for (int i = 1; i < rowCount; i++)
        {
            //准备一个字典存储每一行的数据
            Dictionary<string, object> row = new Dictionary<string, object>();
            for (int j = 0; j < colCount; j++)
            {
                //读取第1行数据作为表头字段
                string field = sheet.Rows[0][j].ToString();
                //Key-Value对应
                row[field] = sheet.Rows[i][j];
            }

            //添加到表数据中
            table.Add(row);
        }


        //生成Json字符串
        string json = JsonUtility.ToJson(table);
        //写入文件

        FileEx.SaveFile(JsonPath, json);
    }

    /// <summary>
    /// 转换为CSV
    /// </summary>
    public static void ConvertToCSV(DataTable sheet, string saveFile)
    {
        if (sheet == null)
        {
            return;
        }
        //判断数据表内是否存在数据
        if (sheet.Rows.Count < 1)
            return;

        //读取数据表行数和列数
        int rowCount = sheet.Rows.Count;
        int colCount = sheet.Columns.Count;

        //创建一个StringBuilder存储数据
        StringBuilder stringBuilder = new StringBuilder();

        //读取数据
        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < colCount; j++)
            {
                //使用","分割每一个数值
                stringBuilder.Append(sheet.Rows[i][j] + ",");
            }
            //使用换行符分割每一行
            stringBuilder.Append("\r\n");
        }

        FileEx.SaveFile(saveFile, stringBuilder.ToString());
    }

    /// <summary>
    /// 导出为Xml
    /// </summary>
    public static void ConvertToXml(DataTable sheet, string saveFile)
    {
        if (sheet == null)
        {
            return;
        }

        //读取数据表行数和列数
        int rowCount = sheet.Rows.Count;
        int colCount = sheet.Columns.Count;

        //创建一个StringBuilder存储数据
        StringBuilder stringBuilder = new StringBuilder();
        //创建Xml文件头
        stringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        stringBuilder.Append("\r\n");
        //创建根节点
        stringBuilder.Append("<Table>");
        stringBuilder.Append("\r\n");
        //读取数据
        for (int i = 1; i < rowCount; i++)
        {
            //创建子节点
            stringBuilder.Append("  <Row>");
            stringBuilder.Append("\r\n");
            for (int j = 0; j < colCount; j++)
            {
                stringBuilder.Append("   <" + sheet.Rows[0][j].ToString() + ">");
                stringBuilder.Append(sheet.Rows[i][j].ToString());
                stringBuilder.Append("</" + sheet.Rows[0][j].ToString() + ">");
                stringBuilder.Append("\r\n");
            }
            //使用换行符分割每一行
            stringBuilder.Append("  </Row>");
            stringBuilder.Append("\r\n");
        }
        //闭合标签
        stringBuilder.Append("</Table>");
        //写入文件
        FileEx.SaveFile(saveFile, stringBuilder.ToString());
    }

}

