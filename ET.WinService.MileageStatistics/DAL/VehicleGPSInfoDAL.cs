using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using ET.WinService.Core.Data;
using ET.WinService.MileageStatistics.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ET.WinService.MileageStatistics.DAL
{
    public class VehicleGPSInfoDAL
    {
        /// <summary>
        /// 获取指定时间段内的轨迹数据
        /// </summary>
        /// <param name="CommNo">终端号</param>
        /// <param name="analyseStartTime">开始时间</param>
        /// <param name="analyseEndTime">结束时间</param>
        /// <param name="maxSpeed">最大速度</param>
        /// <param name="mileage">行驶里程</param>
        public void GetYesterDayGpsInfo(long CommNo, DateTime analyseStartTime, DateTime analyseEndTime, out int maxSpeed, out int mileage)
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
                        maxSpeed = 0;
                        mileage = 0;
                    }
                    else
                    {
                        List<GpsInfo> gpsInfos = DataUtil.ToList<GpsInfo>(ds.Tables[0]);
                        if (gpsInfos == null)
                        {
                            maxSpeed = 0;
                            mileage = 0;
                        }
                        else
                        {
                            var fistGpsInfo = gpsInfos.First();
                            var lastGpsInfo = gpsInfos.Last();

                            //计算最大速度
                            maxSpeed = 0;
                            foreach (var gpsInfo in gpsInfos)
                            {
                                if (gpsInfo.Speed >= maxSpeed)
                                {
                                    maxSpeed = gpsInfo.Speed;
                                }
                            }
                            //计算行驶里程
                            mileage = lastGpsInfo.Mileage - fistGpsInfo.Mileage;

                            lastGpsInfo = null;
                            fistGpsInfo = null;
                            gpsInfos = null;
                        }
                    }
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
