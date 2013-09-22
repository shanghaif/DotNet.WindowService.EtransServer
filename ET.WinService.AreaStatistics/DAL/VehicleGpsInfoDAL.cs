using System;
using System.Collections.Generic;
using System.Text;
using ET.WinService.AreaStatistics.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using ET.WinService.Core.Data;
using ET.WinService.AreaStatistics;
using ET.WinService.AreaStatistics.common;
using System.Data.SqlClient;
namespace ET.WinService.AreaStatistics.DAL
{
    /// <summary>
    /// 描  述： VehicleGpsInfo操作类
    /// 作  者：陈炯炯 (chenjiongjiong@e-trans.com.cn)
    /// 时  间：2013年1月15日
    /// 修  改：
    /// 原  因：
    /// </summary>
    /// <returns></returns>
    public class VehicleGpsInfoDAL
    {
        public List<VehicleGpsInfo> GetList(long CarId, DateTime StartTime, DateTime EndTime)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.OtherConnectionString);
                return this.GetList(CarId, StartTime, EndTime, (DbTransaction)null, db);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 描  述：获取列表对象
        /// 作  者：陈炯炯 (chenjiongjiong@e-trans.com.cn)
        /// 时  间：2013年1月15日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        //public List<VehicleGpsInfo> GetList(long CarId, DateTime StartTime, DateTime EndTime, DbTransaction tran, Database db)
        //{
        //    try
        //    {
        //        DataSet ds = null;
        //        DbCommand dbCommandWrapper = null;
        //        try
        //        {

        //            dbCommandWrapper = db.GetStoredProcCommand("QueryCarGpsInfo_20130118");// db.DbProviderFactory.CreateCommand();
        //            #region Add parameters
        //            //db.GetSqlStringCommand().CommandTimeout = 0;
        //            db.AddInParameter(dbCommandWrapper, "@_car_id", DbType.Int32, CarId);
        //            db.AddInParameter(dbCommandWrapper, "@_sTime", DbType.DateTime, StartTime);
        //            db.AddInParameter(dbCommandWrapper, "@_eTime", DbType.DateTime, EndTime);
        //            db.AddInParameter(dbCommandWrapper, "@_qType", DbType.Int32, 2);
        //            #endregion
        //            if (tran == null)
        //                ds = db.ExecuteDataSet(dbCommandWrapper);
        //            else
        //                ds = db.ExecuteDataSet(dbCommandWrapper, tran);
        //            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count <= 0)
        //            {
        //                return null;
        //            }
        //            dbCommandWrapper.Connection.Close();
        //            dbCommandWrapper.Connection.Dispose();
        //            return DataUtil.ToList<VehicleGpsInfo>(ds.Tables[0]);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //        finally
        //        {
        //            if (dbCommandWrapper != null)
        //            {
        //                dbCommandWrapper = null;
        //            }
        //            if (ds != null)
        //            {
        //                ds = null;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public List<VehicleGpsInfo> GetList(long CarId, DateTime StartTime, DateTime EndTime, DbTransaction tran, Database db)
        {
            DataSet ds = new DataSet();
            SqlParameter[] parameters = {
                new SqlParameter("@_car_id", SqlDbType.Int) ,
                new SqlParameter("@_sTime", SqlDbType.DateTime) ,
                new SqlParameter("@_eTime", SqlDbType.DateTime) ,
                new SqlParameter("@_qType", SqlDbType.Int) 
          
            };

            parameters[0].Value = CarId;
            parameters[1].Value = StartTime;
            parameters[2].Value = EndTime;
            parameters[3].Value = 2;
            ds = SqlServerHelper.ExecuteDataSet("P_QueryCarGpsInfo", CommandType.StoredProcedure, 600, parameters);
            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count <= 0)
            {
                return null;
            }
            return DataUtil.ToList<VehicleGpsInfo>(ds.Tables[0]);
        }
    }
}
