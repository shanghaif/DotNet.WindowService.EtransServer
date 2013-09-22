using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.ValidSpeedStatistics.Entity
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
        /// GPS时间
        /// </summary>
        public DateTime GpsTime { get; set; }
    }
}
