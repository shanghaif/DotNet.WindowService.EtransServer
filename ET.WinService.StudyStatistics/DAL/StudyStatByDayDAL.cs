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
    public class StudyStatByDayDAL
    {
        public StudyStatByDayDAL()
        {
        }

        /// <summary>
        /// 描  述：删除数据
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月14日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="stuID"></param>
        /// <param name="subjectID"></param>
        /// <param name="tran"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public int Delete(int stuID, int subjectID, DbTransaction tran, Database db)
        {
            DbCommand dbCommandWrapper = null;
            try
            {
                string deleteSql = @"DELETE FROM T_StudyStatByDay WHERE StuID=@StuID AND SubjectID=@SubjectID";
                dbCommandWrapper = db.GetSqlStringCommand(deleteSql);

                #region Add parameters
                db.AddInParameter(dbCommandWrapper, "@StuID", DbType.Int32, stuID);
                db.AddInParameter(dbCommandWrapper, "@SubjectID", DbType.Int32, subjectID);
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
        /// 描  述：由学员ID获取列表数据
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月21日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="stuIDs"></param>
        /// <returns></returns>
        public List<StudyStatByDay> GetLisByStuIDs(string stuIDs)
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
                    string selectSql = string.Format(@"SELECT * FROM T_StudyStatByDay
                                                            WHERE StuID in ({0})", stuIDs);
                    dbCommandWrapper.CommandText = selectSql;
                    ds = db.ExecuteDataSet(dbCommandWrapper);
                    if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count <= 0)
                    {
                        return null;
                    }
                    return DataUtil.ToList<StudyStatByDay>(ds.Tables[0]);
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
        /// 描  述：学习统计后更新数据
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月25日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tran"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public int UpdateStatState(StudyStatByDay entity, DbTransaction tran, Database db)
        {
            DbCommand dbCommandWrapper = null;
            try
            {
                string updateSql = @"IF NOT EXISTS (SELECT ID FROM T_StudyStatByDay 
				                                    WHERE StatDate=@StatDate AND StuID=@StuID AND SubjectID=@SubjectID AND StudyType=@StudyType)
	                                    INSERT INTO T_StudyStatByDay(StuID,StatDate,SubjectID,SR_RealTime,SR_ActualTime,ValidActualTime
                                                ,InvalidActualTime,ValidStudyTime_Exam,StudyRate,StudyType)
		                                            VALUES(@StuID,@StatDate,@SubjectID,@SR_RealTime,@SR_ActualTime,@ValidActualTime
                                                ,@InvalidActualTime,@ValidStudyTime_Exam,@StudyRate,@StudyType)
                                    ELSE
                                       UPDATE T_StudyStatByDay SET SR_RealTime=@SR_RealTime, SR_ActualTime=@SR_ActualTime, ValidActualTime=@ValidActualTime, 
		                                    InvalidActualTime=@InvalidActualTime,ValidStudyTime_Exam=@ValidStudyTime_Exam, HasStatExam=0
                                       WHERE StatDate=@StatDate AND StuID=@StuID AND SubjectID=@SubjectID AND StudyType=@StudyType";
                dbCommandWrapper = db.GetSqlStringCommand(updateSql);
                #region Add parameters
                db.AddInParameter(dbCommandWrapper, "@StuID", DbType.Int32, entity.StuID);
                db.AddInParameter(dbCommandWrapper, "@SubjectID", DbType.Int32, entity.SubjectID);
                db.AddInParameter(dbCommandWrapper, "@StatDate", DbType.DateTime, entity.StatDate);
                db.AddInParameter(dbCommandWrapper, "@SR_RealTime", DbType.Double, entity.SR_RealTime);
                db.AddInParameter(dbCommandWrapper, "@SR_ActualTime", DbType.Double, entity.SR_ActualTime);
                db.AddInParameter(dbCommandWrapper, "@ValidActualTime", DbType.Double, entity.ValidActualTime);
                db.AddInParameter(dbCommandWrapper, "@InvalidActualTime", DbType.Double, entity.InvalidActualTime);
                db.AddInParameter(dbCommandWrapper, "@ValidStudyTime_Exam", DbType.Double, entity.ValidStudyTime_Exam);
                db.AddInParameter(dbCommandWrapper, "@StudyRate", DbType.Double, entity.StudyRate);
                db.AddInParameter(dbCommandWrapper, "@StudyType", DbType.Int32, entity.StudyType);
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
