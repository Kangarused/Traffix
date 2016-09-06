using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;

namespace Traffix.Common.Utils.Extensions
{
    public static class EnumExtensions
    {
        private static readonly ConcurrentDictionary<string, string> DescriptionCache = new ConcurrentDictionary<string, string>();
        private static readonly object Locker = new object();

        public static string Description(this Enum value)
        {
            string output;

            if (!DescriptionCache.TryGetValue(value.GetType() + value.ToString(), out output))
            {
                lock (Locker)
                {
                    if (!DescriptionCache.TryGetValue(value.GetType() + value.ToString(), out output))
                    {
                        FieldInfo fi = value.GetType().GetField(value.ToString());
                        var attributes =
                            (DescriptionAttribute[]) fi.GetCustomAttributes(typeof (DescriptionAttribute), false);
                        output = attributes.Length > 0 ? attributes[0].Description : value.ToString();
                        DescriptionCache.TryAdd(value.GetType() + value.ToString(), output);
                    }
                }
            }
            return output;
        }

        public static T EnumValueOf<T>(this string value)
        {
            Type enumType = typeof(T);
            string[] names = Enum.GetNames(enumType);
            foreach (string name in names)
            {
                if (((Enum)Enum.Parse(enumType, name)).Description().Equals(value))
                {
                    return (T)Enum.Parse(enumType, name);
                }
            }

            throw new ArgumentException("The string is not a description or value of the specified enum.");
        }

        public static T EnumParse<T>(this string key)
        {
           return (T) Enum.Parse(typeof (T), key);
        }
    }
}

