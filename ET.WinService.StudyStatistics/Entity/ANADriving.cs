using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.StudyStatistics.Entity
{

    /// <summary>
    /// 道路及区域进出分析记录
    /// </summary>
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：
    /// 版本号：1.0.0.1
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 日  期：2013年1月24日
    /// 修  改：
    /// 原  因：
    /// ------------------------------------------------------------------------------
    /// </remarks>
    /// <example>
    /// [示例代码在这里写入]
    /// </example>
    public class ANADriving
    {
        /// <summary>
        /// 终端号后8位
        /// </summary>
        public string DeviceNo { get; set; }
        /// <summary>
        /// 终端号后8位
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 离开时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 分析数据类型。1:道路进出分析记录;2:道路停留超时分析记录;3:区域进出分析记录;4:区域停留超时分析记录。
        /// </summary>
        public int DataType { get; set; }
    }
}
