using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using ET.WinService.Core.Data;
using ET.WinService.MileageStatistics.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ET.WinService.MileageStatistics.DAL
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
                                            AND WSRA.HasCalcMileage = 0";
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
        /// 写入学历记录的里程信息
        /// <para>里程统计标识：HasCalcMileage设为1</para>
        /// <para>写入最高速度MaxSpeed及里程Mileage</para>
        /// </summary>
        /// <param name="studyRecordID">学时ID</param>
        /// <param name="flag">是否通过检查</param>
        public int Update_MaxSpeed_Mileage_HasCalcMileage(long studyRecordID, int maxSpeed, int mileage)
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
                                         SET MaxSpeed = @MaxSpeed AND Mileage = @Mileage AND HasCalcMileage = 1
                                         WHERE StudyRecordID = @StudyRecordID";
                    dbCommandWrapper.CommandText = selectSql;
                    db.AddInParameter(dbCommandWrapper, "@MaxSpeed", DbType.Int32, maxSpeed);
                    db.AddInParameter(dbCommandWrapper, "@Mileage", DbType.Int32, mileage);
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
    }
}
