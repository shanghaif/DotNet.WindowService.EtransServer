using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Xml.Linq;
using System.Reflection;

using log4net;
using ET.WinService.Core.Extension;

namespace ET.WinService
{

    /// <summary>
    /// 服务配置文件辅助类
    /// </summary>
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：
    /// 版本号：1.0.0.1
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 日  期：2013年1月18日
    /// 修  改：
    /// 原  因：
    /// ------------------------------------------------------------------------------
    /// </remarks>
    /// <example>
    /// [示例代码在这里写入]
    /// </example>
    public class ServiceUtil
    {
        private static ILog log = LogManager.GetLogger(typeof(ServiceUtil));

        /// <summary>
        /// 描  述：获取所有服务名称
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月18日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="serviceFile"></param>
        /// <returns></returns>
        public static IList<string> GetServiceNames(string serviceFile)
        {
            IList<string> serviceNames = new List<string>();
            try
            {
                XDocument xDoc = XDocument.Load(serviceFile);

                foreach (XElement xelem in xDoc.Descendants("service"))
                {
                    string name = xelem.AttributeValue("name");
                    string activate = xelem.AttributeValue("activate");
                    if (!string.IsNullOrEmpty(activate) && !activate.SaftCast<bool>(true)) continue;
                    serviceNames.SafeAdd(name);
                }
            }
            catch (Exception ex)
            {

                log.Error("获取服务配置文件的服务名称出错！", ex);
            }
            return serviceNames;
        }

        /// <summary>
        /// 描  述：获取服务下的任务xml配置文档
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月18日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="serviceFile"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static XElement GetXElementByServiceName(string serviceFile, string serviceName)
        {
            try
            {
                XDocument xDoc = XDocument.Load(serviceFile);

                foreach (XElement xelem in xDoc.Descendants("service"))
                {
                    string name = xelem.AttributeValue("name");
                    string activate = xelem.AttributeValue("activate");
                    if (!string.IsNullOrEmpty(activate) && !activate.SaftCast<bool>(true)) continue;
                    if (name == serviceName && xelem.HasElements)
                    {
                        return xelem;
                    }
                }
            }
            catch (Exception ex)
            {

                log.Error("获取服务配置文件的服务名称出错！", ex);
            }
            return null;
        }

        /// <summary>
        /// 描  述：加载启动服务列表
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月18日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="serviceFile">服务配置文件</param>
        /// <returns></returns>
        public static ServiceBase[] LoadServices(string serviceFile)
        {
            IList<ServiceBase> services = new List<ServiceBase>();
            try
            {
                XDocument xDoc = XDocument.Load(serviceFile);

                foreach (XElement xelem in xDoc.Descendants("service"))
                {
                    string assembly = xelem.AttributeValue("assembly");
                    string implClass = xelem.AttributeValue("class");
                    string activate = xelem.AttributeValue("activate");

                    if (!string.IsNullOrEmpty(activate) && !activate.SaftCast<bool>(true)) continue;
                    Type type = Assembly.Load(assembly).GetType(implClass);
                    //判断是否有子节点，即是否有任务配置
                    if (xelem.HasElements)
                    {
                        services.Add(Activator.CreateInstance(type, xelem) as ServiceBase);
                    }
                    else
                    {
                        services.Add(Activator.CreateInstance(type) as ServiceBase);
                    }
                    log.Info(string.Format("加载服务：{0}", implClass));
                }
            }
            catch (Exception ex)
            {
                log.Error("加载服务配置文件出错！", ex);
            }

            return services.ToArray();
        }
    }
}
