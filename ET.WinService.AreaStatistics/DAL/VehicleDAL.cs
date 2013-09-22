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
    /// 描  述： Vehicle操作类
    /// 作  者：陈炯炯 (chenjiongjiong@e-trans.com.cn)
    /// 时  间：2013年1月29日
    /// 修  改：
    /// 原  因：
    /// </summary>
    /// <returns></returns>
    public  class VehicleDAL
    {
        public List<Vehicle> GetList()
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
        public List<Vehicle> GetList(DbTransaction tran, Database db)
        {
            try
            {
                DataSet ds = null;
                DbCommand dbCommandWrapper = null;
                try
                {
                    dbCommandWrapper = db.DbProviderFactory.CreateCommand();
                    dbCommandWrapper.CommandType = CommandType.Text;
                    string selectSql = @"select ID as VehicleID,substring(convert(varchar(50),commno,120),3,9) as CarId from dbo.MSC_Vehicle ";
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
                    return DataUtil.ToList<Vehicle>(ds.Tables[0]);
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
