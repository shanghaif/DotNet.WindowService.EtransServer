using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.StudyStatistics.Entity
{
    public class StudyDrivingSubject
    {
        public double StudyRate { get; set; }
        public double StudyRate_Simulate { get; set; }
        public double StudyTime_Simulate { get; set; }

        public double StudyRate_Theoretical { get; set; }
        public double StudyTime_Theoretical { get; set; }
        public double StudyRate_Operate { get; set; }
        public double StudyTime_Operate { get; set; }
    }
}
