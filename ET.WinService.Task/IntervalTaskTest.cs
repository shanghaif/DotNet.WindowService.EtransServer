using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using ET.WinService.Core.Task;

namespace ET.WinService.Task
{
    public class IntervalTaskTest:IntervalTask
    {
        public IntervalTaskTest(XElement xelem) : base(xelem) 
        { 
            Interval = TimeSpan.FromSeconds(6);
            log.Info("1/初始化任务类：" + Interval.TotalMilliseconds);
        }

        /// <summary>
        /// 是否触发任务
        /// </summary>
        /// <returns></returns>
        //protected override bool IsTriggerTask()
        //{
        //    return (LastExecuteTime.Month < DateTime.Now.Month && LastExecuteTime.AddMonths(1) > DateTime.Now && LastExecuteTime.AddMonths(1) < DateTime.Now.AddSeconds(30))
        //            || (LastExecuteTime > DateTime.Now && LastExecuteTime < DateTime.Now.AddSeconds(30));
        //}

        public override void Execute(object state)
        {
            try
            {
                log.Info(string.Format("周期性任务执行时间：{0}", DateTime.Now.ToString()));
            }
            catch (Exception ex)
            {
                log.Error("周期性任务执行出错！", ex);
            }
        }
    }
}
