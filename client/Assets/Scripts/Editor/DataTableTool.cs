using UnityEngine;
using UnityEditor;
using System.IO;
public static class DataTableTool 
{
    [MenuItem("Tools/导出数据表代码")]
    static void GenDataTable()
    {
        
        TextAsset text = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/r/database/data.bytes");
        if(text)
        {
            if(SQLite.Instance.Open(text.bytes))
            {
                SQLiteDataTable table = SQLite.Instance.GetDataTables();
                if(table!=null)
                {
                    string IDString = "";
                    string registerString = "";
                    while(table.Read())
                    {
                        string tableName = table.GetByColumnName("name","");
                        string fileName = tableName.Replace("TB", "Table");

                        IDString += "\t" + tableName + ",\n";

                        registerString += string.Format("\t\t\tRegister(new {0}());\n", fileName);

                        string path = string.Format("{0}/Scripts/Data/Tables/{1}.cs", Application.dataPath, fileName);
                        if (File.Exists(path) == false)
                        {
                            SQLiteDataTable info = SQLite.Instance.GetTableInfo(tableName);

                            string code = @"public class {0} : IDataTable
{{
    public DataTableID name
    {{
        get {{ return DataTableID.{1}; }}
    }}

    //public readonly Dictionary<int, {2}> data = new Dictionary<int, {3}>();

    public void Read(SQLiteDataTable table)
    {{
        while (table.Read())
        {{
           {4}
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
                                string tb = "using System.Collections.Generic;\n\npublic class " + tableName + " { \n";
                                string read = "/*\t"+tableName+ " o = new "+tableName+"();\n";
                                
                                while (info.Read())
                                {
                                    string columnName = info.GetByColumnName("name", "");
                                    string columnType = GetType( info.GetByColumnName("type", ""));

                                    tb += "\tpublic " + columnType + " " + columnName + ";\n";
                                    read += "\t\t\to." + columnName + " = table.GetByColumnName(\"" + columnName + "\","+(columnType=="string"?"\"\"":"0")+");\n";
                                }
                                read += "\t\t\tdata.Add(o.id,o);*/\n";
                                tb += "}";

                                code = string.Format(code, 
                                    fileName,
                                    tableName,
                                    tableName,
                                    tableName,
                                    read,
                                    tableName,
                                    fileName,
                                    tableName
                                    );
                                code = tb +"\n\n"+ code;
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
                Debug.LogError("Can't open database!");

            }
        }
        else
        {
            Debug.LogError("Can't load database!");
        }
    }

    static string GetType(string type)
    {
        switch(type.ToUpper())
        {
            case "INT":return "int";
            case "DECIMAL":return "float";

            default:return "string";
                
        }
    }
}