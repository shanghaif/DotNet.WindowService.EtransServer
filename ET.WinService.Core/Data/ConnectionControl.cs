using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.Core.Data
{
    /// <summary>
    /// 对连接的处理
    /// </summary>
    public sealed class ConnectionControl
    {

        private static string _DefaultConnetionstring = "DataConnectionString";

        /// <summary>
        /// 默认连接   与配置文件的同步
        /// </summary>
        public static string DefaultConnectionstring
        {
            get { return _DefaultConnetionstring; }
            set { _DefaultConnetionstring = value; }
        }

        private static string _GPSDBConnetionstring = "GPSDBConnetionstring";

        /// <summary>
        /// GPSDB连接库  与配置文件的同步
        /// </summary>
        public static string GPSDBConnetionstring 
        {
            get { return _GPSDBConnetionstring; }
            set { _GPSDBConnetionstring = value; }
        }

        private static string _OtherConnectionString = "OtherConnectionString";
        /// <summary>
        /// 轨迹连接 车辆轨迹数据库的连接，该库只存储7天的轨迹数据
        /// </summary>
        public static string OtherConnectionString
        {
            get { return ConnectionControl._OtherConnectionString; }
            set { ConnectionControl._OtherConnectionString = value; }
        } 
    }
}
