using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.StudyStatistics.Entity
{

    /// <summary>
    /// 学员学习汇总表
    /// </summary>
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：
    /// 版本号：1.0.0.1
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 日  期：2013年1月21日
    /// 修  改：
    /// 原  因：
    /// ------------------------------------------------------------------------------
    /// </remarks>
    /// <example>
    /// [示例代码在这里写入]
    /// </example>
    public class StudyStatStudent
    {
        public int? ID { get; set; }
        /// <summary>
        /// 学员ID
        /// </summary>
        public int StuID { get; set; }
        /// <summary>
        /// 科目ID
        /// </summary>
        public int SubjectID { get; set; }
        /// <summary>
        /// 学习类型标识。0 理论;1 实操;2 模拟
        /// </summary>
        public int? StudyType { get; set; }
        /// <summary>
        /// 学习时间(分钟)
        /// </summary>
        public double? SR_RealTime { get; set; }
        /// <summary>
        /// 实际学习时间(分钟)
        /// </summary>
        public double? SR_ActualTime { get; set; }
        /// <summary>
        /// 有效学习时间(分钟)
        /// </summary>
        public double? ValidActualTime { get; set; }
        /// <summary>
        /// 无效学习时间(分钟)
        /// </summary>
        public double? InvalidActualTime { get; set; }

        /// <summary>
        /// 有效学时成绩(分钟)有效学习成绩，为考核学员学习是否达标的学时，=实际有效学习时间*系数
        /// </summary>
        public double? ValidStudyTime_Exam { get; set; }
    }
}
