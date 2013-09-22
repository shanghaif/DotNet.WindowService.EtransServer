using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.ValidSpeedStatistics.Entity
{
    /// <summary>
    /// 学习结果
    /// </summary>
    public class StudyRecord
    {
        /// <summary>
        /// ID
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        public long DeviceNo { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime SR_BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime SR_EndTime { get; set; }
    }
}
