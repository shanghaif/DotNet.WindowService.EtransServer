using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Windows.Forms;

using ET.WinService.Core;
using ET.WinService.Core.Utility;

namespace ET.WinService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            try
            {
                ///CreateSetupBatFile();
                LocalLogger.WriteLocal("Begin Run WinService!");
                log4net.Config.XmlConfigurator.Configure(new Uri(ServerContext.Context.LogConfigFile));
                InstallServices();
                LocalLogger.WriteLocal("Run WinService Complete!");
                
            }
            catch (Exception ex)
            {
                LocalLogger.WriteLocal(ex.Message);
                LocalLogger.WriteEvenLog(ex.Message);
            }
        }

        private static void InitLog4net()
        {
            if (log4net.GlobalContext.Properties["LogFileName"] == null)
            {
                log4net.GlobalContext.Properties["LogFileName"] = string.Empty;
            }
            log4net.Config.XmlConfigurator.Configure(new Uri(ServerContext.Context.LogConfigFile));
        }

        private static void InstallServices()
        {
            IList<string> serviceNames = ServiceUtil.GetServiceNames(ServerContext.Context.ServiceConfigFile);
            ServiceController[] services = ServiceController.GetServices();
            foreach (string serviceName in serviceNames)
            {
                bool isInstalled = false;              //是否已经安装
                bool serviceStarting = false;           //是否服务正在启动
             
                foreach (ServiceController service in services)
                {
                    if (service.ServiceName.Equals(serviceName))
                    {
                        isInstalled = true;
                        if (service.Status == ServiceControllerStatus.StartPending)
                        {
                            // If the status is StartPending then the service was started via the SCM             
                            serviceStarting = true;
                        }
                        break;
                    }
                }

                if (!serviceStarting)
                {
                    if (isInstalled == true)
                    {
                        // Thanks to PIEBALDconsult's Concern V2.0
                        DialogResult dr = new DialogResult();
                        dr = MessageBox.Show("Do you REALLY like to uninstall the " + serviceName + "?", "Danger", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dr == DialogResult.Yes)
                        {
                            InstallHelper.Uninstall();
                            MessageBox.Show("Successfully uninstalled the " + serviceName, "Status",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        DialogResult dr = new DialogResult();
                        dr = MessageBox.Show("Do you REALLY like to install the " + serviceName + "?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dr == DialogResult.Yes)
                        {
                            InstallHelper.Install();
                            MessageBox.Show("Successfully installed the " + serviceName, "Status",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    ServiceBase.Run(ServiceUtil.LoadServices(ServerContext.Context.ServiceConfigFile));
                }
            }
        }

        private static void CreateSetupBatFile()
        {
            try
            {
                string setupFilePath = Path.Combine(ServerContext.ExecutePath, "setup.debug.bat");
                string unSetupFilePath = Path.Combine(ServerContext.ExecutePath, "unsetup.debug.bat");
                string dotNetVersion = Environment.Version.ToString();
                IList<string> serviceName = ServiceUtil.GetServiceNames(ServerContext.Context.ServiceConfigFile);
  
                FileHelper.CreateFile(setupFilePath);
                FileHelper.CreateFile(unSetupFilePath);

                StringBuilder str=new StringBuilder();
                str.Append(@"%SystemRoot%\Microsoft.NET\Framework\v");
                str.Append(dotNetVersion.Substring(0,dotNetVersion.LastIndexOf('.')));
                str.Append(@"\InstallUtil.exe ET.WinService.exe");
                str.AppendLine();
                foreach (string name in serviceName)
                {
                    str.AppendLine(string.Format(@"net start {0}",name));
                }
                str.AppendLine("@pause");
                FileHelper.WriteText(setupFilePath, str.ToString());
                str.Clear();
                str.Append(@"%SystemRoot%\Microsoft.NET\Framework\v");
                str.Append(dotNetVersion.Substring(0, dotNetVersion.LastIndexOf('.')));
                str.Append(@"\InstallUtil.exe /u ET.WinService.exe");
                str.AppendLine();
                str.AppendLine("@pause");
                FileHelper.WriteText(unSetupFilePath, str.ToString());
            }
            catch (Exception ex)
            {
                LocalLogger.WriteLocal(ex.Message);
            }

        }
    }
}
