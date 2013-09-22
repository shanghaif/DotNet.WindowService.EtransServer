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
    public class DrivingRoadEnterAndLeaveMonitorDAL
    {
        public int InserDatat(DrivingRoadEnterAndLeaveMonitor model)
        {
            DbCommand dbCommandWrapper = null;
            try
            {
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.GPSDBConnetionstring);
                string deleteSql = @"insert into dbo.ANA_DrivingRoadEnterAndLeaveMonitor(VehicleID,AnalyseGroupID,GenerateTime,RoadID,EnterTime,LeaveTime,TotalTime) " +
                    "values (@VehicleID,@AnalyseGroupID,@GenerateTime,@RoadID,@EnterTime,@LeaveTime,@TotalTime)";
                dbCommandWrapper = db.GetSqlStringCommand(deleteSql);

                #region Add parameters
                db.AddInParameter(dbCommandWrapper, "@VehicleID", DbType.Int64, model.VehicleID);
                db.AddInParameter(dbCommandWrapper, "@AnalyseGroupID", DbType.Int64, model.AnalyseGroupID);
                db.AddInParameter(dbCommandWrapper, "@GenerateTime", DbType.DateTime, model.GenerateTime);
                db.AddInParameter(dbCommandWrapper, "@RoadID", DbType.Int32, model.RoadID);
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
