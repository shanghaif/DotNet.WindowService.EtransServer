using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.AreaStatistics.Entity
{
     /// <summary>
     /// 路段分析条件实体
     /// </summary>
     public class AnalyserRoadEnterAndLeaveMonitor
    {
        public long ID { get; set; }
        /// <summary>
        /// 分析组ID
        /// </summary>
        public long AnalyseGroupID { get; set; }
        /// <summary>
        /// 允许离开的最大距离
        /// </summary>
        public int ValidDistance { get; set; }
        /// <summary>
        /// 允许离开的最大时间
        /// </summary>
        public int AllowLeaveMaxTime { get; set; }
    }
}
