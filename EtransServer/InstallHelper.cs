using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Configuration.Install;

namespace ET.WinService
{

    /// <summary>
    /// 服务安装
    /// </summary>
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：不用InstallUtil.exe安装服务
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
    public static class InstallHelper
    {
        private static readonly string _exePath = Assembly.GetExecutingAssembly().Location;
        public static bool Install()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(
                    new string[] {"/i", _exePath });
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool Uninstall()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(
                    new string[] { "/u", _exePath });
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
