using System.Reflection;

public static class Extensions
{
    public static string GetFullName(this MethodBase methodBase) => $"{methodBase.ReflectedType.Name} {methodBase.Name}";
}