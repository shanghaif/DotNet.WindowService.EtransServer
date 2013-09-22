using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.AreaStatistics.Entity
{
    /// <summary>
    /// 此实体用来记录车辆信息
    /// </summary>
    public class Vehicle
    {
        /// <summary>
        /// 车辆ID
        /// </summary>
        public long VehicleID { get; set; }

        /// <summary>
        /// 车辆ID，为通讯号的后九位
        /// </summary>
        public string CarId { get; set; }
    }
}
