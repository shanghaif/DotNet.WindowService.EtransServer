using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.StudyStatistics.Entity
{

    /// <summary>
    /// 报考申请实体类
    /// </summary>
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：
    /// 版本号：1.0.0.1
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 日  期：2013年1月15日
    /// 修  改：
    /// 原  因：
    /// ------------------------------------------------------------------------------
    /// </remarks>
    /// <example>
    /// [示例代码在这里写入]
    /// </example>
    public class StuExamApply
    {
        public int ID { get; set; }
        public int StuID { get; set; }
        public string DriveType { get; set; }
        public string Subject { get; set; }
        public string ApplyNote { get; set; }
        public DateTime AddTime { get; set; }
        public string CheckResult { get; set; }
        public string CheckNote { get; set; }
        public string CheckMan { get; set; }
        public DateTime CheckTime { get; set; }
        public bool IsPrint { get; set; }
        public int IfBlankPrint { get; set; }
        public int CheckProcessID { get; set; }
        public string AuditUser { get; set; }
        public DateTime AuditDate { get; set; }
    }
}
