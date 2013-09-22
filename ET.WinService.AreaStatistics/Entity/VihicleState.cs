using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.AreaStatistics.Entity
{
    /// <summary>
    /// 此实例用来记录车辆的当前状态
    /// </summary>
    public class VihicleState
    {
        /// <summary>
        /// 车辆ID
        /// </summary>
        public long VehicleID { get; set; }

  

        /// <summary>
        /// 条件ID
        /// </summary>
        public long ConditionID { get; set; }

        /// <summary>
        /// 是否在区域内
        /// </summary>
        public int IfInArea { get; set; }


        /// <summary>
        /// 累计不在区域时间
        /// </summary>
        public int AllOutTime { get; set; }

        /// <summary>
        /// 是否曾经在区域内
        /// </summary>
        public int IfOnceInArea { get; set; }

        /// <summary>
        /// 最后一次Gps时间
        /// </summary>
        public DateTime LastGpsTime { get; set; }
    }
}
