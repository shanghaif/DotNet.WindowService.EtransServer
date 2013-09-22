using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Timers;
using System.Xml.Linq;

using ET.WinService.Core.Extension;

namespace ET.WinService.Core.Task
{
    
    /// <summary>
    /// 任务抽象基类
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
    public abstract class Task
    {
        protected ILog log;

        public virtual string SetLoggerName()
        {
            return typeof(Task).ToString();
        }

        //定时器
        protected Timer timer;

        /// <summary>
        /// 任务实现类
        /// </summary>
        public string ImplClass
        {
            get;
            set;
        }

        /// <summary>
        /// 任务实现类所在程序集
        /// </summary>
        public string Assembly
        {
            get;
            set;
        }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName
        {
            get;
            set;
        }

        /// <summary>
        /// 每个周期发送完毕时线程休眠时间
        /// </summary>
        public int SleepTime
        {
            get;
            set;
        }

        /// <summary>
        /// 上一次执行时间
        /// </summary>
        public DateTime LastExecuteTime
        {
            get;
            set;
        }

        /// <summary>
        /// 是否启动时执行任务
        /// </summary>
        public bool StartupExecute
        {
            get;
            set;
        }

        /// <summary>
        /// 任务参数
        /// </summary>
        public IDictionary<string, string> Parameters
        {
            get;
            set;
        }

        /// <summary>
        /// 累计任务执行次数
        /// </summary>
        public int ExecuteCount
        {
            get;
            set;
        }

        /// <summary>
        /// 是否记录调试信息,默认为true，只有信息很多时才为false
        /// </summary>
        public bool IsLogDebug
        {
            get;
            set;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Task()
        {
            log = LogManager.GetLogger(SetLoggerName());
            Parameters = new Dictionary<string, string>();
            IsLogDebug = true;
        }

        /// <summary>
        /// 初始化任务
        /// </summary>
        /// <param name="?"></param>
        public virtual void Initialize(XElement xelem)
        {
            if (xelem == null)
            {
                log.Error(string.Format("xelem is null!"));
                return;
            }

            ImplClass = xelem.ChildElementValue("class");
            Assembly = xelem.ChildElementValue("assembly");

            TaskName = xelem.ChildElementValue("taskName");
            SleepTime = xelem.ChildElementValue("sleepTime").ToInt(100);
            StartupExecute = xelem.ChildElementValue("startupExecute").SaftCast<bool>(false);
            LastExecuteTime = DateTime.Now.Date;
            ExecuteCount = 0;

            XElement xelemParameters = xelem.Element("parameters");
            if (xelemParameters != null)
                Parameters = xelemParameters.Elements().ToDictionary(k => k.Name.LocalName, e => e.Value);
        }

        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="state"></param>
        public abstract void Start(object state);

        /// <summary>
        /// 是否触发任务
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsTriggerTask()
        {
            return true;
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void Execute(object state)
        { }

        /// <summary>
        /// 结束任务
        /// </summary>
        /// <param name="state"></param>
        public abstract void Stop(object state);
    }
}
