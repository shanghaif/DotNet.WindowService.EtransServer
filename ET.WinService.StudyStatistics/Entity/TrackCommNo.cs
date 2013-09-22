using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.StudyStatistics.Entity
{
    public class TrackCommNo
    {
        public string DeviceNo { get; set; }
        public DateTime SR_BeginTime{get;set;}
        public DateTime SR_EndTime { get; set; }
        public short IsValid { get; set; }

        public TrackCommNo()
        {
            IsValid = 1;
        }
    }
}
