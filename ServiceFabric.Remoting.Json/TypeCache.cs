using System.Collections.Concurrent;

namespace ServiceFabric.Remoting.Json
{
    public static class TypeCache
    {
        private static readonly ConcurrentDictionary<string, Type> TypeNameToTypeCache = new();

        public static Type GetType(string typeName)
        {
            return TypeNameToTypeCache.GetOrAdd(typeName, tn => Type.GetType(tn));
        }
    }
}
