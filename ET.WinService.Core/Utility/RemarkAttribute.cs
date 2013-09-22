using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace ET.WinService.Core.Utility
{

    /// <summary>
    /// 枚举Remark
    /// </summary>
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：
    /// 版本号：1.0.0.1
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 日  期：2013年1月16日
    /// 修  改：
    /// 原  因：
    /// ------------------------------------------------------------------------------
    /// </remarks>
    /// <example>
    /// [示例代码在这里写入]
    /// </example>
    public class RemarkAttribute : Attribute
    {
        //缓存
        private static Dictionary<string, string> _cache = new Dictionary<string, string>();
        private string _remark = string.Empty;
        public string Remark
        {
            get
            {
                return this._remark;
            }
            set
            {
                this._remark = value;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="remark"></param>
        public RemarkAttribute(string remark)
        {
            this._remark = remark;
        }

        /// <summary>
        /// 描  述：获取枚举的中文说明
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月16日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumRemark(System.Enum value)
        {
            string key = string.Format("Enum_{0}", value);
            string result;
            if (RemarkAttribute._cache.ContainsKey(key))
            {
                result = RemarkAttribute._cache[key];
            }
            else
            {
                Type type = value.GetType();
                FieldInfo field = type.GetField(value.ToString());
                string text = string.Empty;
                try
                {
                    object[] customAttributes = field.GetCustomAttributes(typeof(RemarkAttribute), false);
                    object[] array = customAttributes;
                    for (int i = 0; i < array.Length; i++)
                    {
                        RemarkAttribute remarkAttribute = (RemarkAttribute)array[i];
                        text = remarkAttribute.Remark;
                    }
                    if (!string.IsNullOrEmpty(text))
                    {
                        RemarkAttribute._cache.Add(key, text);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                result = text;
            }
            return result;
        }

        /// <summary>
        /// 描  述：根据类型获取Remark
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月16日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTypeRemark(Type type)
        {
            string key = string.Format("{0}_Remark", type.Name);
            string result;
            if (RemarkAttribute._cache.ContainsKey(key))
            {
                result = RemarkAttribute._cache[key];
            }
            else
            {
                string text = string.Empty;
                try
                {
                    object[] customAttributes = type.GetCustomAttributes(typeof(RemarkAttribute), false);
                    object[] array = customAttributes;
                    for (int i = 0; i < array.Length; i++)
                    {
                        RemarkAttribute remarkAttribute = (RemarkAttribute)array[i];
                        text = remarkAttribute.Remark;
                    }
                    if (!string.IsNullOrEmpty(text))
                    {
                        RemarkAttribute._cache.Add(key, text);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                result = text;
            }
            return result;
        }
    }
}
