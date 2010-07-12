using System;
using System.Reflection;

namespace Magecrawl.Utilities
{
    public static class TypeLocator
    {
        public static Type GetTypeToMake(Type typeInAssembly, string space, string typeName)
        {
            Assembly thisAssembly = typeInAssembly.Assembly;
            Type type = thisAssembly.GetType(space + "." + typeName);
            if (type != null)
                return type;
            else
                throw new ArgumentException("GetTypeToMake - don't know how to make: " + typeName);
        }
    }
}
