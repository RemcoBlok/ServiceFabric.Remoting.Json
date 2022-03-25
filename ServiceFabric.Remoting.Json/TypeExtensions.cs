using System;
using System.Reflection;

namespace ServiceFabric.Remoting.Json
{
    public static class TypeExtensions
    {
        public static string GetSimpleAssemblyQualifiedName(this Type type)
        {
            if (type.FullName == null)
            {
                return null;
            }

            return Assembly.CreateQualifiedName(AssemblyNameCache.GetName(type.Assembly).Name, type.FullName);
        }
    }
}
