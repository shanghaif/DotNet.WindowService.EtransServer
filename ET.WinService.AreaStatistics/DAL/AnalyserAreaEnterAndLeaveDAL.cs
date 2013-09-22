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
    /// 描  述： AnalyserAreaEnterAndLeaveMonitor操作类
    /// 作  者：陈炯炯 (chenjiongjiong@e-trans.com.cn)
    /// 时  间：2013年1月15日
    /// 修  改：
    /// 原  因：
    /// </summary>
    /// <returns></returns>
    public class AnalyserAreaEnterAndLeaveDAL
    {

        public List<AnalyserAreaEnterAndLeaveMonitor> GetList()
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.GPSDBConnetionstring);
                return this.GetList((DbTransaction)null, db);
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
        public List<AnalyserAreaEnterAndLeaveMonitor> GetList(DbTransaction tran, Database db)
        {
            try
            {
                DataSet ds = null;
                DbCommand dbCommandWrapper = null;
                try
                {
                    dbCommandWrapper = db.DbProviderFactory.CreateCommand();
                    dbCommandWrapper.CommandType = CommandType.Text;
                   
                    string selectSql = @"select ID,AnalyseGroupID,AreaID,AllowLeaveMaxTime from dbo.ANA_AnalyserCheckVehicleInAreaDaliy";
                    dbCommandWrapper.CommandText = selectSql;
                    if (tran == null)
                        ds = db.ExecuteDataSet(dbCommandWrapper);
                    else
                        ds = db.ExecuteDataSet(dbCommandWrapper, tran);
                    if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count <= 0)
                    {
                        return null;
                    }
                    dbCommandWrapper.Connection.Close();
                    dbCommandWrapper.Connection.Dispose();
                    return DataUtil.ToList<AnalyserAreaEnterAndLeaveMonitor>(ds.Tables[0]);
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

        public List<AnalyserAreaEnterAndLeaveMonitor> GetList(long AnalyseGroupID)
        {
            try
            {
                DataSet ds = null;
                DbCommand dbCommandWrapper = null;
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.GPSDBConnetionstring);
                try
                {
                    dbCommandWrapper = db.DbProviderFactory.CreateCommand();
                    dbCommandWrapper.CommandType = CommandType.Text;
                    string selectSql = @"select ID,AnalyseGroupID,AreaID,AllowLeaveMaxTime from dbo.ANA_AnalyserCheckVehicleInAreaDaliy where AnalyseGroupID=@AnalyseGroupID";
                    dbCommandWrapper.CommandText = selectSql;
                    #region Add parameters
                    db.AddInParameter(dbCommandWrapper, "@AnalyseGroupID", DbType.Int64, AnalyseGroupID);
                    #endregion
                    ds = db.ExecuteDataSet(dbCommandWrapper);
                  
                    if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count <= 0)
                    {
                        return null;
                    }
                    return DataUtil.ToList<AnalyserAreaEnterAndLeaveMonitor>(ds.Tables[0]);
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
