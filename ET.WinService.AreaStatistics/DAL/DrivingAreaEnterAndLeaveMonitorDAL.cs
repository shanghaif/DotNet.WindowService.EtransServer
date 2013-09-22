using System;
using System.Collections.Generic;
using System.Text;
using ET.WinService.AreaStatistics.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using ET.WinService.Core.Data;
using ET.WinService.AreaStatistics;


namespace ET.WinService.AreaStatistics.DAL
{
     /// <summary>
    /// 描  述： DrivingAreaEnterAndLeaveMonitorDAL操作类
    /// 作  者：陈炯炯 (chenjiongjiong@e-trans.com.cn)
    /// 时  间：2013年1月16日
    /// 修  改：
    /// 原  因：
    /// </summary>
    /// <returns></returns>
    public class DrivingAreaEnterAndLeaveMonitorDAL
    {
        public int InserDatat(DrivingAreaEnterAndLeaveMonitor model)
        {
            DbCommand dbCommandWrapper = null;
            try
            {
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.GPSDBConnetionstring);
                string deleteSql = @"insert into dbo.ANA_DrivingAreaEnterAndLeaveMonitor(VehicleID,AnalyseGroupID,GenerateTime,AreaID,EnterTime,LeaveTime,TotalTime) "+
                    "values (@VehicleID,@AnalyseGroupID,@GenerateTime,@AreaID,@EnterTime,@LeaveTime,@TotalTime)";
                dbCommandWrapper = db.GetSqlStringCommand(deleteSql);

                #region Add parameters
                db.AddInParameter(dbCommandWrapper, "@VehicleID", DbType.Int64, model.VehicleID);
                db.AddInParameter(dbCommandWrapper, "@AnalyseGroupID", DbType.Int64, model.AnalyseGroupID);
                db.AddInParameter(dbCommandWrapper, "@GenerateTime", DbType.DateTime, model.GenerateTime);
                db.AddInParameter(dbCommandWrapper, "@AreaID", DbType.Int32, model.AreaID);
                db.AddInParameter(dbCommandWrapper, "@EnterTime", DbType.DateTime, model.EnterTime);
                db.AddInParameter(dbCommandWrapper, "@LeaveTime", DbType.DateTime, model.LeaveTime);
                db.AddInParameter(dbCommandWrapper, "@TotalTime", DbType.Int32, model.TotalTime);
                #endregion
                return db.ExecuteNonQuery(dbCommandWrapper);
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
            }
        }
    }
}
