using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using ET.WinService.Core.Extension;

namespace ET.WinService.Core.Task
{
    
    /// <summary>
    /// 周期性任务
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
    public abstract class IntervalTask : Task
    {
        /// <summary>
        ///任务触发时间间隔
        /// </summary>
        public TimeSpan Interval
        {
            get;
            set;
        }

        /// <summary>
        /// 第一次执行开始时间
        /// </summary>
        public DateTime StartTime
        {
            get;
            set;
        }

        /// <summary>
        ///构造函数
        /// </summary>
        /// <param name="xelem">配置结点</param>
        public IntervalTask(XElement xelem)
        {
            Initialize(xelem);
        }

        /// <summary>
        /// 判断是否触发执行任务
        /// </summary>
        /// <returns></returns>
        protected override bool IsTriggerTask()
        {
                return DateTime.Now > LastExecuteTime.Add(Interval);
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //判断是否触发执行任务
            if (IsTriggerTask() || StartupExecute)
            {
                if (IsLogDebug)
                    log.Info(string.Format("开始第{0}次执行{1}任务", ++ExecuteCount, TaskName));

                StartupExecute = false;
                LastExecuteTime = DateTime.Now;

                try
                {
                    Execute(e);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                if (IsLogDebug)
                    log.Info(string.Format("结束第{0}次执行{1}任务", ExecuteCount, TaskName));
            }
        }


        /// <summary>
        /// 初始化任务
        /// </summary>
        /// <param name="parameters"></param>
        public override void Initialize(XElement xelem)
        {
            base.Initialize(xelem);

            try
            {
                Interval = xelem.ChildElementValue("interval").ToTimeSpan();
                StartTime = xelem.ChildElementValue("startTime").ToDateTime(DateTime.Now.Date);

                //如果开始时间是过去的时候
                if (StartTime < DateTime.Now)
                {
                    //如果启动执行，则日期被置为当前
                    if (StartupExecute)
                    {
                        StartTime = DateTime.Now.Date.Add(StartTime.TimeOfDay);
                    }
                    else if (StartTime.AddMonths(1) > DateTime.Now)
                    {
                        StartTime = StartTime.AddMonths(1);
                    }
                    else if (DateTime.Now.Subtract(StartTime).Days > 30 * 2 && StartTime.Year == DateTime.Now.Year)
                    {
                        StartTime = StartTime.AddMonths(DateTime.Now.Month - StartTime.Month - 1);
                    }
                    else
                    {
                        StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, StartTime.Day).Add(StartTime.TimeOfDay);
                    }
                }

                LastExecuteTime = StartTime;
            }
            catch (Exception ex)
            {
                log.Error("初始化时间间隔任务出错！", ex);
            }
        }

        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="state"></param>
        public override void Start(object state)
        {   
            log.Info(string.Format("开始启动-{0}任务！", TaskName));
            timer = new System.Timers.Timer();
            timer.Interval = 60000 < Interval.TotalMilliseconds ? 3000 : Interval.TotalMilliseconds;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.AutoReset = true;
            timer.Enabled = true;

            log.Info(string.Format("成功启动-{0}任务！", TaskName));
        }

        /// <summary>
        /// 结束任务
        /// </summary>
        /// <param name="state"></param>
        public override void Stop(object state)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }
        }
    }
}
