using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.AreaStatistics.Entity
{
    public class DrivingAreaEnterAndLeaveMonitor
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 车辆ID
        /// </summary>
        public long VehicleID { get; set; }

        /// <summary>
        /// 分析组
        /// </summary>
        public long AnalyseGroupID { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime GenerateTime { get; set; }

        /// <summary>
        /// 区域ID
        /// </summary>
        public int AreaID { get; set; }

        /// <summary>
        /// 进入区域时间
        /// </summary>
        public DateTime EnterTime { get; set; }

        /// <summary>
        /// 离开区域时间
        /// </summary>
        public DateTime LeaveTime { get; set; }

        /// <summary>
        /// 经历时间总长
        /// </summary>
        public int TotalTime { get; set; } 
    }
}
