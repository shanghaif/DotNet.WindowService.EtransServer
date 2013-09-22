using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

using log4net;

namespace ET.WinService.Core.Utility
{
    public class Logger
    {
        private ILog log;

        public Logger()
        {
            SetFileName(AssemblyTitle);
            StackTrace st = new StackTrace(1, true);
            StackFrame[] stFrames = st.GetFrames();
            Type t = stFrames[0].GetMethod().DeclaringType;
            log = log4net.LogManager.GetLogger(t);
        }

        public Logger(string name)
        {
            log = log4net.LogManager.GetLogger(name);
        }

        private string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public static string LogPosition()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(2);
            return "Position：" + sf.GetMethod().DeclaringType.ToString() + "[" + sf.GetMethod().Name + "]\r\n";
        }

        private static void SetFileName(string fileName)
        {
            log4net.GlobalContext.Properties["LogFileName"] = fileName + "\\";
            log4net.Config.XmlConfigurator.Configure(new Uri(ServerContext.Context.LogConfigFile));
        }

        public void Debug(string format, params object[] args)
        {
            log.DebugFormat(format, args);
        }

        public void Info(string format, params object[] args)
        {
            log.InfoFormat(LogPosition() + format, args);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            log.ErrorFormat(format, args);
        }

        public void Error(object message)
        {
            log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            log.Error(message, exception);
        }

        public void Fatal(string format, params object[] args)
        {
            log.FatalFormat(format, args);
        }
    }
}
