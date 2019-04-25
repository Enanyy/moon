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
                                Debug.Log(classname);
                                string file = Application.dataPath + "/Scripts/Message/MSG_"+classname+".cs";

                                if (File.Exists(file) == false)
                                {

                                    string name = "";
                                    for (int k = 0; k < classname.Length; k++)
                                    {

                                        if (k > 0 && classname[k] >= 'A' && classname[k] <= 'Z')
                                        {
                                            name += "_";
                                            name += classname[k];
                                        }
                                        else
                                        {
                                            name += classname[k];
                                        }
                                    }

                                    ///Debug.Log(name.ToUpper());
                                }
                            }
                        }

                    }
                }
            }
        }
    }
}

