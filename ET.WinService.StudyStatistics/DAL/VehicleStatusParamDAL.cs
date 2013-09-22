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

    /// <summary>
    /// 车辆状态参数操作类
    /// </summary>
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：
    /// 版本号：1.0.0.1
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 日  期：2013年1月17日
    /// 修  改：
    /// 原  因：
    /// ------------------------------------------------------------------------------
    /// </remarks>
    /// <example>
    /// [示例代码在这里写入]
    /// </example>
    public class VehicleStatusParamDAL
    {
        public VehicleStatusParamDAL()
        {
        }

        /// <summary>
        /// 描  述：获取列表数据
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月17日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <returns></returns>
        public List<VehicleStatusParam> GetList()
        {
                
                DataSet ds = null;
                DbCommand dbCommandWrapper = null;
                try
                {
                    Database db = DatabaseFactory.CreateDatabase(ConnectionControl.DefaultConnectionstring);
                    dbCommandWrapper = db.DbProviderFactory.CreateCommand();
                    dbCommandWrapper.CommandType = CommandType.Text;
                    string selectSql = @"select * from T_StudyStatParam_VehicleStatus";
                    dbCommandWrapper.CommandText = selectSql;
                    ds = db.ExecuteDataSet(dbCommandWrapper);
                    if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count <= 0)
                    {
                        return null;
                    }
                    return DataUtil.ToList<VehicleStatusParam>(ds.Tables[0]);
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
