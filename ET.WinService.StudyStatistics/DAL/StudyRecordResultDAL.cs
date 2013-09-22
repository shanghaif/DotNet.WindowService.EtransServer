using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data;
using System.Collections;

using ET.WinService.StudyStatistics.Entity;
using ET.WinService.Core.Data;

namespace ET.WinService.StudyStatistics.DAL
{

    /// <summary>
    /// 学习结果数据操作类
    /// </summary>
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：
    /// 版本号：1.0.0.1
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 日  期：2013年1月15日
    /// 修  改：
    /// 原  因：
    /// ------------------------------------------------------------------------------
    /// </remarks>
    /// <example>
    /// [示例代码在这里写入]
    /// </example>
    public class StudyRecordResultDAL
    {
        public StudyRecordResultDAL()
        {
        }

        /// <summary>
        /// 描  述：获取需要重算学时的列表对象
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月14日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<TempStudyRecord> GetReclacList()
        {
            DataSet ds = null;
            DbCommand dbCommandWrapper = null;
            try
            {
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.DefaultConnectionstring);
                dbCommandWrapper = db.DbProviderFactory.CreateCommand();
                dbCommandWrapper.CommandType = CommandType.Text;
                string selectSql = @"SELECT a.ID as StudyRecordID, b.ID as StuID, s.ID as SubjectID,s.SJ_Name as SubjectName
                                            FROM w_StudyRecord_Result a
                                            INNER JOIN T_Student_Base b ON a.S_No = b.S_No
                                            INNER JOIN T_SysSubject s ON s.SJ_StudyType = a.SR_StudyType
                                            INNER JOIN T_StudyRecordReclac rc ON rc.StuID=b.ID AND rc.SubjectID=s.ID";
                dbCommandWrapper.CommandText = selectSql;

                ds = db.ExecuteDataSet(dbCommandWrapper);
                if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count <= 0)
                {
                    return null;
                }
                return DataUtil.ToList<TempStudyRecord>(ds.Tables[0]);
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

        /// <summary>
        /// 描  述：获取还没有统计的记录列表(for验证有效性)
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月15日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <returns></returns>
        public List<StudyRecord> GetNotStatForValidList()
        {
            DataSet ds = null;
            DbCommand dbCommandWrapper = null;
            try
            {
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.DefaultConnectionstring);
                dbCommandWrapper = db.DbProviderFactory.CreateCommand();
                dbCommandWrapper.CommandType = CommandType.Text;
                string selectSql = @"SELECT a.ID as StudyRecordID, b.ID as StuID, s.ID as SubjectID, a.SR_RealTime, a.SR_ActualTime, a.SR_BeginTime, a.SR_EndTime,
                                                  a.T_Teacher, a.DeviceNo, ISNULL(aa.CalcFlag,1) as CalcFlag_Add,ValidFlag=CASE WHEN (ISNULL(b.S_Status,'')<>'已激活') THEN 8192 
                                                  WHEN (ISNULL(tch.T_Flag,'')<>'已激活') THEN 16384 ELSE 1 END
                                                 ,ISNULL(aa.StudyType,1) StudyType, ISNULL(aa.ValidSpeed,0) ValidSpeed, ISNULL(aa.HasCalcVehStatus,0) HasCalcVehStatus, 
                                                  ISNULL(aa.ValidVehStatus,0) ValidVehStatus,tch.TeacherType,a.SR_StudyType
                                                        FROM w_StudyRecord_Result a
                                                            INNER JOIN T_Student_Base b ON a.S_No = b.S_No
                                                                    AND a.HasStat<>1 
                                                            INNER JOIN T_SysSubject s ON s.SJ_StudyType = a.SR_StudyType
                                                            LEFT JOIN w_StudyRecord_Result_Addition aa ON aa.StudyRecordID=a.ID
                                                            LEFT JOIN T_Teacher tch ON tch.T_TeacherNo=a.T_Teacher";
                dbCommandWrapper.CommandText = selectSql;

                ds = db.ExecuteDataSet(dbCommandWrapper);
                if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count <= 0)
                {
                    return null;
                }
                return DataUtil.ToList<StudyRecord>(ds.Tables[0]);
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

        /// <summary>
        /// 描  述：获取还没有统计的记录列表
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月21日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <returns></returns>
        public List<StudyRecord> GetNotStatList()
        {
            DataSet ds = null;
            DbCommand dbCommandWrapper = null;
            try
            {
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.DefaultConnectionstring);
                dbCommandWrapper = db.DbProviderFactory.CreateCommand();
                dbCommandWrapper.CommandType = CommandType.Text;
                string selectSql = @"SELECT a.ID as StudyRecordID, b.ID as StuID, s.ID as SubjectID, a.SR_RealTime, 
                                            a.SR_ActualTime, a.SR_BeginTime, a.SR_EndTime, a.T_Teacher, a.DeviceNo, 
                                            ISNULL(aa.CalcFlag,1) as CalcFlag,isnull(aa.StudyType,1) as StudyType,1 as ValidFlag,aa.ValidSpeed,aa.Mileage
                                          FROM w_StudyRecord_Result a
                                          INNER JOIN T_Student_Base b ON a.S_No = b.S_No
                                            AND a.HasStat<>1
                                          INNER JOIN T_SysSubject s ON s.SJ_StudyType = a.SR_StudyType
                                          LEFT JOIN w_StudyRecord_Result_Addition aa ON aa.StudyRecordID=a.ID";
                dbCommandWrapper.CommandText = selectSql;

                ds = db.ExecuteDataSet(dbCommandWrapper);
                if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count <= 0)
                {
                    return null;
                }
                return DataUtil.ToList<StudyRecord>(ds.Tables[0]);
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

        public ArrayList GetRecordIDByTeacher(string recordIDs)
        {
            DataSet ds = null;
            DbCommand dbCommandWrapper = null;
            ArrayList rtnValue = new ArrayList();
            try
            {
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.DefaultConnectionstring);
                dbCommandWrapper = db.DbProviderFactory.CreateCommand();
                dbCommandWrapper.CommandType = CommandType.Text;
                dbCommandWrapper.CommandTimeout = 600;
                string selectSql = string.Format(@"SELECT a.ID as StudyRecordID from w_StudyRecord_Result a
                                              INNER JOIN T_Student_Base b ON a.S_No = b.S_No
                                              WHERE a.ID in({0}) and NOT EXISTS
                                                 (SELECT 1 FROM T_Teacher c WHERE c.T_TeacherNo=a.T_Teacher
                                                AND PATINDEX('%,'+b.S_StudyVehicleType+',%',','+dbo.F_GetVehicleTypeNameList(c.T_TechVehicleType)+',')>0)", recordIDs);
                dbCommandWrapper.CommandText = selectSql;
                ds = db.ExecuteDataSet(dbCommandWrapper);
                if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count <= 0)
                {
                    return null;
                }
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    rtnValue.Add(int.Parse(ds.Tables[0].Rows[i][0].ToString()));
                }
                return rtnValue;
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

        /// <summary>
        /// 描  述：update表w_StudyRecord_Result字段HasStat值
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月14日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="studyRecordIDs">以逗号隔开的ID字符串</param>
        /// <param name="tran"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public int UpdateHasStat(string studyRecordIDs, short value, DbTransaction tran, Database db)
        {
            DbCommand dbCommandWrapper = null;
            try
            {
                string updateSql = string.Format(@" UPDATE w_StudyRecord_Result SET HasStat={0} WHERE ID in({1})", value, studyRecordIDs);
                dbCommandWrapper = db.GetSqlStringCommand(updateSql);

                if (tran == null)
                    return db.ExecuteNonQuery(dbCommandWrapper);
                else
                    return db.ExecuteNonQuery(dbCommandWrapper, tran);
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

        /// <summary>
        /// 描  述：有效性检查后更新数据
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月18日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="studyRecord"></param>
        /// <param name="tran"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public int UpdateCheckedState(StudyRecord studyRecord, DbTransaction tran, Database db)
        {
            DbCommand dbCommandWrapper = null;
            try
            {
                string updateSql = string.Format(@"UPDATE w_StudyRecord_Result SET StudyRate=@StudyRate,ValidActualTime=@ValidActualTime,HasStat=1
                                                           WHERE ID=@ID");
                dbCommandWrapper = db.GetSqlStringCommand(updateSql);
                #region Add parameters
                db.AddInParameter(dbCommandWrapper, "@StudyRate", DbType.Double, studyRecord.StudyRate);
                db.AddInParameter(dbCommandWrapper, "@ValidActualTime", DbType.Double, studyRecord.ValidActualTime);
                db.AddInParameter(dbCommandWrapper, "@ID", DbType.Int32, studyRecord.StudyRecordID);
                #endregion


                if (tran == null)
                    return db.ExecuteNonQuery(dbCommandWrapper);
                else
                    return db.ExecuteNonQuery(dbCommandWrapper, tran);
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
