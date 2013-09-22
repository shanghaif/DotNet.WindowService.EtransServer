using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using ET.WinService.Core.Task;
using ET.WinService.ValidSpeedStatistics;

namespace ET.WinService.Task
{
    public class ValidSpeedStatisticsTask : TriggerTimeTask
    {
        public ValidSpeedStatisticsTask(XElement xelem) : base(xelem) { }
        public override void Execute(object state)
        {
            try
            {
                ValidSpeedStat ValidSpeedStatTask = new ValidSpeedStat();
                ValidSpeedStatTask.Execute();
            }
            catch (Exception ex)
            {
                log.Error("AreaStatisticsTask任务执行出错！", ex);
            }
        }
    }
}
