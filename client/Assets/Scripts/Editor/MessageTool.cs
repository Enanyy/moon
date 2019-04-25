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

            if (assembly != null)
            {
                Type[] types = assembly.GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    if (!types[j].IsAbstract)
                    {
                        if (types[j].Namespace =="PBMessage")
                        {
                            string classname = types[j].ToString().Replace("PBMessage.", "");
                            if (classname.EndsWith("Request")
                                || classname.EndsWith("Return")
                                || classname.EndsWith("Notify"))
                            {
                                string filename = "MSG_" + classname;
                                string file = Application.dataPath + "/Scripts/Message/"+ filename + ".cs";

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

                                    string fm = @"
using  PBMessage;
public class {0} : Message<{1}>
{{
    public {2}() : base(MessageID.{3})
    {{

    }}

    protected override void OnMessage()
    {{
       
    }}
}}
";
                                    string code = string.Format(fm, filename, classname, filename, ID);
                                    File.WriteAllText(file, code);
                                  
                                }
                            }
                        }

                    }
                }
            }
        }
    }
}

