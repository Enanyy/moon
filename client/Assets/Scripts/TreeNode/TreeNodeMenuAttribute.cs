using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TreeNodeMenuAttribute : Attribute
{
    public string menu { get; private set; }

    public TreeNodeMenuAttribute(string menu)
    {
        this.menu = menu;
    }
}

