using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using ET.WinService.Core.Task;
using ET.WinService.VehicleStatusStatistics;

namespace ET.WinService.Task
{
    public class VehicleStatusStatisticsTask : TriggerTimeTask
    {
        public VehicleStatusStatisticsTask(XElement xelem) : base(xelem) { }
        public override void Execute(object state)
        {
            try
            {
                VehicleStatusStat VehicleStatusStatTask = new VehicleStatusStat();
                VehicleStatusStatTask.Execute();
            }
            catch (Exception ex)
            {
                log.Error("AreaStatisticsTask任务执行出错！", ex);
            }
        }
    }
}
