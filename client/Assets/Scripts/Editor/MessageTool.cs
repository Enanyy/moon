using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;

public class MessageTool
{ 
    [MenuItem("Tools/生成协议代码")]
    static void GenMessage()
    {
        string register = "";
        for (int i = 0; i < 4; i++)
        {
            Assembly assembly = null;
            try
            {
                switch (i)
                {
                    case 0:
                        assembly = Assembly.Load("Assembly-CSharp");
                        break;
                    case 1:
                        assembly = Assembly.Load("Assembly-CSharp-firstpass");
                        break;
                    case 2:
                        assembly = Assembly.Load("Assembly-UnityScript");
                        break;
                    case 3:
                        assembly = Assembly.Load("Assembly-UnityScript-firstpass");
                        break;
                }
            }
            catch (Exception)
            {
                assembly = null;
            }

            if (assembly == null)
            {
                continue;
            }
            Type[] types = assembly.GetTypes();
            for (int j = 0; j < types.Length; j++)
            {
                if (types[j].IsAbstract)
                {
                    continue;
                }
                if (types[j].Namespace == "PBMessage")
                {
                    string classname = types[j].ToString().Replace("PBMessage.", "");
                    if (classname.EndsWith("Request")
                        || classname.EndsWith("Return")
                        || classname.EndsWith("Notify"))
                    {
                        string filename = "MSG_" + classname;
                        string file = Application.dataPath + "/Scripts/Message/" + filename + ".cs";

                        if (File.Exists(file) == false)
                        {

                            string ID = "";
                            for (int k = 0; k < classname.Length; k++)
                            {

                                if (k > 0 && classname[k] >= 'A' && classname[k] <= 'Z')
                                {
                                    ID += "_";
                                    ID += classname[k];
                                }
                                else
                                {
                                    ID += classname[k];
                                }
                            }
                            ID = ID.ToUpper();

                            string fm = @"using UnityEngine;
using PBMessage;
public class {0} : Message<{1}>
{{
    public {2}() : base(MessageID.{3})
    {{

    }}

    public static {4} Get()
    {{
        return MessageManager.Instance.Get<{5}>(MessageID.{6});
    }}

    protected override void OnMessage()
    {{
       
    }}
}}
";
                            string code = string.Format(fm, filename, classname, filename, ID, filename, filename, ID);
                            File.WriteAllText(file, code);
                        }
                        register += "\t\tRegister(new " + filename + "());\n";

                    }


                }

            }
        }

        string managerFile = Application.dataPath + "/Scripts/Message/MessageManager.cs";


        string content = File.ReadAllText(managerFile);

        int startIDIndex = content.IndexOf("//REGISTER_MESSAGE_START");
        int endIDIndex = content.IndexOf("//REGISTER_MESSAGE_END");

        string part1 = content.Substring(0, startIDIndex + "//REGISTER_MESSAGE_START".Length + 1);
        string part2 = content.Substring(endIDIndex);

        content = part1 + register + part2;
        StreamWriter writer = new StreamWriter(managerFile);
        writer.Write(content);
        writer.Close();
        writer.Dispose();
    }
}

