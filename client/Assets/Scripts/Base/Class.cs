using System;
using System.Collections.Generic;
using System.Reflection;

public class Class
{
    public static List<Type> GetTypes(Type attribute, bool inherit)
    {
        List<Type> list = new List<Type>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for(int i = 0; i < assemblies.Length; ++i)
        {
            var assembly = assemblies[i];
           
            var types = assembly.GetTypes();
            for(int j = 0;j <types.Length; ++j)
            {
                Type type = types[j];
                if(type.IsDefined(attribute, inherit))
                {
                    list.Add(type);
                }
            }
        }

        return list;
    }
    public static List<Type> GetTypes(Assembly assembly , Type attribute, bool inherit)
    {
        List<Type> list = new List<Type>();
        var types = assembly.GetTypes();
        for (int j = 0; j < types.Length; ++j)
        {
            Type type = types[j];
            if (type.IsDefined(attribute, inherit))
            {
                list.Add(type);
            }
        }
        return list;
    }
}

