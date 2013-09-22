using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

using log4net;
using ET.WinService.Core;


namespace ET.WinService
{
    [RunInstaller(true)]
    public class EtransInstaller : System.Configuration.Install.Installer
    {
        ILog log = LogManager.GetLogger(typeof(EtransInstaller));
        private System.ServiceProcess.ServiceProcessInstaller spInstaller;

        public EtransInstaller()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // 创建ServiceProcessInstaller对象和ServiceInstaller对象
            this.spInstaller = new System.ServiceProcess.ServiceProcessInstaller();

            // 设定ServiceProcessInstaller对象的帐号、用户名和密码等信息
            this.spInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.spInstaller.Username = null;
            this.spInstaller.Password = null;
            this.Installers.Add(spInstaller);
            //安装服务文件
            SetInstallers();
        }

        #endregion

        /// <summary>
        /// 描  述：一个安装服务里面安装多个服务文件
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月10日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void SetInstallers()
        {
            //获取所有运行的服务名称
            IList<string> serviceNames = ServiceUtil.GetServiceNames(ServerContext.Context.ServiceConfigFile);
            ServiceInstaller[] installers = serviceNames.Select(o => new ServiceInstaller
             {
                 ServiceName = o,
                 Description = "Etrans公司Windows服务",
                 StartType = System.ServiceProcess.ServiceStartMode.Automatic
             }).ToArray();
            this.Installers.AddRange(installers);
        }


    }
}
