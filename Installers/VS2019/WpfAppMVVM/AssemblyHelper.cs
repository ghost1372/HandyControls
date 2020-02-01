using System;
namespace $safeprojectname$
{
    internal class AssemblyHelper
{
    private static readonly string NameSpaceStr = typeof(AssemblyHelper).Assembly.GetName().Name;

    public static object CreateInternalInstance(string className)
    {
        try
        {

            var type = Type.GetType($"{NameSpaceStr}.{className}");
            return type == null ? null : Activator.CreateInstance(type);
        }
        catch
        {
            return null;
        }
    }
}
}