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
    public class StudyDrivingSubjectDAL
    {
        public StudyDrivingSubjectDAL()
        {
        }

        public StudyDrivingSubject GetEntity(int pirorStuID, int pirorSubjectID)
        {

            DataSet ds = null;
            DbCommand dbCommandWrapper = null;
            try
            {
                Database db = DatabaseFactory.CreateDatabase(ConnectionControl.DefaultConnectionstring);
                dbCommandWrapper = db.DbProviderFactory.CreateCommand();
                dbCommandWrapper.CommandType = CommandType.Text;
                string selectSql = @" SELECT b.StudyRate,b.StudyRate_Theoretical,b.StudyRate_Simulate,b.StudyRate_Operate,
                                             b.StudyTime_Operate*60 as StudyTime_Operate,
                                             b.StudyTime_Simulate*60 as StudyTime_Simulate,
                                             b.StudyTime_Theoretical*60 as StudyTime_Theoretical
                                                FROM T_Student_Base a
                                                INNER JOIN T_StudyDrivingSubject b
                                                  ON a.ID=@PirorStuID AND b.DrivingType=a.S_StudyVehicleType 
                                                        AND b.SubjectID=@PirorSubjectID";
                dbCommandWrapper.CommandText = selectSql;
                #region Add parameters
                db.AddInParameter(dbCommandWrapper, "@PirorStuID", DbType.Int32, pirorStuID);
                db.AddInParameter(dbCommandWrapper, "@PirorSubjectID", DbType.Int32, pirorSubjectID);
                #endregion
                ds = db.ExecuteDataSet(dbCommandWrapper);
                if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count <= 0)
                {
                    return null;
                }
                return DataUtil.ToEntity<StudyDrivingSubject>(ds.Tables[0].Rows[0], null);
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
