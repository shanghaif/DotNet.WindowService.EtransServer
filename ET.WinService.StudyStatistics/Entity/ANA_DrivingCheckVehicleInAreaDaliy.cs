using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.StudyStatistics.Entity
{
    public class ANADrivingCheckVehicleInAreaDaliy
    {
        public int VehicleID { get; set; }
        public DateTime InsideBeginCheckTime { get; set; }
        public DateTime InsideEndCheckTime { get; set; }
    }
}
