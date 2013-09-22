using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.StudyStatistics.Entity
{
    public class TrackDataCommNo
    {
        public string DeviceNo { get; set; }
        public DateTime GpsTime { get; set; }
        public short IsAccClose { get; set; }
        public short IsEngClose { get; set; }
    }
}
