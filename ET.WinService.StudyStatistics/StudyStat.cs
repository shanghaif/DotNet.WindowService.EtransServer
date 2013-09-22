using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

using log4net;
using ET.WinService.StudyStatistics.DAL;
using ET.WinService.StudyStatistics.Entity;
using ET.WinService.Core.Data;
using ET.WinService.Core.Utility;
using ET.WinService.Core.Exceptions;

namespace ET.WinService.StudyStatistics
{

    /// <summary>
    /// 学时统计
    /// </summary>
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：
    /// 版本号：1.0.0.1
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 日  期：2013年1月11日
    /// 修  改：
    /// 原  因：
    /// ------------------------------------------------------------------------------
    /// </remarks>
    /// <example>
    /// [示例代码在这里写入]
    /// </example>
    public class StudyStat
    {
        protected ILog log = LogManager.GetLogger(typeof(StudyStat));
        //private Logger log = new Logger("StudyStat");
        private StudyRecordResultDAL studyRecordResultDAL;
        private StudyRecordResultAdditionDAL studyRecordResultAdditionDAL;
        private StuExamResultDAL stuExamResultDAL;
        private StudyStatStudentDAL studyStatStudentDAL;
        private StudyStatByDayDAL studyStatByDayDAL;
        private StudyRecordInvalidDAL studyRecordInvalidDAL;
        private StudyRecordReclacDAL studyRecordReclacDAL;


        public StudyStat()
        {
            studyRecordResultDAL = new StudyRecordResultDAL();
            studyRecordResultAdditionDAL = new StudyRecordResultAdditionDAL();
            stuExamResultDAL = new StuExamResultDAL();
            studyStatStudentDAL = new StudyStatStudentDAL();
            studyStatByDayDAL = new StudyStatByDayDAL();
            studyRecordInvalidDAL = new StudyRecordInvalidDAL();
            studyRecordReclacDAL = new StudyRecordReclacDAL();
        }

        /// <summary>
        /// 描  述：进行学时统计
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月18日
        /// 修  改：
        /// 原  因：
        /// </summary>
        public void Execute()
        {
            try
            {
                log.Info("……开始执行……");
                //1、清空需要重算学员的学习时间
                ResetStudyTimeByReclac();

                //2、计算里程与有效速度
                StudyRecordCalc();

                //3、学时统计
                 STimeStat();

                //4、生成学习成绩统计
                CreateStudyExamResult();

                //5、清空学员学习时间重算表
                ClearStudyRecordReclac();
                log.Info("……结束执行……");

            }
            catch (Exception ex)
            {
                if (!(ex is NullException))
                {
                    log.Error(ex);
                }
            }
        }

        /// <summary>
        /// 描  述：清空需要重算学员的学习时间
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月11日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void ResetStudyTimeByReclac()
        {
            DbConnection _dbConnection = null;              //定义数据库连接，为事务使用
            DbTransaction transaction = null;               //定义事务
            try
            {
                log.Info("【1、重设需要重算学员的学习时间】");
                //获取需要重算的记录
                IList<TempStudyRecord> studyRecord = studyRecordResultDAL.GetReclacList();
                if (null != studyRecord)
                {
                    //启用事务
                    _dbConnection = DBO.GetInstance().CreateConnection();
                    _dbConnection.Open();
                    transaction = _dbConnection.BeginTransaction();
                    string studyRecordIDs = string.Join(",", studyRecord.Select(o => o.StudyRecordID).Distinct().ToArray<int>());
                    //更新表w_StudyRecord_Result状态 
                    studyRecordResultDAL.UpdateHasStat(studyRecordIDs, 0, transaction, DBO.GetInstance());
                    //更新表w_StudyRecord_Result_Addition字段ValidFlag为1
                    studyRecordResultAdditionDAL.UpdateValidFlag(studyRecordIDs, 1, transaction, DBO.GetInstance());

                    foreach (var item in studyRecord)
                    {
                        //删除表T_StuExamResult相关记录
                        stuExamResultDAL.Delete(item.StuID, item.SubjectID, transaction, DBO.GetInstance());
                        //删除表T_StudyStat_Student相关记录
                        studyStatStudentDAL.Delete(item.StuID, item.SubjectID, transaction, DBO.GetInstance());
                        //删除表T_StudyStatByDay相关记录
                        studyStatByDayDAL.Delete(item.StuID, item.SubjectID, transaction, DBO.GetInstance());
                    }

                    //删除表T_StudyRecord_Invalid相关记录
                    studyRecordInvalidDAL.Delete(studyRecordIDs, transaction, DBO.GetInstance());

                    //结束事务
                    transaction.Commit();
                }
                else
                {
                    log.Info("没有需要重算的记录");
                }
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    //回滚事务
                    transaction.Rollback();
                }
                throw new Exception(string.Format("出错：【重设需要重算学员的学习时间】--{0}", ex.Message));

            }
            finally
            {
                if (_dbConnection != null)
                {
                    //关闭连接
                    _dbConnection.Close();
                }
            }

        }

        /// <summary>
        /// 描  述：计算里程与有效速度
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月11日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void StudyRecordCalc()
        {
            //计算里程与有效速度
            log.Info("【2、计算里程与有效速度】");
            studyRecordResultAdditionDAL.StudyRecordCalc();
        }

        /// <summary>
        /// 描  述：学时统计
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月11日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void STimeStat()
        {
            log.Info("【3、学时统计】");
            //先验证学时有效性
            ValidityCheck validityCheck = new ValidityCheck();
            validityCheck.validate();
            //学时统计
            StudyTimeStat studyTimeStat = new StudyTimeStat();
            studyTimeStat.Stat();
        }

        /// <summary>
        /// 描  述：生成学习成绩统计
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月11日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void CreateStudyExamResult()
        {
            //生成学习成绩统计
            log.Info("【4、生成学习成绩统计】");
            stuExamResultDAL.StuExamResultBuild();
        }

        /// <summary>
        /// 描  述：清空学员学习时间重算表
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月11日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void ClearStudyRecordReclac()
        {
            //清空学员学习时间重算表
            log.Info("【5、清空学员学习时间重算表】");
            studyRecordReclacDAL.clear();
        }


    }
}
