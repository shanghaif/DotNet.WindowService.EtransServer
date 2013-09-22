using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;

using ET.WinService.StudyStatistics.Entity;
using ET.WinService.Core.Data;

namespace ET.WinService.StudyStatistics.DAL
{
    public class ANADrivingDAL
    {
        public ANADrivingDAL()
        {
        }

        public List<ANADrivingCheckVehicleInAreaDaliy> GetLisByVehicleIDs(string gpsDbName, string vehicleIDs)
        {
            DataSet ds = null;
            DbCommand dbCommandWrapper = null;
            try
            {
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.DefaultConnectionstring);
                dbCommandWrapper = db.DbProviderFactory.CreateCommand();
                dbCommandWrapper.CommandType = CommandType.Text;
                string selectSql = string.Format(@"select VehicleID,InsideBeginCheckTime,InsideEndCheckTime 
                                                            from {0}.dbo.ANA_DrivingCheckVehicleInAreaDaliy 
                                                        where VehicleID in ({1})", gpsDbName, vehicleIDs);
                dbCommandWrapper.CommandText = selectSql;
                ds = db.ExecuteDataSet(dbCommandWrapper);
                if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count <= 0)
                {
                    return null;
                }
                return DataUtil.ToList<ANADrivingCheckVehicleInAreaDaliy>(ds.Tables[0]);
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
    }
}
