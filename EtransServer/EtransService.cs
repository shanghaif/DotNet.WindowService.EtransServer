using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Xml.Linq;

using log4net;
using ET.WinService.Core.Task;
using ET.WinService.Core;

namespace ET.WinService
{
    partial class EtransService : ServiceBase
    {
        private ILog log = LogManager.GetLogger(typeof(EtransService));
        private TaskManager taskManager;

        public EtransService()
        {
            InitializeComponent();
        }

        public EtransService(XElement xelem)
        {
            try
            {
                InitializeComponent();
                taskManager = new TaskManager(xelem.Elements("task").ToList());
            }
            catch (Exception ex)
            {
                log.Error("服务初始化时出错：", ex);
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                //服务重启时会从这里开始
                if (taskManager == null)
                {
                    XElement xelem = ServiceUtil.GetXElementByServiceName(ServerContext.Context.ServiceConfigFile, "EtransService");
                    taskManager = new TaskManager(xelem.Elements("task").ToList());
                }
                taskManager.Start();
            }
            catch (Exception ex)
            {
                log.Error("OnStart Error", ex);
            }
        }

        protected override void OnStop()
        {

            try
            {
                if (taskManager != null)
                {
                    taskManager.Stop();
                    taskManager = null;
                }
            }
            catch (Exception ex)
            {
                log.Error("OnStop Error", ex);
            }
        }
    }
}
