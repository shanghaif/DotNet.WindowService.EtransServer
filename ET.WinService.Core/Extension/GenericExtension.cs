using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.ComponentModel;

namespace ET.WinService.Core.Extension
{
    /// <summary>
    /// 泛型扩展
    /// </summary>
    public static class GenericExtension
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(GenericExtension));

        public static bool In<T>(this T value, IEnumerable<T> c)
        {
            return c.Any(i => i.Equals(value));
        }

        public static T SaftCast<T>(this object value)
        {
            return SaftCast<T>(value, default(T));
        }

        public static T SaftCast<T>(this object value, T defaultValue)
        {
            if (value == null) return defaultValue;
            try
            {
                if (value is T)
                    return (T)value;

                if (typeof(T) == typeof(Guid))
                    value = new Guid(value.ToSafeString());
                else if (typeof(T).IsEnum)
                    return (T)Enum.Parse(typeof(T), value.ToSafeString(), true);
                else if (typeof(T) == typeof(TimeSpan))
                    return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);
                else if (typeof(T) == typeof(bool))
                {
                    object boolValue = value.ToString().Equals("true", StringComparison.OrdinalIgnoreCase);

                    return (T)boolValue;
                }

                return (T)Convert.ChangeType(value, typeof(T), null);
            }
            catch (Exception ex)
            {
                //log.Error(ex);
            }
            return defaultValue;
        }

        public static T If<T>(this T t, Predicate<T> predicate, Action<T> action) where T : class
        {
            if (t == null) throw new ArgumentNullException();
            if (predicate(t)) action(t);
            return t;
        }

        public static T If<T>(this T t, Predicate<T> predicate, Func<T, T> func) where T : struct
        {
            return predicate(t) ? func(t) : t;
        }

        public static string If(this string s, Predicate<string> predicate, Func<string, string> func)
        {
            return predicate(s) ? func(s) : s;
        }

        public static T If<T>(this T t, Predicate<T> predicate, Action<T> action, Action<T> action2) where T : class
        {
            if (predicate(t))
                action(t);
            else
                action2(t);

            return t;
        }

        public static IDictionary<TKey, TValue> Action<TKey, TValue>(this IDictionary<TKey, TValue> t, Predicate<IDictionary<TKey, TValue>> predicate, Action<IDictionary<TKey, TValue>> action, Action<IDictionary<TKey, TValue>> action2)
        {
            if (predicate(t))
                action(t);
            else
                action2(t);

            return t;
        }

        public static IDictionary<TKey, TValue> SafeAdd<TKey, TValue>(this IDictionary<TKey, TValue> t, TKey key, TValue value)
        {
            if (t == null)
            {
                t = new Dictionary<TKey, TValue>();
            }

            return t.If(o => o.ContainsKey(key), o => o[key] = value, o => o.Add(key, value));
        }

        public static T GetSafeValue<T>(this IDictionary<string, object> dict, string key, T defautValue)
        {
            T value = defautValue;
            object objValue;

            try
            {
                if (dict.TryGetValue(key, out objValue))
                {
                    if (typeof(T) == typeof(bool))
                    {
                        object boolValue = objValue.ToString().Equals("true", StringComparison.OrdinalIgnoreCase);

                        return (T)boolValue;
                    }
                    else if (typeof(T).IsEnum)
                    {
                        return (T)Enum.Parse(typeof(T), objValue.ToString(), true);
                    }
                    else if (typeof(T) == typeof(Guid))
                    {
                        object guid = new Guid(objValue.ToString());

                        return (T)guid;
                    }

                    value = (T)Convert.ChangeType(objValue, typeof(T), null);
                }
            }
            catch (Exception ex)
            {
                value = defautValue;
                ILog log = LogManager.GetLogger(typeof(GenericExtension));
                log.Error(ex);
            }

            if (value == null && typeof(T) == typeof(string)) return (T)Convert.ChangeType(string.Empty, typeof(T), null);

            return value;
        }

        public static T GetSafeValue<T>(this IDictionary<string, object> dict, string key)
        {
            return GetSafeValue<T>(dict, key, default(T));
        }

        public static T GetSafeValue<T>(this IDictionary<string, string> dict, string key)
        {
            T value = default(T);
            string objValue;

            try
            {
                if (dict.TryGetValue(key, out objValue))
                {
                    if (typeof(T) == typeof(bool))
                    {
                        object boolValue = objValue.ToString().Equals("true", StringComparison.OrdinalIgnoreCase);

                        return (T)boolValue;
                    }
                    else if (typeof(T).IsEnum)
                    {
                        return (T)Enum.Parse(typeof(T), objValue.ToString(), true);
                    }
                    else if (typeof(T) == typeof(Guid))
                    {
                        object guid = new Guid(objValue.ToString());

                        return (T)guid;
                    }

                    value = (T)Convert.ChangeType(objValue, typeof(T), null);
                }
            }
            catch (Exception ex)
            {
                value = default(T);
                ILog log = LogManager.GetLogger(typeof(GenericExtension));
                log.Error(ex);
            }

            if (value == null && typeof(T) == typeof(string)) return (T)Convert.ChangeType(string.Empty, typeof(T), null);

            return value;
        }

        public static IList<TValue> SafeAdd<TValue>(this IList<TValue> list, TValue value)
        {
            if (list == null) list = new List<TValue>();

            if (!list.Contains(value)) list.Add(value);

            return list;
        }


        public static void Update<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static T ConvertTo<T>(this IConvertible convertibleValue)
        {
            if (null == convertibleValue)
            {
                return default(T);
            }

            if (!typeof(T).IsGenericType)
            {
                //return (T)Convert.ChangeType(convertibleValue, typeof(T));
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(convertibleValue);
            }
            else
            {
                Type genericTypeDefinition = typeof(T).GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(Nullable<>))
                {
                    return (T)Convert.ChangeType(convertibleValue, Nullable.GetUnderlyingType(typeof(T)));
                }
            }
            throw new InvalidCastException(string.Format("Invalid cast from type \"{0}\" to type \"{1}\".", convertibleValue.GetType().FullName, typeof(T).FullName));
        }
    }
}
