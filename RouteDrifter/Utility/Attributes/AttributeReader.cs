using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

public static class AttributeReader
{
    public static IEnumerable<MethodInfo> ReadMethods<T>(Type type) where T : Attribute
    {
        var methodInfos = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
        return methodInfos.Where(methodInfo => methodInfo.GetCustomAttributes(typeof(T), true).Any());
    }

    public static T GetMethodAttribute<T>(MethodInfo methodInfo) where T : Attribute
    {
        var list = methodInfo.GetCustomAttributes(true).Where(m => m.GetType() == typeof(T));
        int c = list.Count();
        var r = list.ElementAt(0);
        return c > 0 ? (T)r : null;
    }

}