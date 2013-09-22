using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.MileageStatistics.Entity
{
    /// <summary>
    /// 轨迹信息
    /// </summary>
    public class GpsInfo
    {
        /// <summary>
        /// 终端号
        /// </summary>
        public long CarID { get; set; }

        /// <summary>
        /// 速度
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// 里程
        /// </summary>
        public int Mileage { get; set; }

        /// <summary>
        /// GPS时间
        /// </summary>
        public DateTime GpsTime { get; set; }
    }
}
