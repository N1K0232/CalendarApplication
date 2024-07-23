using System.Reflection;

namespace CalendarApplication.Extensions;

public static class AssemblyExtensions
{
    public static string GetAttribute<T>(this Assembly assembly, Func<T, string> action) where T : Attribute
    {
        var attribute = GetAttribute<T>(assembly);
        return action.Invoke(attribute);
    }

    internal static T GetAttribute<T>(this Assembly assembly) where T : Attribute
    {
        var attribute = (T)Attribute.GetCustomAttribute(assembly, typeof(T));
        return attribute;
    }
}