using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.StudyStatistics.Entity
{

    /// <summary>
    /// Gps数据实体类
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：
    /// 版本号：1.0.0.1
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 日  期：2013年1月17日
    /// 修  改：
    /// 原  因：
    /// ------------------------------------------------------------------------------
    /// </remarks>
    /// <example>
    /// [示例代码在这里写入]
    /// </example>
    public class StudyRecordGpsData
    {
        public int StudyRecordID { get; set; }          //对应w_StudyRecord_Result表的ID
        public DateTime SR_BeginTime { get; set; }
        public DateTime SR_EndTime { get; set; }
        public string DeviceNo { get; set; }            //终端号后8位
        public DateTime StartGpsTime { get; set; }
        public DateTime EndGpsTime { get; set; }
        public int Duration_Sec { get; set; }           //持续时间(秒)
        public short IsValid { get; set; }
        public short HasFinished { get; set; }          //是否计算完(无效)

        public StudyRecordGpsData()
        {
            Duration_Sec = 0;
            IsValid = 1;
            HasFinished = 0;
        }
    }
}
