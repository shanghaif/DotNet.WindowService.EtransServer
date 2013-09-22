#region Description
/*==============================================================================
 *  Copyright (c) hgq.  All rights reserved.
 * ===============================================================================
 * This code and information is provided "as is" without warranty of any kind,
 * either expressed or implied, including but not limited to the implied warranties
 * of merchantability and fitness for a particular purpose.
 * ===============================================================================
 * This code is only for study
 * ==============================================================================*/
#endregion
using System;
using System.IO;
using System.Xml.Serialization;

namespace ET.WinService.Core.Utility
{

    /// <summary>
    /// xml辅助类
    /// </summary>
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：
    /// 版本号：1.0.0.1
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 日  期：2013年1月11日
    /// 修  改：
    /// 原  因：
    /// ------------------------------------------------------------------------------
    /// </remarks>
    /// <example>
    /// [示例代码在这里写入]
    /// </example>
    public class XmlUtil
    {
        private static XmlSerializer CreateXMLSerializer(Type typeToSerialise, string pStrXMLRootName)
        {
            XmlSerializer serializer;
            if (pStrXMLRootName != null && pStrXMLRootName != string.Empty)
            {
                serializer = new XmlSerializer(typeToSerialise, pStrXMLRootName);
            }
            else
            {
                serializer = new XmlSerializer(typeToSerialise);
            }
            return serializer;
        }

        private static XmlRootAttribute CreateXMLRootAttribute(string psRootName)
        {
            // Create an XmlRootAttribute overloaded constructer 
            //and set its namespace.
            XmlRootAttribute newXmlRootAttribute = null;
            if (psRootName != null && psRootName != string.Empty)
            {
                newXmlRootAttribute = new XmlRootAttribute(psRootName);
            }
            return newXmlRootAttribute;
        }

        public static string SerializeToXML(object obj)
        {
            StringWriter writer = new StringWriter();
            Type type = obj.GetType();
            CreateXMLSerializer(type, null).Serialize(writer, obj);
            writer.Close();
            return writer.ToString();
        }

        public static string SerializeToXML(object obj, string pStrXMLRootName)
        {
            StringWriter writer = new StringWriter();
            CreateXMLSerializer(obj.GetType(), pStrXMLRootName).Serialize(writer, obj);
            writer.Close();
            return writer.ToString();
        }

        public static T Deserialise<T>(string xml)
        {
            StringReader rd = new StringReader(xml);
            T deserialisedObj;
            XmlSerializer serialiser = CreateXMLSerializer(typeof(T), null);
            deserialisedObj = (T)serialiser.Deserialize(rd);
            return deserialisedObj;
        }

        public static T Deserialise<T>(Stream stream)
        {
            T deserialisedObj;
            XmlSerializer serialiser = CreateXMLSerializer(typeof(T), null);
            deserialisedObj = (T)serialiser.Deserialize(stream);
            return deserialisedObj;
        }

        public static T Deserialise<T>(string pStrXMLRootName, Stream stream)
        {
            T deserialisedObj;
            XmlSerializer serialiser = CreateXMLSerializer(typeof(T), pStrXMLRootName);
            deserialisedObj = (T)serialiser.Deserialize(stream);
            return deserialisedObj;
        }

        public static T Deserialise<T>(string pStrXMLRootName, string xml)
        {
            StringReader rd = new StringReader(xml);
            T deserialisedObj;
            XmlSerializer serialiser = CreateXMLSerializer(typeof(T), pStrXMLRootName);
            deserialisedObj = (T)serialiser.Deserialize(rd);
            return deserialisedObj;
        }
    }
}
