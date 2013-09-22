using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Configuration;
using log4net;

namespace ET.WinService
{
    /// <summary>
    /// 描  述：只是测试之用
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 时  间：2013年1月25日
    /// 修  改：
    /// 原  因：
    /// </summary>
    public partial class FileMonitorService : ServiceBase
    {
        private static ILog log = LogManager.GetLogger(typeof(FileMonitorService));
        private bool servicePaused;

        public FileMonitorService()
        {
            InitializeComponent();
        }

        #region ServiceBase方法实现
        protected override void OnStart(string[] args)
        {
            log.Info("OnStart开始");
            FileSystemWatcher curWatcher = new FileSystemWatcher();

            curWatcher.BeginInit();
            curWatcher.IncludeSubdirectories = true;
            curWatcher.Path =System.Configuration.ConfigurationManager.AppSettings["FileMonitorDirectory"];
            curWatcher.Changed += new FileSystemEventHandler(OnFileChanged);
            curWatcher.Created += new FileSystemEventHandler(OnFileCreated);
            curWatcher.Deleted += new FileSystemEventHandler(OnFileDeleted);
            curWatcher.Renamed += new RenamedEventHandler(OnFileRenamed);
            curWatcher.EnableRaisingEvents = true;
            curWatcher.EndInit();
        }
        protected override void OnStop()
        {
            log.Info("Onstrop");
        }


        protected override void OnPause()
        {
            log.Info("onPause");
            servicePaused = true;
        }

        protected override void OnContinue()
        {
            log.Info("OnContinue");
            servicePaused = false;
        }

        #endregion

        #region 内部方法
        /// <summary>
        /// 描  述：
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月9日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnFileChanged(Object source, FileSystemEventArgs e)
        {

            if (servicePaused == false)
            {
                log.Info("文件修改");
            }

        }



        private void OnFileRenamed(Object source, RenamedEventArgs e)
        {

            if (servicePaused == false)
            {
                log.Info("修改文件名");
            }

        }



        private void OnFileCreated(Object source, FileSystemEventArgs e)
        {

            if (servicePaused == false)
            {
                log.Info("创建文件");
            }

        }



        private void OnFileDeleted(Object source, FileSystemEventArgs e)
        {

            if (servicePaused == false)
            {
                log.Info("删除文件");
            }

        }

        #endregion
    }
}
