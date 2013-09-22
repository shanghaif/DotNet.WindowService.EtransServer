using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using ET.WinService.Core.Task;
using ET.WinService.StudyStatistics;

namespace ET.WinService.Task
{

    /// <summary>
    /// 驾培学时统计任务
    /// </summary>
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：
    /// 版本号：1.0.0.1
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 日  期：2013年1月18日
    /// 修  改：
    /// 原  因：
    /// ------------------------------------------------------------------------------
    /// </remarks>
    /// <example>
    /// [示例代码在这里写入]
    /// </example>
    public class StudyStaisticsTask : TriggerTimeTask
    {
        public StudyStaisticsTask(XElement xelem) : base(xelem) { }

        public override void Execute(object state)
        {
            try
            {
                StudyStat studyStat = new StudyStat();
                studyStat.Execute();
            }
            catch (Exception ex)
            {
                log.Error("任务执行出错！", ex);
            }
        }

        //public override string SetLoggerName()
        //{
        //    return "StudyStat";
        //}
    }
}
