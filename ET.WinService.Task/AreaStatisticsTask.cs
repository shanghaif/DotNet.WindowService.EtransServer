using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using ET.WinService.Core.Task;
using ET.WinService.AreaStatistics;
namespace ET.WinService.Task
{
    public class AreaStatisticsTask : TriggerTimeTask
    {
        public AreaStatisticsTask(XElement xelem) : base(xelem) { }

        public override void Execute(object state)
        {
            try
            {
                AreaStat AreaStatTask = new AreaStat();
                AreaStatTask.Execute();
            }
            catch (Exception ex)
            {
                log.Error("AreaStatisticsTask任务执行出错！", ex);
            }
        }
    }
}
