using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.VehicleStatusStatistics.Entity
{
    public class VehicleGPSInfo
    {
        /// <summary>
        /// GPS时间
        /// </summary>
        public DateTime GpsTime { get; set; }

        /// <summary>
        /// ACC状态
        /// </summary>
        public bool ACC { get; set; }

        /// <summary>
        /// ENG状态
        /// </summary>
        public bool ENG { get; set; }
    }
}
