using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        //写入文件
        using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            using (TextWriter writer = new StreamWriter(fileStream, Encoding.GetEncoding("utf-8")))
            {
                writer.Write(content);
            }
        }
    }
}

