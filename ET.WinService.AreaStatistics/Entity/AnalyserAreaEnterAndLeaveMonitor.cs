using System;
using System.Collections.Generic;
using System.Text;

namespace ET.WinService.AreaStatistics.Entity
{
    public class AnalyserAreaEnterAndLeaveMonitor
    {
        public long ID { get; set; }
        /// <summary>
        /// 分析组ID
        /// </summary>
        public long AnalyseGroupID { get; set; }
        /// <summary>
        /// 区域ID
        /// </summary>
        public int AreaID { get; set; }
        /// <summary>
        /// 允许离开的最大时间
        /// </summary>
        public int AllowLeaveMaxTime { get; set; }
    }
}
