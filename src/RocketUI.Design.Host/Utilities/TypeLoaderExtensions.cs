using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RocketUI.Design.Host.Utilities
{
    public static class TypeLoaderExtensions
    {
        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null) 
                throw new ArgumentNullException(nameof(assembly));

            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
        
        public static Type[] GetTypesWithInterface<T>(this Assembly assembly) 
        {
            var it = typeof (T);
            return assembly.GetLoadableTypes().Where(x => !x.IsInterface && !x.IsAbstract && it.IsAssignableFrom(x)).ToArray();
        }
    }
}