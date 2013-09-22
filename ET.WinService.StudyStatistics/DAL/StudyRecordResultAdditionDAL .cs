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
    public class StudyRecordResultAdditionDAL
    {

        public StudyRecordResultAdditionDAL()
        {
        }

        /// <summary>
        /// 描  述：update表w_StudyRecord_Result_Addition字段ValidFlag值
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月14日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="studyRecordIDs"></param>
        /// <param name="validFlag"></param>
        /// <param name="tran"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public int UpdateValidFlag(string studyRecordIDs, short validFlag, DbTransaction tran, Database db)
        {
            DbCommand dbCommandWrapper = null;
            try
            {
                string updateSql = string.Format(@"UPDATE w_StudyRecord_Result_Addition SET ValidFlag={0} WHERE StudyRecordID in ({1})", validFlag, studyRecordIDs);
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
        /// 描  述：学习有效性检查后更新数据
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
                string updateSql = @"IF NOT EXISTS (SELECT StudyRecordID FROM w_StudyRecord_Result_Addition WHERE StudyRecordID=@StudyRecordID)
	                                    INSERT INTO w_StudyRecord_Result_Addition(StudyRecordID,ValidFlag,StudyType,HasCalcVehStatus,ValidVehStatus)
		                                    VALUES(@StudyRecordID,@ValidFlag,@StudyType,@HasCalcVehStatus,@ValidVehStatus)
                                    ELSE
	                                    UPDATE w_StudyRecord_Result_Addition 
                                            SET ValidFlag=@ValidFlag,HasCalcVehStatus=@HasCalcVehStatus, ValidVehStatus=@ValidVehStatus
		                                WHERE StudyRecordID=@StudyRecordID";
                dbCommandWrapper = db.GetSqlStringCommand(updateSql);
                #region Add parameters
                db.AddInParameter(dbCommandWrapper, "@StudyRecordID", DbType.Int32, studyRecord.StudyRecordID);
                db.AddInParameter(dbCommandWrapper, "@ValidFlag", DbType.Int32, studyRecord.ValidFlag);
                db.AddInParameter(dbCommandWrapper, "@StudyType", DbType.Int32, studyRecord.StudyType);
                db.AddInParameter(dbCommandWrapper, "@HasCalcVehStatus", DbType.Int16, studyRecord.HasCalcVehStatus);
                db.AddInParameter(dbCommandWrapper, "@ValidVehStatus", DbType.Int16, studyRecord.ValidVehStatus);
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

        /// <summary>
        /// 描  述：学习时间统计后更新数据
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月25日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="studyRecord"></param>
        /// <param name="tran"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public int UpdateStatState(StudyRecord studyRecord, DbTransaction tran, Database db)
        {
            DbCommand dbCommandWrapper = null;
            try
            {
                string updateSql = @"IF NOT EXISTS (SELECT StudyRecordID FROM w_StudyRecord_Result_Addition WHERE StudyRecordID=@StudyRecordID)
	                                    INSERT INTO w_StudyRecord_Result_Addition(StudyRecordID,ValidFlag)
		                                    VALUES(@StudyRecordID,@ValidFlag)
                                    ELSE
	                                    UPDATE w_StudyRecord_Result_Addition 
                                            SET ValidFlag=@ValidFlag
		                                WHERE StudyRecordID=@StudyRecordID";
                dbCommandWrapper = db.GetSqlStringCommand(updateSql);
                #region Add parameters
                db.AddInParameter(dbCommandWrapper, "@StudyRecordID", DbType.Int32, studyRecord.StudyRecordID);
                db.AddInParameter(dbCommandWrapper, "@ValidFlag", DbType.Int32, studyRecord.ValidFlag);
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

        /// <summary>
        /// 描  述：学习记录行车里程统计
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月28日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <returns></returns>
        public int StudyRecordCalc()
        {

            DbCommand dbCommandWrapper = null;
            try
            {
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.DefaultConnectionstring);
                dbCommandWrapper = db.GetStoredProcCommand("P_StudyRecordCalc_SJZ");
                dbCommandWrapper.CommandTimeout = 600;
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
