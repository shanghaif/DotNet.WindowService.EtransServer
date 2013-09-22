using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace ET.WinService.Core.Extension
{
    public static class ObjectExtension
    {
        /// <summary>
        /// 字符串转换为int,出错时返回defaultValue
        /// </summary>
        /// <param name="objValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(this object objValue, int defaultValue)
        {
            if (objValue == null)
                return defaultValue;

            return objValue.ToString().ToInt();
        }

        /// <summary>
        /// 字符串转换为long,出错时返回0
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public static int ToInt(this object objValue)
        {
            return ToInt(objValue, 0);
        }

        /// <summary>
        /// 字符串转换为long,出错时返回defaultValue
        /// </summary>
        /// <param name="objValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ToLong(this object objValue, long defaultValue)
        {
            if (objValue == null)
                return defaultValue;

            return objValue.ToString().ToLong();
        }

        /// <summary>
        /// 字符串转换为int,出错时返回0
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public static long ToLong(this object objValue)
        {
            return ToLong(objValue, 0);
        }

        /// <summary>
        /// 字符串转换为double,出错时返回defaultValue
        /// </summary>
        /// <param name="objValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double ToDouble(this object objValue, double defaultValue)
        {
            if (objValue == null)
                return defaultValue;

            return objValue.ToString().ToDouble();
        }

        /// <summary>
        /// 字符串转换为double,出错时返回0
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public static double ToDouble(this object objValue)
        {
            return ToDouble(objValue, 0.00);
        }

        public static string ToSafeString(this object objValue)
        {
            return ToSafeString(objValue, string.Empty);
        }

        public static string ToSafeString(this object objValue, string defaultValue)
        {
            string value = defaultValue;

            if (objValue != null) value = objValue.ToString();

            return value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static PropertyInfo FindProperty(this Type type, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return null;

            return type.GetProperties(BindingFlags.Public).FirstOrDefault(o => o.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="objValue"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static TValue GetPropertyValue<TValue>(this object objValue, string propertyName)
        {
            if (objValue == null) return default(TValue);

            Type type = objValue.GetType();
            if (type.IsClass)
            {
                PropertyInfo property = type.FindProperty(propertyName);
                if (property == null) return default(TValue);

                object value = property.GetValue(objValue, null);

                if (value == null) return default(TValue);

                if (typeof(TValue) == typeof(Guid)) value = new Guid(value.ToSafeString());

                try
                {
                    return (TValue)Convert.ChangeType(value, typeof(TValue), null);
                }
                catch (Exception ex)
                {
                    log4net.ILog log = log4net.LogManager.GetLogger(typeof(ObjectExtension));
                    log.Error(ex);
                }
            }

            return default(TValue);
        }

        public static T To<T>(this object objValue)
        {
            return To<T>(objValue, default(T));
        }

        public static T To<T>(this object objValue, T defaultValue)
        {
            if (objValue == null) return defaultValue;

            object value = objValue;

            if (typeof(T) == typeof(Guid)) value = new Guid(value.ToSafeString());

            try
            {
                return (T)Convert.ChangeType(value, typeof(T), null);
            }
            catch (Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(typeof(ObjectExtension));
                log.Error(ex);
            }

            return default(T);
        }
    }
}
