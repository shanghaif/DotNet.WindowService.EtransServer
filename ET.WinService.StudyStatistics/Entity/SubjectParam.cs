using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.StudyStatistics.Entity
{

    /// <summary>
    /// 科目学时参数配置实体类
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
    public class SubjectParam
    {
        public int SubjectID { get; set; }      //科目ID
        public int MinTime { get; set; }        //每次最小计时时间(分钟)
        public int MaxTime { get; set; }        //每天最大有限学时(分钟)
        public int? StudyType { get; set; }      //学习类型标识。0 理论;1 实操;2 模拟
        public string StartTime { get; set; }   //有效开始时间
        public string EndTime { get; set; }     //有效结束时间
    }
}
