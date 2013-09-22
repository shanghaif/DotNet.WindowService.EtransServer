using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;

using log4net;
using ET.WinService.Core.Extension;

namespace ET.WinService.Core.Task
{
    
    /// <summary>
    /// 任务管理类
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
    public class TaskManager
    {
        private ILog log = LogManager.GetLogger(typeof(LogManager));
        //任务列表
        private IList<Task> tasks;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="xelem">任务结点信息</param>
        public TaskManager(IList<XElement> xelemTasks)
        {
            tasks = xelemTasks.Where(e => string.IsNullOrEmpty(e.ChildElementValue("activate")) || e.ChildElementValue("activate").IgnoreCaseEquals("true")).Select(o => TaskFactory.CreateTask(o)).ToList();
        }

        /// <summary>
        /// 启动任务列表
        /// </summary>
        public void Start()
        {
            foreach (var task in tasks)
            {
                try
                {
                    task.Start(null);
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("启动任务{0}出错", task.TaskName), ex);
                }
            }
        }


        /// <summary>
        /// 停止任务列表
        /// </summary>
        public void Stop()
        {
            foreach (var task in tasks)
            {
                try
                {
                    task.Stop(null);
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("停止任务{0}出错", task.TaskName), ex);
                }
            }
        }
    }

    
    /// <summary>
    /// 任务创建工厂类
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
    public static class TaskFactory
    {
        public static Task CreateTask(XElement xelemTask)
        {
            string assembly = string.Empty;
            string implClass = string.Empty;
            try
            {
                assembly = xelemTask.ChildElementValue("assembly");
                implClass = xelemTask.ChildElementValue("class");

                Type type = Assembly.Load(assembly).GetType(implClass);

                return Activator.CreateInstance(type, xelemTask) as Task;
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(TaskFactory));
                log.Error(string.Format("创建任务对象{0}.{1}出错！", assembly, implClass), ex);
            }

            return null;
        }
    }
}
