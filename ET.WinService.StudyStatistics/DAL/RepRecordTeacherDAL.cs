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
    public class RepRecordTeacherDAL
    {
        public RepRecordTeacherDAL()
        {
        }

        /// <summary>
        /// 描  述：获取教练教学数据列表
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月17日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="teachers"></param>
        /// <returns></returns>
        public List<RepRecordTeacher> GetLisByTeachers(string teachers)
        {
            DataSet ds = null;
            DbCommand dbCommandWrapper = null;
            try
            {
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.DefaultConnectionstring);
                dbCommandWrapper = db.DbProviderFactory.CreateCommand();
                dbCommandWrapper.CommandType = CommandType.Text;
                string selectSql = string.Format(@"SELECT a.ID as StudyRecordID, b.ID as StuID, s.ID as SubjectID, a.SR_ActualTime, a.SR_BeginTime, a.SR_EndTime, a.T_Teacher, ISNULL(e.ValidFlag,1) as ValidFlag, a.HasStat,c.TeacherType,e.StudyType
                                              FROM w_StudyRecord_Result a
                                              INNER JOIN T_Teacher c on a.T_Teacher=c.T_TeacherNo
                                              INNER JOIN T_Student_Base b ON a.S_No = b.S_No
                                              INNER JOIN T_SysSubject s ON s.SJ_StudyType = a.SR_StudyType
                                              INNER JOIN w_StudyRecord_Result_Addition e ON e.StudyRecordID=a.ID
                                           WHERE a.T_Teacher in ({0})
                                              ORDER BY a.T_Teacher,a.SR_BeginTime,a.ID", teachers);
                dbCommandWrapper.CommandText = selectSql;
                ds = db.ExecuteDataSet(dbCommandWrapper);
                if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count <= 0)
                {
                    return null;
                }
                return DataUtil.ToList<RepRecordTeacher>(ds.Tables[0]);
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
