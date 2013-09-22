using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using ET.WinService.Core.Extension;

namespace ET.WinService.Core.Task
{
    
    /// <summary>
    /// 时间点任务
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
    public abstract class TriggerTimeTask : Task
    {
        public TriggerTimeTask(XElement xelem)
        {
            Initialize(xelem);
        }

        /// <summary>
        /// 任务触发时间字典
        /// </summary>
        public IDictionary<TimeSpan, bool> TriggerTimes
        {
            get;
            set;
        }

        /// <summary>
        /// 重置触发历史记录
        /// </summary>
        protected void Reset()
        {
            TriggerTimes = TriggerTimes.ToDictionary(o => o.Key, o => false);
        }

        /// <summary>
        /// 判断是否触发执行任务
        /// </summary>
        /// <returns></returns>
        protected override bool IsTriggerTask()
        {
            IList<TimeSpan> triggeredTimes = new List<TimeSpan>();

            foreach (KeyValuePair<TimeSpan, bool> keyValuePair in TriggerTimes)
            {
                if (!keyValuePair.Value && DateTime.Now > DateTime.Now.Date.Add(keyValuePair.Key) && DateTime.Now < DateTime.Now.Date.Add(keyValuePair.Key).Add(TimeSpan.FromMinutes(ServerContext.Context.TaskValidTimeSpan)))
                {
                    triggeredTimes.SafeAdd(keyValuePair.Key);
                }
            }

            if (triggeredTimes.Count > 0)
            {
                foreach (var item in triggeredTimes)
                    TriggerTimes[item] = true;
            }

            return triggeredTimes.Count > 0;
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (DateTime.Now.Date != LastExecuteTime.Date)//如果时间上次执行日期与当前日期不相等重置触发时间
                Reset();

            //判断是否触发执行任务
            if (IsTriggerTask() || StartupExecute)
            {
                if (IsLogDebug)
                {
                    log.Info("执行情况：" + string.Join(",", TriggerTimes.Select(t => string.Format("{0}={1}", t.Key.ToSafeString(), t.Value)).ToArray()));
                    log.Info(string.Format("开始第{0}次执行{1}任务", ++ExecuteCount, TaskName));
                }

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
                {
                    log.Info(string.Format("结束第{0}次执行{1}任务", ExecuteCount, TaskName));
                }
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
                TriggerTimes = xelem.Descendants("triggerTime").ToDictionary(k => k.Value.ToTimeSpan(), e => false);
            }
            catch (Exception ex)
            {
                log.Error("triggerTime 格式错误！", ex);
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
            timer.Interval = 3000;
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
            log.Info(string.Format("停止-{0}任务！", TaskName));
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }
        }
    }
}
