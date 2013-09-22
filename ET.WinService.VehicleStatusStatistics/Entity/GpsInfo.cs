using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.VehicleStatusStatistics.Entity
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
        /// GPS时间
        /// </summary>
        public DateTime GpsTime { get; set; }

        /// <summary>
        /// GPS状态
        /// </summary>
        public string GpsState { get; set; }
    }
}
