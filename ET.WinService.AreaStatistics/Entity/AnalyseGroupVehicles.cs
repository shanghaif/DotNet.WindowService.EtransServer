using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.AreaStatistics.Entity
{
    public class AnalyseGroupVehicles
    {
         /// <summary>
         /// 主键ID
         /// </summary>
         public long ID{get; set;}

         /// <summary>
         /// 车辆ID
         /// </summary>
         public long VehicleID { get; set; }
         
         /// <summary>
         /// 分析组ID
         /// </summary>
         public long AnalyseGroupID { get; set; }
         
         /// <summary>
         /// 车辆ID，为通讯号的后九位
         /// </summary>
         public string CarId { get; set; }
    }
}
