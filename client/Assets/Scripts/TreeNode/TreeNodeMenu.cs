using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class TreeNodeMenu : Attribute
{
    public string menu { get; private set; }

    public TreeNodeMenu(string menu)
    {
        this.menu = menu;
    }
}

