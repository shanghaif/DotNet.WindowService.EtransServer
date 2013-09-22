using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using ET.WinService.Core.Data;
using ET.WinService.VehicleStatusStatistics.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ET.WinService.VehicleStatusStatistics.DAL
{
    public class StudyRecordDAL
    {
        /// <summary>
        /// 获取上一天未分析过的学习记录
        /// </summary>
        /// <param name="analyseStartTime">开始时间</param>
        /// <param name="analyseEndTime">结束时间</param>
        public List<StudyRecord> GetUnValidStudentRecord(DateTime analyseStartTime, DateTime analyseEndTime)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.DefaultConnectionstring);
                DataSet ds = null;
                DbCommand dbCommandWrapper = null;
                try
                {
                    dbCommandWrapper = db.DbProviderFactory.CreateCommand();
                    dbCommandWrapper.CommandType = CommandType.Text;
                    string selectSql = @" 
                                    SELECT 
                                        WSR.ID, 
                                        WSR.DeviceNo,
                                        WSR.SR_BeginTime,
                                        WSR.SR_EndTime
                                    FROM w_StudyRecord_Result WSR 
                                    INNER JOIN w_StudyRecord_Result_Addition WSRA
                                        ON WSR.ID = WSRA.StudyRecordID 
                                            AND WSRA.StudyType = 1 
                                            AND (WSR.SR_BeginTime BETWEEN @startTime AND @endTime) 
                                            AND WSR.HasCalcVehStatus = 0";
                    dbCommandWrapper.CommandText = selectSql;
                    db.AddInParameter(dbCommandWrapper, "@startTime", DbType.DateTime, analyseStartTime);
                    db.AddInParameter(dbCommandWrapper, "@endTime", DbType.DateTime, analyseEndTime);
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 写入学习记录的车辆状态标识
        /// 车辆状态统计标识：HasCalcVehStatus设为1
        /// 通过检查：ValidVehStatus设为1
        /// 不通过检查：ValidVehStatus设为0
        /// </summary>
        /// <param name="studyRecordID">学时ID</param>
        /// <param name="validVehStatus">是否通过检查</param>
        public int Update_ValidVehStatus_HasCalcVehStatus(long studyRecordID, bool validVehStatus)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.DefaultConnectionstring);
                DataSet ds = null;
                DbCommand dbCommandWrapper = null;
                try
                {
                    dbCommandWrapper = db.DbProviderFactory.CreateCommand();
                    dbCommandWrapper.CommandType = CommandType.Text;
                    string selectSql = @"UPDATE [w_StudyRecord_Result_Addition] 
                                         SET ValidVehStatus = @ValidVehStatus AND HasCalcVehStatus = 1
                                         WHERE StudyRecordID = @StudyRecordID";
                    dbCommandWrapper.CommandText = selectSql;
                    db.AddInParameter(dbCommandWrapper, "@ValidVehStatus", DbType.Boolean, validVehStatus);
                    db.AddInParameter(dbCommandWrapper, "@StudyRecordID", DbType.Int64, studyRecordID);
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

        /// <summary>
        /// 更新学习记录的统计标识为已统计
        /// <para>HasStat设为1</para>
        /// </summary>
        /// <param name="p"></param>
        public int Update_HasStat(long studyRecordID)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.DefaultConnectionstring);
                DataSet ds = null;
                DbCommand dbCommandWrapper = null;
                try
                {
                    dbCommandWrapper = db.DbProviderFactory.CreateCommand();
                    dbCommandWrapper.CommandType = CommandType.Text;
                    string selectSql = @"UPDATE [w_StudyRecord_Result] 
                                         SET HasStat = 1
                                         WHERE StudyRecordID = @StudyRecordID";
                    dbCommandWrapper.CommandText = selectSql;
                    db.AddInParameter(dbCommandWrapper, "@StudyRecordID", DbType.Int64, studyRecordID);
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

        /// <summary>
        /// 无效类型：非法车辆状态
        /// </summary>
        /// <param name="p"></param>
        public int Insert_StudyRecord_Invalid(long studyRecordID, DateTime startTime, DateTime endTime)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.DefaultConnectionstring);
                DataSet ds = null;
                DbCommand dbCommandWrapper = null;
                try
                {
                    dbCommandWrapper = db.DbProviderFactory.CreateCommand();
                    dbCommandWrapper.CommandType = CommandType.Text;
                    string selectSql = @"INSERT INTO [T_StudyRecord_Invalid] 
                                            (StudyRecordID, InvalidActualTime, TypeName, Remark)
                                        VALUES
                                            (@StudyRecordID, @InvalidActualTime, @TypeName, @Remark)";
                    dbCommandWrapper.CommandText = selectSql;
                    db.AddInParameter(dbCommandWrapper, "@StudyRecordID", DbType.Int64, studyRecordID);
                    db.AddInParameter(dbCommandWrapper, "@InvalidActualTime", DbType.Decimal, (endTime - startTime).TotalMinutes);
                    db.AddInParameter(dbCommandWrapper, "@TypeName", DbType.String, "非法车辆状态");
                    db.AddInParameter(dbCommandWrapper, "@Remark", DbType.String, string.Format("无效的学习时间段：{0}~{1}", startTime, endTime));
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
