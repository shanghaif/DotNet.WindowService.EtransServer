using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.Core.Task
{

    /// <summary>
    /// 任务实体类
    /// </summary>
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：
    /// 版本号：1.0.0.1
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 日  期：2013年2月1日
    /// 修  改：
    /// 原  因：
    /// ------------------------------------------------------------------------------
    /// </remarks>
    /// <example>
    /// [示例代码在这里写入]
    /// </example>
    public class TaskModel
    {
        /// <summary>
        /// 任务实现类
        /// </summary>
        public string Class { get; set; }
        /// <summary>
        /// 任务实现类所在程序集
        /// </summary>
        public string Assembly { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 每个周期发送完毕时线程休眠时间
        /// </summary>
        public int SleepTime { get; set; }
        /// <summary>
        /// 任务触发时间列表
        /// </summary>
        public string triggerTimes { get; set; }
        /// <summary>
        /// 是否使用
        /// </summary>
        public bool Activate { get; set; }
        /// <summary>
        /// 任务触发时间间隔
        /// </summary>
        public TimeSpan Interval { get; set; }
        /// <summary>
        /// 第一次执行开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
    }
}
