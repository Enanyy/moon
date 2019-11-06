using System;
using System.Collections.Generic;
using System.IO;
public static class FileEx
{
    public static void ReplaceContent(string file, string beginFlag, string endFlag, string replace)
    {
        string content = File.ReadAllText(file);
        content.ReplaceEx(beginFlag, endFlag, replace);
        SaveFile(file, content);
    }

    public static void SaveFile(string path, string content)
    {
        string dir = Path.GetDirectoryName(path);
        if (Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory(dir);
        }
        StreamWriter writer = new StreamWriter(path);
        writer.Write(content);
        writer.Close();
        writer.Dispose();
    }
}

