using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

using log4net;
using ET.WinService.Core.Extension;
using ET.WinService.Core.Utility;

namespace ET.WinService.Core
{
    public class ServerContext
    {
         private static object lockObj = new object();
        private static ServerContext context = null;
        private ILog log = LogManager.GetLogger(typeof(ServerContext));

        //执行程序路径
        public static readonly string ExecutePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        //配置文件路径
        public static readonly string CfgFilePath = Path.Combine(ExecutePath, "ConfigFiles");

        public static readonly string LogFilePath = Path.Combine(ExecutePath, "WinService");

        /// <summary>
        ///参数
        /// </summary>
        public Dictionary<string, string> Parameters
        {
            get;
            set;
        }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName
        {
            get
            {
                return Parameters.GetSafeValue<string>("CompanyName");
            }
            set
            {
                Parameters.SafeAdd("CompanyName", value);
            }
        }

        /// <summary>
        /// 程序版本号
        /// </summary>
        public string AppVersion
        {
            get
            {
                return Parameters.GetSafeValue<string>("AppVersion");
            }
            set
            {
                Parameters.SafeAdd("AppVersion", value);
            }
        }

        /// <summary>
        /// 任务启动时延
        /// </summary>
        public double TaskValidTimeSpan
        {
            get
            {
                return Parameters.GetSafeValue<double>("TaskValidTimeSpan");
            }
            set
            {
                Parameters.SafeAdd("TaskValidTimeSpan", value.ToSafeString());
            }
        }

        /// <summary>
        /// 错误重发次数
        /// </summary>
        public int TrySendTimes
        {
            get
            {
                return Parameters.GetSafeValue<int>("TrySendTimes");
            }
            set
            {
                Parameters.SafeAdd("TrySendTimes", value.ToSafeString());
            }
        }

        /// <summary>
        /// 服务配置文件
        /// </summary>
        public string ServiceCfg
        {
            get
            {
                return Parameters.GetSafeValue<string>("ServiceCfg");
            }
            set
            {
                Parameters.SafeAdd("ServiceCfg", value);
            }
        }

        /// <summary>
        /// 日志配置文件
        /// </summary>
        public string LogCfg
        {
            get
            {
                return Parameters.GetSafeValue<string>("LogCfg");
            }
            set
            {
                Parameters.SafeAdd("LogCfg", value);
            }
        }



        private ServerContext()
        {
            Parameters = new Dictionary<string, string>();
            string cfgFile = Path.Combine(CfgFilePath, "WinService.xml");
            LocalLogger.WriteLocal("WinService cfgFile=" + cfgFile);

            try
            {
                XDocument xDoc = XDocument.Load(cfgFile);
                Parameters = xDoc.Root.Elements().ToDictionary(key => key.Name.LocalName, value => value.Value);

                LocalLogger.WriteLocal("Service cfgFile=" + ServiceConfigFile);
            }
            catch (Exception ex)
            {
                LocalLogger.WriteLocal("Init ServerContext Error:cfgFile=" + cfgFile + ex.Message);
            }
        }

        static ServerContext()
        { }

        public static ServerContext Context
        {
            get
            {
                if (context == null)
                {
                    lock (lockObj)
                    {
                        if (context == null)
                        {
                            context = new ServerContext();
                        }
                    }
                }

                return context;
            }
        }


        //服务配置文件
        public string ServiceConfigFile
        {
            get
            {
                return Path.Combine(CfgFilePath, ServiceCfg);
            }
        }

        //log4net配置文件
        public string LogConfigFile
        {
            get
            {
                return Path.Combine(CfgFilePath, LogCfg);
            }
        }

        /// <summary>
        /// 本地文件日志
        /// </summary>
        public string LocalLogPath
        {
            get
            {
                return Path.Combine(LogFilePath, "LocalLog.txt");
            }
        }
    }
}
