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
    public class StuExamResultDAL
    {
        public StuExamResultDAL()
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
        public int Delete(int stuID,int subjectID, DbTransaction tran, Database db)
        {
            DbCommand dbCommandWrapper = null;
            try
            {
                string deleteSql = @"DELETE FROM T_StuExamResult WHERE StuID=@StuID AND SubjectID=@SubjectID";
                dbCommandWrapper = db.GetSqlStringCommand(deleteSql);

                #region Add parameters
                db.AddInParameter(dbCommandWrapper, "@StuID", DbType.Int32, stuID);
                db.AddInParameter(dbCommandWrapper, "@SubjectID", DbType.Int32,subjectID);
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
        /// 描  述：生成学习成绩统计信息
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月28日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <returns></returns>
        public int StuExamResultBuild()
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.DefaultConnectionstring);
                DbCommand dbCommandWrapper = null;
                try
                {
                    dbCommandWrapper = db.GetStoredProcCommand("P_T_StuExamResult_Build");
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
