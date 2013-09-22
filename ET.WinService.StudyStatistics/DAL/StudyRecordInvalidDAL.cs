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
    public class StudyRecordInvalidDAL
    {
        public StudyRecordInvalidDAL()
        {
        }

        /// <summary>
        /// 描  述：删除数据
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月14日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="studyRecordIDs"></param>
        /// <param name="tran"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public int Delete(string studyRecordIDs, DbTransaction tran, Database db)
        {
            DbCommand dbCommandWrapper = null;
            try
            {
                string deleteSql = string.Format(@"DELETE FROM T_StudyRecord_Invalid WHERE StudyRecordID in ({0})", studyRecordIDs);
                dbCommandWrapper = db.GetSqlStringCommand(deleteSql);

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
        /// 描  述：插入数据
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月18日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tran"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public int Add(StudyRecordInvalid entity, DbTransaction tran, Database db)
        {
            DbCommand dbCommandWrapper = null;
            try
            {
                string deleteSql = @"INSERT INTO T_StudyRecord_Invalid(StudyRecordID, InvalidActualTime,TypeName,Remark)
	                                        VALUES(@StudyRecordID, @InvalidActualTime, @TypeName, @Remark)";
                dbCommandWrapper = db.GetSqlStringCommand(deleteSql);
                #region Add parameters
                db.AddInParameter(dbCommandWrapper, "@StudyRecordID", DbType.Int32, entity.StudyRecordID);
                db.AddInParameter(dbCommandWrapper, "@InvalidActualTime", DbType.Double, entity.InvalidActualTime);
                db.AddInParameter(dbCommandWrapper, "@TypeName", DbType.String, entity.TypeName);
                db.AddInParameter(dbCommandWrapper, "@Remark", DbType.String, entity.Remark);
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
