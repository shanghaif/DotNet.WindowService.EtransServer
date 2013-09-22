using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.AreaStatistics.Entity
{
    public class ParamRoadSegments
    {
        /// <summary>
        /// 路段ID
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 分析组ID
        /// </summary>
        public long AnalyseGroupID { get; set; }
    }
}
