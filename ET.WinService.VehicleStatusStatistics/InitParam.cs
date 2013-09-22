using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using ET.WinService.Core.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ET.WinService.VehicleStatusStatistics
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
            /// 从[T_StudyStatParam_VehicleStatus]查条件
            ///     1	ACC ACC关;持续10分钟;无效
            ///     2	ENG 发动机关;持续10分钟;无效
            if (dtParameterValue == null)
            {
                try
                {
                    Database db = DatabaseFactory.CreateDatabase(ConnectionControl.DefaultConnectionstring);
                    DataSet ds = null;
                    DbCommand dbCommandWrapper = null;
                    try
                    {
                        dbCommandWrapper = db.DbProviderFactory.CreateCommand();
                        dbCommandWrapper.CommandType = CommandType.Text;
                        string selectSql = @"SELECT * FROM [T_StudyStatParam_VehicleStatus] WHERE [ID] IN (1,2)";
                        dbCommandWrapper.CommandText = selectSql;
                        ds = db.ExecuteDataSet(dbCommandWrapper);
                        dtParameterValue = ds.Tables[0];
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (dbCommandWrapper != null)
                        {
                            dbCommandWrapper = null;
                        }
                        if (ds != null)
                        {
                            ds = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
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

        private bool _checkACCIsEnabled;
        /// <summary>
        /// ACC检查是否启用
        /// </summary>
        public bool CheckACCIsEnabled
        {
            get
            {
                _checkACCIsEnabled = bool.Parse(dtParameterValue.Rows[0]["IsEnabled"].ToString());
                return _checkACCIsEnabled;
            }
        }

        private int _checkACCDuration;
        /// <summary>
        /// ACC持续时间（单位：分）
        /// </summary>
        public int CheckACCDuration
        {
            get
            {
                _checkACCDuration = int.Parse(dtParameterValue.Rows[0]["Duration"].ToString());
                return _checkACCDuration;
            }
        }

        private bool _checkENGsEnabled;
        /// <summary>
        /// ENG检查是否启用
        /// </summary>
        public bool CheckENGsEnabled
        {
            get
            {
                _checkENGsEnabled = bool.Parse(dtParameterValue.Rows[1]["IsEnabled"].ToString());
                return _checkENGsEnabled;
            }
        }

        private int _checkENGDuration;
        /// <summary>
        /// ENG持续时间（单位：分）
        /// </summary>
        public int CheckENGDuration
        {
            get
            {
                _checkENGDuration = int.Parse(dtParameterValue.Rows[1]["Duration"].ToString());
                return _checkENGDuration;
            }
        }
    }
}
