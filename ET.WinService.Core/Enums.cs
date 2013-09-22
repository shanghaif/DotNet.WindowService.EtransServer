using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ET.WinService.Core.Utility;

namespace ET.WinService.Core
{
    public enum ServiceStatus
    {
        /// <summary>
        /// 服务未运行
        /// </summary>
        [Remark("服务未运行")]
        Stopped = 1,

        /// <summary>
        ///  服务正在启动
        /// </summary>
        [Remark("服务正在启动")]
        StartPending = 2,

        /// <summary>
        ///  服务正在停止
        /// </summary>
        [Remark("服务正在停止")]
        StopPending = 3,

        /// <summary>
        /// 服务正在运行
        /// </summary>
        [Remark("服务正在运行")]
        Running = 4,

        /// <summary>
        /// 服务即将继续
        /// </summary>
        [Remark("服务即将继续")]
        ContinuePending = 5,

        /// <summary>
        /// 服务即将暂停
        /// </summary>
        [Remark("服务即将暂停")]
        PausePending = 6,

        /// <summary>
        /// 服务已暂停
        /// </summary>
        [Remark("服务已暂停")]
        Paused = 7,

        /// <summary>
        /// 服务已卸载
        /// </summary>
        [Remark("服务已卸载")]
        Uninstall = 8,
    }
}
