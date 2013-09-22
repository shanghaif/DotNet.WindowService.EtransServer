using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.AreaStatistics.Entity
{
    public class VehicleGpsInfo
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public long GpsId { get; set;}

        /// <summary>
        /// 通讯号ID，后八位
        /// </summary>
        public long CarId { get; set; }
      

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// GPS时间
        /// </summary>
        public DateTime GpsTime { get; set; }

        /// <summary>
        /// 登记时间
        /// </summary>
        public DateTime RecvTime { get; set; }

      

        /// <summary>
        /// 速度
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// gps状态
        /// </summary>
        public string GpsState { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Head { get; set; }


        public bool GpsValid { get; set; }
    }
}
