using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using ET.WinService.Core.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ET.WinService.MileageStatistics
{
    /// <summary>
    /// 初始化参数
    /// </summary>
    public class InitParam
    {
        private static InitParam instance;
        public DataTable dtParameterValue;

        public InitParam()
        {
            
        }

        /// <summary>
        /// 单例访问
        /// </summary>
        public static InitParam Instance
        {
            get
            {
                lock (typeof(InitParam))
                {
                    if (instance == null)
                        instance = new InitParam();
                    return instance;
                }
            }
        }

        private string dayStartTime;
        /// <summary>
        /// 分析开始时间
        /// </summary>
        public string DayStartTime
        {
            get
            {
                if (string.IsNullOrEmpty(dayStartTime))
                {
                    dayStartTime = ConfigurationManager.AppSettings["AreaStartTime"].ToString();
                }
                return dayStartTime;
            }
        }

        private string dayEndTime;
        /// <summary>
        /// 分析结束时间
        /// </summary>
        public string DayEndTime
        {
            get
            {
                if (string.IsNullOrEmpty(dayEndTime))
                {
                    dayEndTime = ConfigurationManager.AppSettings["AreaEndTime"].ToString();
                }
                return dayEndTime;
            }
        }
    }
}
