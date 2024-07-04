using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SecondRoundProject.Helpers
{
    public static class ReflectionHelper
    {
        public static List<string> GetPropertyNames<T>()
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            return properties.Select(prop => prop.Name).ToList();
        }
    }

}
