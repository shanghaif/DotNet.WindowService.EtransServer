using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using ET.WinService.Core.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ET.WinService.ValidSpeedStatistics
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
            /// 从[T_Parameter]查参数
            ///     115	最小速度值
            ///     116	最大速度值
            ///     117	合法速度百比值
            /// 从[T_ParameterValue]查结果
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
                        string selectSql = @"SELECT [P_Value] FROM [T_ParameterValue] WHERE [P_SortID] IN (115,116,117)";
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

        private float _minSpeed;
        /// <summary>
        /// 最小速度值
        /// </summary>
        public float MinSpeed
        {
            get
            {
                if (_minSpeed == 0)
                {
                    _minSpeed = float.Parse(dtParameterValue.Rows[0][0].ToString());
                }
                return _minSpeed;
            }
        }

        private float _maxSpeed;
        /// <summary>
        /// 最大速度值
        /// </summary>
        public float MaxSpeed
        {
            get
            {
                if (_maxSpeed == 0)
                {
                    _maxSpeed = float.Parse(dtParameterValue.Rows[1][0].ToString());
                }
                return _maxSpeed;
            }
        }

        private float _validSpeedPercent;
        /// <summary>
        /// 合法速度百比值
        /// </summary>
        public float ValidSpeedPercent
        {
            get
            {
                if (_validSpeedPercent == 0)
                {
                    _validSpeedPercent = float.Parse(dtParameterValue.Rows[2][0].ToString());
                }
                return _validSpeedPercent / 100;
            }
        }
    }
}
