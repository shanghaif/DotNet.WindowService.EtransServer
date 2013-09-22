using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading;

using ET.WinService.Core.Extension;

namespace ET.WinService.Core.Task
{
    /// <summary>
    /// 描  述：多线程任务
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 时  间：2013年1月11日
    /// 修  改：
    /// 原  因：
    /// </summary>
    public abstract class ThreadTask : Task
    {
        private Thread workThread = null;
        private object lockObj = new object();

        /// <summary>
        ///构造函数
        /// </summary>
        /// <param name="xelem">配置结点</param>
        public ThreadTask(XElement xelem)
        {
            Initialize(xelem);
        }

        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="state"></param>
        public override void Start(object state)
        {
            log.Info(string.Format("开始启动-{0}任务！", TaskName));

            workThread = new Thread(new ParameterizedThreadStart(ThreadStart));
            workThread.IsBackground = true;
            workThread.Start(state);

            log.Info(string.Format("已经启动-{0}任务！", TaskName));
        }

        public void ThreadStart(object state)
        {
            while (true)
            {
                if (IsTriggerTask())
                {
                    lock (lockObj)
                    {
                        if (IsLogDebug)
                            log.Info(string.Format("开始第{0}次执行{1}任务", ++ExecuteCount, TaskName));

                        try
                        {
                            Execute(state);
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }
                    }

                    if (IsLogDebug)
                        log.Info(string.Format("结束第{0}次执行{1}任务", ExecuteCount, TaskName));
                }
                Thread.Sleep(SleepTime);
            }
        }

        /// <summary>
        /// 结束任务
        /// </summary>
        /// <param name="state"></param>
        public override void Stop(object state)
        {
            if (workThread != null)
            {
                ExecuteCount = 0;
                workThread.Join(500);
                workThread.Abort();
                workThread = null;
            }
        }
    }
}
