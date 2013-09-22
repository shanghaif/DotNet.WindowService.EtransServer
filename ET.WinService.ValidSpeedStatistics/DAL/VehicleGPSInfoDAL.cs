using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using ET.WinService.Core.Data;
using ET.WinService.ValidSpeedStatistics.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ET.WinService.ValidSpeedStatistics.DAL
{
    public class VehicleGPSInfoDAL
    {
        /// <summary>
        /// 获取指定时间段内的轨迹数据
        /// </summary>
        /// <param name="analyseStartTime">开始时间</param>
        /// <param name="analyseEndTime">结束时间</param>
        /// <returns></returns>
        public List<GpsInfo> GetYesterDayGpsInfo(long CommNo, DateTime analyseStartTime, DateTime analyseEndTime)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.OtherConnectionString);
                DataSet ds = null;
                DbCommand dbCommandWrapper = null;
                try
                {
                    dbCommandWrapper = db.GetStoredProcCommand("P_QueryCarGpsInfo");
                    db.AddInParameter(dbCommandWrapper, "@_car_id", DbType.Int64, CommNo);
                    db.AddInParameter(dbCommandWrapper, "@_sTime", DbType.DateTime, analyseStartTime);
                    db.AddInParameter(dbCommandWrapper, "@_eTime", DbType.DateTime, analyseEndTime);
                    db.AddInParameter(dbCommandWrapper, "@_qType", DbType.Int32, 2);
                    ds = db.ExecuteDataSet(dbCommandWrapper);
                    if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count <= 0)
                    {
                        return null;
                    }
                    return DataUtil.ToList<GpsInfo>(ds.Tables[0]);
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
}
