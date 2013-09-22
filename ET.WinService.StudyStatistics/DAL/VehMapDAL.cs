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
    /// 车辆对照表操作类
    /// </summary>
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：
    /// 版本号：1.0.0.1
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 日  期：2013年1月24日
    /// 修  改：
    /// 原  因：
    /// ------------------------------------------------------------------------------
    /// </remarks>
    /// <example>
    /// [示例代码在这里写入]
    /// </example>
    public class VehMapDAL
    {
        public VehMapDAL()
        {
        }

        /// <summary>
        /// 描  述：由 DeviceNo获取列表
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月24日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="gpsDbName">HB_JP_GPSDB</param>
        /// <param name="deviceNos"></param>
        /// <returns></returns>
        public List<VehMap> GetLisByDeviceNos(string gpsDbName, string deviceNos)
        {
            DataSet ds = null;
            DbCommand dbCommandWrapper = null;
            try
            {
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.DefaultConnectionstring);
                dbCommandWrapper = db.DbProviderFactory.CreateCommand();
                dbCommandWrapper.CommandType = CommandType.Text;
                string selectSql = string.Format(@"SELECT M.VehID, GVeh.ID as MapVehID,M.C_FactoryNo as DeviceNo
                                                  FROM (SELECT a.C_FactoryNo, MIN(b.ID) VehID
                                                    FROM T_BaseDevCarMachine a
                                                    INNER JOIN T_Vehicle b ON b.V_VehicleNo=a.VehicleNo
                                                    GROUP BY a.C_FactoryNo
                                                  ) M
                                                  INNER JOIN (SELECT RIGHT(CommNo,8) CommNo_8,MIN(ID) ID 
                                                                FROM {0}.dbo.MSC_Vehicle 
                                                                    GROUP BY RIGHT(CommNo,8)) GVeh ON GVeh.CommNo_8=M.C_FactoryNo
                                                  where M.C_FactoryNo in ({1})", gpsDbName, deviceNos);
                dbCommandWrapper.CommandText = selectSql;
                ds = db.ExecuteDataSet(dbCommandWrapper);
                if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count <= 0)
                {
                    return null;
                }
                return DataUtil.ToList<VehMap>(ds.Tables[0]);
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
