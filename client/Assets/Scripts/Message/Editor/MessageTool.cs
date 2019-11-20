using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;

public class MessageTool
{
    static int MESSAGEID_BEGIN = 1000;

    [MenuItem("Tools/Message/生成协议代码(Assembly)")]
    static void GenerateMessageFromAssembly()
    {
        string codeRegister = "";
        string codeID = "";
        int id = MESSAGEID_BEGIN;
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

                    if (IsMessage(classname))
                    {
                        string ID = GetID(classname);
                        codeID += "\t" + ID + " = " + (id++) + ",\n";
                        codeRegister += "\t\t\tRegister(new " + "MSG_" + classname + "());\n";

                        WriteFile(classname, ID);
                    }

                }

            }
        }
        FileEx.ReplaceContent(Application.dataPath + "/Scripts/Message/MessageManager.cs", "//REGISTER_MESSAGE_BEGIN", "//REGISTER_MESSAGE_END", codeRegister);
        FileEx.ReplaceContent(Application.dataPath + "/Scripts/Message/MessageID.cs", "//MESSAGEID_BEGIN", "//MESSAGEID_END", codeID);
    }
    [MenuItem("Tools/Message/生成协议代码(Proto)")]
    static void GenerateMessageFromProto()
    {
        string dir = Application.dataPath + "/../../public/proto/";
        string[] protos = Directory.GetFiles(dir, "*.proto");

        string codeRegister = "";
        string codeID = "";

        int id = MESSAGEID_BEGIN;

        for (int i = 0; i < protos.Length; ++i)
        {
            string[] lines = File.ReadAllLines(protos[i]);

            for (int j = 0; j < lines.Length; ++j)
            {
                if (lines[j].Contains("message") == false)
                {
                    continue;
                }
                string classname = lines[j].Replace("message", "").Trim();
                if (IsMessage(classname))
                {

                    string ID = GetID(classname);
                    codeID += "\t" + ID +" = " + (id++)+",\n";
                    codeRegister += "\t\tRegister(new " + "MSG_" + classname + "());\n";

                    WriteFile(classname, ID);
                }
            }
        }

        FileEx.ReplaceContent(Application.dataPath + "/Scripts/Message/MessageManager.cs", "//REGISTER_MESSAGE_BEGIN", "//REGISTER_MESSAGE_END", codeRegister);
        FileEx.ReplaceContent(Application.dataPath + "/Scripts/Message/MessageID.cs", "//MESSAGEID_BEGIN", "//MESSAGEID_END", codeID);
    }
    static bool IsMessage(string classname)
    {
        return classname.EndsWith("Request")
                     || classname.EndsWith("Return")
                     || classname.EndsWith("Notify");
    }
        
    static string GetID(string classname)
    {
        string ID = "";
        for (int k = 0; k < classname.Length; k++)
        {
            if ((k > 0 && classname[k] >= 'a' && classname[k] <= 'z') && (k + 1 < classname.Length && classname[k + 1] >= 'A' && classname[k + 1] <= 'Z'))
            {
                ID += classname[k];
                ID += "_";
            }
            else
            {
                ID += classname[k];
            }
        }
        ID = ID.ToUpper();
        return ID;

    }

    static void WriteFile(string classname, string ID)
    {
        if (string.IsNullOrEmpty(classname))
        {
            return;
        }

        string filename = "MSG_" + classname;
        string file = Application.dataPath + "/Scripts/Message/" + filename + ".cs";

        if (File.Exists(file) == false)
        {
            string fm = @"using UnityEngine;
using PBMessage;
public class {filename} : Message<{classname}>
{
    public {filename}() : base(MessageID.{ID})
    {

    }

    public static {filename} Get()
    {
        return MessageManager.Instance.Get<{filename}>(MessageID.{ID});
    }

    protected override void OnRecv(Connection connection)
    {
       
    }
}
";
            string code = fm.Replace("{filename}", filename)
                .Replace("{classname}", classname)
                .Replace("{ID}", ID);
            File.WriteAllText(file, code);
        }

    }
}

