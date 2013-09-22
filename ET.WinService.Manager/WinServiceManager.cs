using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.ServiceProcess;
using System.Collections;
using System.Threading;

using ET.WinService.Core;
using ET.WinService.Core.Utility;
using ET.WinService.Core.Extension;
using ET.WinService.Core.Task;

namespace ET.WinService.Manager
{
    public partial class WinServiceManager : Form
    {
        private IList<Service> serviceList;                 //配置文件中的服务列表
        private ServiceController[] winServices;            //Window系统中所有的服务列表
        private Thread workThread = null;                   //多线程

        public WinServiceManager()
        {
            InitializeComponent();
        }

        #region 窗体事件
        private void WinServiceManager_Load(object sender, EventArgs e)
        {
            //获取服务配置文件里的所有服务
            serviceList = GetServiceList(ServerContext.Context.ServiceConfigFile);
            if (serviceList.Count==0) return;
            BindServices();
            string serviceName = this.dgServices.Rows[0].Cells["ServiceName"].Value.ToSafeString();
            BindTasks(serviceName);
            OnLoadWServices(serviceName);
            SetInstallButton();
        }

        /// <summary>
        /// 描  述：安装服务
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月31日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInstall_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确认安装服务?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.No) return;
            try
            {
                lblTrip.Text = "正在安装服务……";
                lblTrip.Visible = true;
                serviceList.Where(o => o.Status == ServiceStatus.Uninstall.ToString() && o.Activate == true)
                    .Update(item =>
                    {
                        workThread = new Thread(new ThreadStart(() =>
                                    {
                                        InstallHelper.Install();

                                        //刷新列表
                                        this.Invoke(new Action(() =>
                                        {
                                            this.WSControllerAgent.ServiceName = item.ServiceName;
                                            StartService();
                                            BindServices();
                                            lblTrip.Visible = false;
                                            SetInstallButton();
                                        }));

                                    }));
                        workThread.IsBackground = true;
                        workThread.Start();
                    });



            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 描  述：卸载服务
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月31日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUninstall_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确认卸载服务?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.No) return;
            try
            {
                lblTrip.Text = "服务正在卸载……";
                lblTrip.Visible = true;
                serviceList.Where(o => o.Status != ServiceStatus.Uninstall.ToString())
                   .Update(item =>
                   {
                       workThread = new Thread(new ThreadStart(() =>
                                    {
                                        InstallHelper.Uninstall();
                                        //刷新列表
                                        this.Invoke(new Action(() =>
                                        {
                                            BindServices();
                                            lblTrip.Visible = false;
                                            this.WSControllerAgent.Close();
                                            SetInstallButton();
                                        }));

                                    }));
                       workThread.IsBackground = true;
                       workThread.Start();

                   });

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void dgServices_MouseClick(object sender, MouseEventArgs e)
        {
            string serviceName = this.dgServices.CurrentRow.Cells["ServiceName"].Value.ToString();
            OnLoadWServices(serviceName);
            BindTasks(serviceName);
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            StartService();
            BindServices();
            SetCommandButton();
        }

        private void ButtonPause_Click(object sender, EventArgs e)
        {
            PauseService();
            BindServices();
            SetCommandButton();
        }

        private void ButtonStop_Click(object sender, EventArgs e)
        {
            StopService();
            BindServices();
            SetCommandButton();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string times = dgTask.CurrentRow.Cells["triggerTimes"].Value.ToString();
            bool isActivate = dgTask.CurrentRow.Cells["activate"].Value.SaftCast<bool>(true);
            TaskEdit taskEdit = new TaskEdit(times, isActivate);
            taskEdit.StartPosition = FormStartPosition.CenterScreen;
            taskEdit.SaveTimes += new SaveChange(taskEdit_SaveTimes);
            taskEdit.ShowDialog();
        }

        private void taskEdit_SaveTimes(ArrayList arr, bool isActivate)
        {
            try
            {
                string serviceName = dgServices.CurrentRow.Cells["ServiceName"].Value.ToString();
                string classValue = dgTask.CurrentRow.Cells["class"].Value.ToString();
                string str = string.Join(",", arr.ToArray());
                dgTask.CurrentRow.Cells["triggerTimes"].Value = str;
                dgTask.CurrentRow.Cells["Activate"].Value = isActivate;
                //保存配置文件
                SaveXml(ServerContext.Context.ServiceConfigFile, serviceName, classValue, arr, isActivate);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void dgTask_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string times = dgTask.CurrentRow.Cells["triggerTimes"].Value.ToString();
            bool isActivate = dgTask.CurrentRow.Cells["activate"].Value.SaftCast<bool>(true);
            TaskEdit taskEdit = new TaskEdit(times, isActivate);
            taskEdit.StartPosition = FormStartPosition.CenterScreen;
            taskEdit.SaveTimes += new SaveChange(taskEdit_SaveTimes);
            taskEdit.ShowDialog();
        }

        #endregion

        #region  内部方法
        /// <summary>
        /// 描  述：绑定服务列表
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年2月1日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void BindServices()
        {
            //更新服务状态
            UpdateServiceStatus();
            dgServices.DataSource = serviceList;
            dgServices.Refresh();
        }

        /// <summary>
        /// 描  述：绑定任务列表
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年2月1日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="serviceName"></param>
        private void BindTasks(string serviceName)
        {
            var list = serviceList.Where(o => o.ServiceName == serviceName);
            if (list.Any())
            {
                dgTask.DataSource = list.First().TaskList;
                dgTask.Refresh();
            }
        }

        /// <summary>
        /// 描  述：更新服务状态
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月31日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void UpdateServiceStatus()
        {
            winServices = ServiceController.GetServices();
            (from t1 in serviceList
             join t2 in winServices
             on t1.ServiceName equals t2.ServiceName into temp
             from t in temp.DefaultIfEmpty()
             select new
             {
                 t1,
                 Status = t == null ? ServiceStatus.Uninstall.ToString() : t.Status.ToString()
             }).Update(item =>
                {
                    item.t1.Status = item.Status;
                });
        }

        private void SetInstallButton()
        {
           var list= serviceList.Where(o => o.Status != ServiceStatus.Uninstall.ToString());
           if (list.Any())
           {
               btnInstall.Enabled = false;
               btnUninstall.Enabled = true;
               ButtonStop.Enabled = true;
           }
           else
           {
               btnInstall.Enabled = true;
               btnUninstall.Enabled = false;
               ButtonPause.Enabled = false;
               ButtonStart.Enabled = false;
               ButtonStop.Enabled = false;
           }
        }

        private void SetCommandButton()
        {
            string strServerStatus = WSControllerAgent.Status.ToString();

            if (strServerStatus == "Running")
            {
                if (WSControllerAgent.CanPauseAndContinue == true)
                {
                    ButtonPause.Enabled = true;
                }
                else
                {
                    ButtonPause.Enabled = false;
                }

                ButtonStop.Enabled = true;
                ButtonStart.Enabled = false;
            }
            else if (strServerStatus == "Paused")
            {
                ButtonStart.Enabled = true;
                ButtonPause.Enabled = false;
                ButtonStop.Enabled = true;
            }
            else if (strServerStatus == "Stopped")
            {
                ButtonStart.Enabled = true;
                ButtonPause.Enabled = false;
                ButtonStop.Enabled = false;
            }

        }

        private void OnLoadWServices(string strServiceName)
        {
            try
            {
                ServiceController[] AvailableServices = ServiceController.GetServices();

                if (strServiceName != "")
                {
                    foreach (ServiceController AvailableService in AvailableServices)
                    {
                        if (AvailableService.ServiceName == strServiceName)
                        {
                            this.WSControllerAgent.ServiceName = strServiceName;
                            this.SetCommandButton();
                            return;
                        }
                    }

                }
            }
            catch
            {
                MessageBox.Show("服务没有安装");
            }
        }

        #endregion

        #region 配置文件操作
        private IList<string> GetServiceNames(string serviceFile)
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

                MessageBox.Show("获取服务配置文件的服务名称出错！");
            }
            return serviceNames;
        }

        private IList<Service> GetServiceList(string serviceFile)
        {
            IList<Service> serviceList = new List<Service>();
            try
            {
                XDocument xDoc = XDocument.Load(serviceFile);

                //获取所有服务
                foreach (XElement xelem in xDoc.Descendants("service"))
                {
                    IList<TaskModel> tasks = new List<TaskModel>();
                    if (xelem.HasElements)
                    {
                        //获取所有任务
                        foreach (XElement task in xelem.Descendants("task"))
                        {
                            tasks.Add(new TaskModel()
                            {
                                Class = task.Element("class").GetSafeValue(),
                                Assembly = task.Element("assembly").GetSafeValue(),
                                Interval = task.Element("inerval").GetSafeValue().SaftCast<TimeSpan>(),
                                Activate = task.Element("activate").GetSafeValue().SaftCast<bool>(true),
                                SleepTime = task.Element("sleepTime").GetSafeValue().SaftCast<int>(0),
                                StartTime = task.Element("startTime").GetSafeValue().SaftCast<DateTime>(),
                                TaskName = task.Element("taskName").GetSafeValue(),
                                triggerTimes = GetTriggerTimes(task)
                            });
                        }
                    }
                    serviceList.Add(new Service
                    {
                        ServiceName = xelem.AttributeValue("name"),
                        Activate = xelem.AttributeValue("activate").SaftCast<bool>(true),
                        Status = "",
                        TaskList = tasks
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return serviceList;
        }

        private string GetTriggerTimes(XElement task)
        {
            XElement xlem = task.Element("triggerTimes");
            if (xlem == null) return null;
            IList<TimeSpan> timeSpans = new List<TimeSpan>();
            if (xlem.HasElements)
            {
                foreach (var item in xlem.Elements())
                {
                    timeSpans.Add(item.GetSafeValue().SaftCast<TimeSpan>());
                }
            }
            return string.Join(",", timeSpans);
        }

        /// <summary>
        /// 描  述：保存配置文件
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年2月4日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="serviceFile"></param>
        /// <param name="serviceName"></param>
        /// <param name="classValue"></param>
        /// <param name="arr"></param>
        /// <param name="isActivate"></param>
        private void SaveXml(string serviceFile, string serviceName, string classValue, ArrayList arr, bool isActivate)
        {
            XDocument xDoc = XDocument.Load(serviceFile);
            var xelem = xDoc.Descendants("service").Where(o => o.AttributeValue("name") == serviceName);
            var task = xelem.Descendants("task").Where(o => o.Element("class").Value == classValue);
            var times = task.First().Element("triggerTimes");
            times.Elements().Remove();
            foreach (var item in arr)
            {
                times.Add(new XElement("triggerTime", item.ToString()));
            }
            task.First().SetElementValue("activate", isActivate.ToString());
            xDoc.Save(serviceFile);
        }
        #endregion

        #region 服务操作
        private void StartService()
        {
            //检查服务状态
            if (WSControllerAgent.Status.ToString() == "Paused")
            {
                WSControllerAgent.Continue();
            }
            else if (WSControllerAgent.Status.ToString() == "Stopped")
            {
                ServiceController[] ParentServices = WSControllerAgent.ServicesDependedOn;
                try
                {
                    if (ParentServices.Length >= 1)
                    {
                        foreach (ServiceController ParentService in ParentServices)
                        {
                            //确保父服务正在运行，或至少暂停.
                            if (ParentService.Status.ToString() != "Running" || ParentService.Status.ToString() != "Paused")
                            {

                                if (MessageBox.Show("This service is required. Would you like to also start this service?\n" + ParentService.DisplayName, "Required Service", MessageBoxButtons.YesNo).ToString() == "Yes")
                                {
                                    ParentService.Start();
                                    ParentService.WaitForStatus(ServiceControllerStatus.Running);
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                    }
                    WSControllerAgent.Start();
                    WSControllerAgent.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running);
                }

                catch (Exception ex)
                {

                    MessageBox.Show("Error Occured: " + ex.Message.ToString());

                }
            }
        }

        private void StopService()
        {
            //检查服务是否可以停止.
            if (WSControllerAgent.CanStop == true)
            {
                ServiceController[] DependentServices = WSControllerAgent.DependentServices;

                if (DependentServices.Length >= 1)
                {
                    foreach (ServiceController DependentService in DependentServices)
                    {
                        //确保相关的服务是不是已经停止了
                        if (DependentService.Status.ToString() != "Stopped")
                        {
                            if (MessageBox.Show("Would you like to also stop this dependent service?\n"
                                + DependentService.DisplayName
                                , "Dependent Service"
                                , MessageBoxButtons.YesNo).ToString() == "Yes")
                            {
                                DependentService.Stop();
                                DependentService.WaitForStatus(ServiceControllerStatus.Stopped);
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
                if (WSControllerAgent.Status.ToString() == "Running" || WSControllerAgent.Status.ToString() == "Paused")
                {
                    WSControllerAgent.Stop();
                }
                WSControllerAgent.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped);

            }

        }

        private void PauseService()
        {
            //检查服务是否可以暂停和继续
            if (WSControllerAgent.CanPauseAndContinue == true)
            {
                if (WSControllerAgent.Status.ToString() == "Running")
                {
                    WSControllerAgent.Pause();
                }

                WSControllerAgent.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Paused);
            }
        }
        #endregion



    }
}
