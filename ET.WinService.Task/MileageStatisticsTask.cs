using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using ET.WinService.Core.Task;
using ET.WinService.MileageStatistics;

namespace ET.WinService.Task
{
    public class MileageStatisticsTask : TriggerTimeTask
    {
        public MileageStatisticsTask(XElement xelem) : base(xelem) { }

        public override void Execute(object state)
        {
            try
            {
                MileageStat MileageStatTask = new MileageStat();
                MileageStatTask.Execute();
            }
            catch (Exception ex)
            {
                log.Error("AreaStatisticsTask任务执行出错！", ex);
            }
        }
    }
}
