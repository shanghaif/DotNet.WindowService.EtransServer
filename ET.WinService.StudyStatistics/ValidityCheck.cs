using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.Common;

using ET.WinService.StudyStatistics.DAL;
using ET.WinService.StudyStatistics.Entity;
using ET.WinService.Core.Utility;
using ET.WinService.Core.Data;
using ET.WinService.Core.Extension;
using ET.WinService.Core.Exceptions;
using log4net;

namespace ET.WinService.StudyStatistics
{

    /// <summary>
    /// 学习记录有效性检查
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
    public class ValidityCheck
    {
        // private Logger log = new Logger("StudyStat");
        private ILog log = LogManager.GetLogger(typeof(ValidityCheck));
        #region 数据操作类
        //数据操作类
        private SubjectParamDAL subjectParamDAL;
        private StudyRecordResultDAL studyRecordResultDAL;
        private StudyRecordResultAdditionDAL studyRecordResultAdditionDAL;
        private StudyRecordInvalidDAL studyRecordInvalidDAL;
        private StudyStatFlowSubjectDAL studyStatFlowSubjectDAL;
        private SysSubjectDAL sysSubjectDAL;
        private StuExamApplyDAL stuExamApplyDAL;
        private ReqExamProcessDAL reqExamProcessDAL;
        private RepRecordTeacherDAL repRecordTeacherDAL;
        private VehicleStatusParamDAL vehicleStatusParamDAL;
        private DataSourceDAL dataSourceDAL;
        #endregion

        #region 数据集
        //数据集
        List<SubjectParam> subjectParamList;                    //科目学时参数列表
        List<StudyRecord> studyRecordList;                      //学时列表
        List<StudyRecordInvalid> studyRecordInvalidList;        //学习无效记录列表
        List<StudyStatFlowSubject> studyStatFlowSubjectList;    //学习状态流程列表
        // List<SysSubject> sysSubjectList;                        //科目列表
        List<StuExamApply> stuExamApplyList;                    //报考申请列表
        //List<ReqExamProcess> reqExamProcessList;                //报考审批流程列表
        List<RepRecordTeacher> repRecordTeacherList;            //教练教学数据集
        List<RepRecordStudent> repRecordStudentList;            //学生学习数据集
        // List<VehicleStatusParam> vehicleStatusParamList;        //车辆状态参数列表
        //List<DataSource> dataSourceList;                        //数据源列表
        // List<TrackCommNo> trackCommNoList;
        // List<StudyRecordGpsData> studyRecordGpsDataList;        //Gps数据
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ValidityCheck()
        {
            subjectParamDAL = new SubjectParamDAL();
            studyRecordResultDAL = new StudyRecordResultDAL();
            studyRecordResultAdditionDAL = new StudyRecordResultAdditionDAL();
            studyRecordInvalidDAL = new StudyRecordInvalidDAL();
            studyStatFlowSubjectDAL = new StudyStatFlowSubjectDAL();
            sysSubjectDAL = new SysSubjectDAL();
            stuExamApplyDAL = new StuExamApplyDAL();
            reqExamProcessDAL = new ReqExamProcessDAL();
            repRecordTeacherDAL = new RepRecordTeacherDAL();
            vehicleStatusParamDAL = new VehicleStatusParamDAL();
            dataSourceDAL = new DataSourceDAL();

            //数据集
            studyRecordInvalidList = new List<StudyRecordInvalid>();
        }

        /// <summary>
        /// 描  述：验证有效性公开方法
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月21日
        /// 修  改：
        /// 原  因：
        /// </summary>
        public void validate()
        {
            log.Info("######开始检查学习记录有效性######");
            try
            {
                //获取科目学时参数配置表
                subjectParamList = subjectParamDAL.GetList();
                //获取还没有统计的记录列表
                log.Info("--获取还没有统计的记录列表--");
                studyRecordList = studyRecordResultDAL.GetNotStatForValidList();
                //没有待统计的记录即退出
                if (studyRecordList == null)
                {
                    log.Info("没有待统计的记录");
                    throw new NullException();
                }

                //科目一的StudyType=0,前台程序没有填写(BUG)
                log.Info("--设置科目一的StudyType=0--");
                studyRecordList.ForEach(item =>
                {
                    if (item.SubjectID == 1)
                    {
                        item.StudyType = 0;
                    }
                });
                //未激活卡
                log.Info("--验证未激活卡--");
                SetInvalidCard();
                //只取审批通过后有效的科目
                log.Info("--验证科目未审批--");
                SetValidSubject();
                //异常数据
                log.Info("--验证异常数据--");
                SetAbnormalData();
                //不在规定的时间段内学习
                log.Info("--验证不在规定的时间段内学习--");
                SetNotInSpecifiedTime();
                //单次学时小于最小要求学时
                log.Info("--验证单次学时小于最小要求学时--");
                SetLessThanMinimumHours();
                //不在准教车型范围内
                log.Info("--验证不在准教车型范围内--");
                SetNotInTeachingRange();
                //不在准教科目范围内
                log.Info("--验证不在准教科目范围内--");
                SetNotInSubjectRange();
                //获取教练教车数据，并更新状态
                log.Info("--获取教练教车数据，并更新状态--");
                GetTeacherRecord();
                UpdateTeacherRecord();
                //教练教车重叠时间段
                log.Info("--验证教练教车重叠时间段--");
                SetTeacherOverlappingTime();
                //学员学车重叠时间段
                log.Info("--验证学员学车重叠时间段--");
                SetStudentOverlappingTime();
                //不在有效速度范围内
                log.Info("--验证不在有效速度范围内--");
                SetNotInSpeedRange();
                //非法车辆状态:ACC及发动机状态检查
                // log.Info("--验证非法车辆状态--");
                // SetIllegalVehicleState();

                //更新表数据
                log.Info("--更新表数据--");
                UpdateStudyRecord();
                log.Info("######结束检查学习记录有效性######");
            }
            catch (Exception ex)
            {
                //log.Info(ex.Message);
                throw ex;
            }
        }
        #region 有效性检查

        /// <summary>
        /// 描  述：插入未激活卡记录
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月15日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void SetInvalidCard()
        {
            AddRangeToInvalidList(InvalidType.StudentCardNotActivated);
            AddRangeToInvalidList(InvalidType.TeacherCardNotActivated);
        }

        /// <summary>
        /// 描  述：设置科目未审批
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月15日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void SetValidSubject()
        {
            //学习状态流程列表
            studyStatFlowSubjectList = studyStatFlowSubjectDAL.GetList();
            if (studyStatFlowSubjectList == null)
            {
                log.Info("学习状态流程列表为空");
                return;
            }
            //报考申请列表
            string stuIDs = string.Join(",", studyRecordList.Select(o => o.StuID).Distinct().ToArray());    //stuID以逗号隔开字符串
            if (string.IsNullOrWhiteSpace(stuIDs)) return;
            stuExamApplyList = stuExamApplyDAL.GetLisByStuID(stuIDs);
            if (stuExamApplyList == null)
            {
                log.Info("审批通过的科目列表为空");
                return;
            }
            //只取报考申请审批通过的科目
            var list1 = (from t1 in studyRecordList
                         join t2 in studyStatFlowSubjectList
                             on t1.SubjectID equals t2.NextSubjectID
                         join t3 in stuExamApplyList
                             on t1.StuID equals t3.StuID
                         select new
                         {
                             t1.StuID,
                             t1.SubjectID
                         }).Distinct();

            var list2 = (from t1 in studyRecordList
                         join t2 in studyStatFlowSubjectList
                             on t1.SubjectID equals t2.NextSubjectID
                         where t1.SubjectID == 1
                         select new
                         {
                             t1.StuID,
                             t1.SubjectID
                         }).Distinct();
            list1 = list1.Union(list2);
            if (!list1.Any())
            {
                log.Info("找不到报考申请审批通过的科目");
                return;
            }
            //设置科目未审批
            (from t1 in studyRecordList
             join t2 in list1
               on new { StuID = t1.StuID, SubjectID = t1.SubjectID } equals new { StuID = t2.StuID, SubjectID = t2.SubjectID } into temp
             from t in temp.DefaultIfEmpty()
             select new
             {
                 t1,
                 StuID = (t == null ? -1 : t.StuID)
             }).Where(o => o.t1.ValidFlag == 1 && o.StuID == -1)
                       .Update(item =>
                       {
                           item.t1.ValidFlag = (int)InvalidType.SubjectsNotApproval;
                       });

            //学习无效记录列表添加未审批记录
            AddRangeToInvalidList(InvalidType.SubjectsNotApproval);
        }

        /// <summary>
        /// 描  述：异常数据
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月16日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void SetAbnormalData()
        {
            //审核无效的异常数据
            studyRecordList.Where(o => o.ValidFlag == 1 && (o.CalcFlag_Add & 4) == 4).Update(item =>
            {
                item.ValidFlag = (int)InvalidType.AbnormalData;
            });
            AddRangeToInvalidList(InvalidType.AbnormalData, "审核无效的异常数据");

            //结束时间大于系统时间的异常数据
            studyRecordList.Where(o => o.ValidFlag == 1 && o.SR_EndTime > DateTime.Now).Update(item =>
            {
                item.ValidFlag = (int)InvalidType.EndTimeLargerSystemTime;
            });
            AddRangeToInvalidList(InvalidType.EndTimeLargerSystemTime, "异常数据");
        }

        /// <summary>
        /// 描  述：不在规定时间段范围内签到、签退的记录都设为无效
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月16日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void SetNotInSpecifiedTime()
        {
            //不在规定时间段范围内签到、签退的记录都设为无效
            studyRecordList.Where(o => o.ValidFlag == 1 &&
                !(subjectParamList.Where(p => p.SubjectID == o.SubjectID && TimeSpan.Parse(p.StartTime) <= o.SR_BeginTime.TimeOfDay
             && TimeSpan.Parse(p.EndTime) >= o.SR_EndTime.TimeOfDay).Any())).Update(item =>
             {
                 item.ValidFlag = (int)InvalidType.NotInSpecifiedTime;
             });

            //在学习无效记录列表中插入数据集
            AddRangeToInvalidListForTime(InvalidType.NotInSpecifiedTime);
        }

        /// <summary>
        /// 描  述：单次学时小于最小要求学时
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月16日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void SetLessThanMinimumHours()
        {
            var list = from t in subjectParamList
                       group t by new { t.SubjectID, t.StudyType } into g
                       select new
                       {
                           g.Key.SubjectID,
                           g.Key.StudyType,
                           MinTime = g.Min(p => p.MinTime)
                       };
            //单次学时小于最小要求学时
            (from t1 in studyRecordList
             join t2 in list
             on new { SubjectID = t1.SubjectID, StudyType = t1.StudyType } equals new { SubjectID = t2.SubjectID, StudyType = t2.StudyType }
             where t1.ValidFlag == 1 && t1.SR_RealTime < t2.MinTime
             select t1).Update(item =>
                            {
                                item.ValidFlag = (int)InvalidType.LessThanMinimumHours;
                            });

            //学习无效记录列表添加记录
            AddRangeToInvalidListForTime(InvalidType.LessThanMinimumHours);
        }

        /// <summary>
        /// 描  述：不在准教车型范围内
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月17日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void SetNotInTeachingRange()
        {
            //只对实操学习检查准教车型
            string recordIDs = string.Join(",", studyRecordList.Where(o => o.ValidFlag == 1 && o.StudyType == 1).Select(o => o.StudyRecordID));
            if (string.IsNullOrWhiteSpace(recordIDs)) return;
            ArrayList arrayList = studyRecordResultDAL.GetRecordIDByTeacher(recordIDs);
            if (arrayList == null)
            {
                log.Info("不在准教车型范围内的数据为空");
                return;
            }

            studyRecordList.Where(o => arrayList.Contains(o.StudyRecordID)).Update(item =>
            {
                item.ValidFlag = (int)InvalidType.NotInTeachingRange;
            });

            //学习无效记录列表添加记录
            AddRangeToInvalidListForTime(InvalidType.NotInTeachingRange);
        }

        /// <summary>
        /// 描  述：不在准教科目范围内
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月17日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void SetNotInSubjectRange()
        {
            var list = studyRecordList.Where(o => o.ValidFlag == 1 && o.StudyType != 2 &&
               ((o.StudyType == 0 && o.TeacherType == 2)
               || (o.StudyType == 1 && o.TeacherType == 1)
               || (o.StudyType == 2 && o.TeacherType == 2)));
            list.Update(item =>
                   {
                       item.ValidFlag = (int)InvalidType.NotInSubjectRange;
                   });

            //学习无效记录列表添加记录
            AddRangeToInvalidListForTime(InvalidType.NotInSubjectRange);
        }

        /// <summary>
        /// 描  述：教练教车重叠时间段
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月17日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void SetTeacherOverlappingTime()
        {
            //教车重叠时间段
            repRecordTeacherList.Where(o => o.ValidFlag == 1 && o.HasStat != 1 && o.StudyType == 1).Update(item =>
            {
                var temp = repRecordTeacherList.Where(o => o.SR_BeginTime <= item.SR_BeginTime &&
                    o.StudyType == 1 && o.ValidFlag == 1 && o.T_Teacher == item.T_Teacher &&
                    (
                       (o.SR_BeginTime > item.SR_BeginTime && o.SR_BeginTime < item.SR_EndTime)
                    || (o.SR_EndTime > item.SR_BeginTime && o.SR_EndTime < item.SR_EndTime)
                    || (item.SR_BeginTime > o.SR_BeginTime && item.SR_BeginTime < o.SR_EndTime)
                    || (item.SR_EndTime > o.SR_BeginTime && item.SR_EndTime < o.SR_EndTime)
                    )
                    ).Select(o => o.StudyRecordID);
                if (temp.Count() > 0)
                {
                    item.ValidFlag = (int)InvalidType.TeacherOverlappingTime;
                }
            });
            //更新状态
            (from t1 in studyRecordList
             join t2 in repRecordTeacherList
             on t1.StudyRecordID equals t2.StudyRecordID
             where t1.ValidFlag == (int)InvalidType.Normal && t2.ValidFlag == (int)InvalidType.TeacherOverlappingTime
             select t1).Update(item =>
                           {
                               item.ValidFlag = (int)InvalidType.TeacherOverlappingTime;
                           });


            //学习无效记录列表添加记录
            AddRangeToInvalidListForTime(InvalidType.TeacherOverlappingTime);
        }

        /// <summary>
        /// 描  述：学员学车重叠时间段
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月17日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void SetStudentOverlappingTime()
        {
            //获取学员学车记录
            repRecordStudentList = repRecordTeacherList.Where(o => o.ValidFlag == (int)InvalidType.Normal)
                .OrderBy(o => o.StuID)
                .ThenBy(o => o.SR_BeginTime)
                .ThenBy(o => o.StudyRecordID)
                .Select(o => new RepRecordStudent
                {
                    StudyRecordID = o.StudyRecordID,
                    StuID = o.StuID,
                    SubjectID = o.SubjectID,
                    SR_ActualTime = o.SR_ActualTime,
                    SR_BeginTime = o.SR_BeginTime,
                    SR_EndTime = o.SR_EndTime,
                    ValidFlag = o.ValidFlag,
                    HasStat = o.HasStat
                }).ToList();
            //学车重叠时间段
            repRecordStudentList.Where(item => item.ValidFlag == 1 && item.HasStat != 1).Update(item =>
            {
                var temp = repRecordStudentList.Where(o => o.SR_BeginTime <= item.SR_BeginTime
                    && o.ValidFlag == 1 && o.StuID == item.StuID &&
                    (
                       (o.SR_BeginTime > item.SR_BeginTime && o.SR_BeginTime < item.SR_EndTime)
                    || (o.SR_EndTime > item.SR_BeginTime && o.SR_EndTime < item.SR_EndTime)
                    || (item.SR_BeginTime > o.SR_BeginTime && item.SR_BeginTime < o.SR_EndTime)
                    || (item.SR_EndTime > o.SR_BeginTime && item.SR_EndTime < o.SR_EndTime)
                    )
                    ).Select(o => o.StudyRecordID);
                if (temp.Count() > 0)
                {
                    item.ValidFlag = (int)InvalidType.StudentOverlappingTime;
                }
            });
            //更新状态
            (from t1 in studyRecordList
             join t2 in repRecordStudentList
             on t1.StudyRecordID equals t2.StudyRecordID
             where t1.ValidFlag == (int)InvalidType.Normal && t2.ValidFlag == (int)InvalidType.StudentOverlappingTime
             select t1).Update(item =>
                          {
                              item.ValidFlag = (int)InvalidType.StudentOverlappingTime;
                          });

            //学习无效记录列表添加记录
            AddRangeToInvalidListForTime(InvalidType.StudentOverlappingTime);
        }

        /// <summary>
        /// 描  述：不在有效速度范围内
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月17日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void SetNotInSpeedRange()
        {
            //只取有效速度的学习记录,只判断科目三
            studyRecordList.Where(o => o.ValidFlag == 1 && o.StudyType == 1 && o.SR_StudyType == 32 &&
                        (o.ValidSpeed == 0 || o.ValidSpeed == null)).Update(item =>
                {
                    item.ValidFlag = (int)InvalidType.NotInSpeedRange;
                });
            AddRangeToInvalidListForTime(InvalidType.NotInSpeedRange);
        }

        /// <summary>
        /// 描  述：非法车辆状态:ACC及发动机状态检查
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月17日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void SetIllegalVehicleState()
        {
            //TODO:不需要把SQL的代码转换，这块只需要从w_StudyRecord_Result_Addition表的HasCalcVehStatus字段判断车辆状态，由其它模块提供

            #region 暂时作废代码
            //studyRecordList.ForEach(item =>
            //    {
            //        if (item.ValidFlag == 1 && item.StudyType == 1 && item.HasCalcVehStatus == 1 && item.ValidVehStatus == 0)
            //        {
            //            item.ValidFlag = (int)InvalidType.IllegalVehicleState;
            //        }
            //    });
            ////车辆状态
            //vehicleStatusParamList = vehicleStatusParamDAL.GetList();
            //var checkList = vehicleStatusParamList.Where(o => (o.TypeName == "ACC" || o.TypeName == "ENG") && o.IsEnabled == 1)
            //    .Select(o => new { IsEnabled = o.IsEnabled, Duration = o.Duration });
            ////数据源 TrackDB:老版本GPS系统数据库名称
            //dataSourceList = dataSourceDAL.GetList();
            //var db = dataSourceList.Where(o => o.DataSourceCode == "TrackDB").Select(o => o);
            ////ACC或发动机关，或存在GPS数据
            //if (checkList.Count() > 0 && db.Count() > 0)
            //{
            //    trackCommNoList = studyRecordList.Where(o => o.StudyType == 1 && o.HasCalcVehStatus == 0)
            //                                   .GroupBy(o => o.DeviceNo)
            //                                   .Select(g => new TrackCommNo
            //                                   {
            //                                       DeviceNo = g.Key,
            //                                       SR_BeginTime = g.Min(o => o.SR_BeginTime),
            //                                       SR_EndTime = g.Max(o => o.SR_EndTime)
            //                                   }).ToList();
            //    //Gps数据
            //    studyRecordGpsDataList = studyRecordList.Select(o => new StudyRecordGpsData
            //    {
            //        StudyRecordID = o.StudyRecordID,
            //        SR_BeginTime = o.SR_BeginTime,
            //        SR_EndTime = o.SR_EndTime,
            //        DeviceNo = o.DeviceNo
            //    }).ToList();
            //}
            #endregion
        }
        #endregion

        #region 写数据
        /// <summary>
        /// 描  述：更新学习记录表
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月18日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void UpdateStudyRecord()
        {
            studyRecordList.RemoveAll(o => o.ValidFlag == 1);

            DbConnection _dbConnection = null;              //定义数据库连接，为事务使用
            DbTransaction transaction = null;               //定义事务
            int rtnValue = 0;
            try
            {
                if (null != studyRecordList)
                {
                    //启用事务
                    _dbConnection = DBO.GetInstance().CreateConnection();
                    _dbConnection.Open();
                    transaction = _dbConnection.BeginTransaction();
                    log.Info("++++++开始批量更新数据++++++");
                    //更新数据
                    studyRecordList.ForEach(item =>
                    {
                        //更新学习记录表
                        rtnValue = studyRecordResultDAL.UpdateCheckedState(item, transaction, DBO.GetInstance());
                        //更新学习记录附加表
                        rtnValue = studyRecordResultAdditionDAL.UpdateCheckedState(item, transaction, DBO.GetInstance());
                    });
                    //写入到无效的学习记录表
                    studyRecordInvalidList.ForEach(item =>
                    {
                        rtnValue = studyRecordInvalidDAL.Add(item, transaction, DBO.GetInstance());
                    });

                    //结束事务
                    transaction.Commit();
                    log.Info("++++++结束批量更新数据++++++");
                }
                else
                {
                    log.Info("不存在无效的学习记录");
                    return;
                }
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    //回滚事务
                    transaction.Rollback();
                }
                throw ex;
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

        #endregion

        #region 内部方法
        /// <summary>
        /// 描  述：获取教练教车数据
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月17日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void GetTeacherRecord()
        {
            string teachers = string.Join(",", studyRecordList.Select(o => string.Format("'{0}'", o.T_Teacher)).Distinct());
            repRecordTeacherList = repRecordTeacherDAL.GetLisByTeachers(teachers);
        }

        /// <summary>
        /// 描  述：只对实操学习检查教练教车重叠时间段
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月17日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void UpdateTeacherRecord()
        {

            (from t1 in repRecordTeacherList
             join t2 in studyRecordList
             on t1.StudyRecordID equals t2.StudyRecordID
             where t2.StudyType == 1 && (t2.ValidFlag == (int)InvalidType.NotInTeachingRange || t2.ValidFlag == (int)InvalidType.NotInSubjectRange)
             select new
             {
                 t1,
                 t2.ValidFlag
             }).Update(item =>
                           {
                               if (item.ValidFlag == (int)InvalidType.NotInTeachingRange)
                               {
                                   item.t1.ValidFlag = (int)InvalidType.NotInTeachingRange;
                               }
                               if (item.ValidFlag == (int)InvalidType.NotInSubjectRange)
                               {
                                   item.t1.ValidFlag = (int)InvalidType.NotInSubjectRange;
                               }
                           });

        }
        #endregion

        #region 公共方法

        /// <summary>
        /// 描  述：在学习无效记录列表中插入数据集
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月16日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="type">无效类型</param>
        /// <param name="remark"></param>
        private void AddRangeToInvalidList(InvalidType type, string remark = "")
        {
            var invalidList = studyRecordList.Where(o => o.ValidFlag == (int)type)
            .Select(o => new StudyRecordInvalid
            {
                StudyRecordID = o.StudyRecordID,
                InvalidActualTime = o.SR_ActualTime,
                TypeName = RemarkAttribute.GetEnumRemark(type),
                Remark = remark
            });
            studyRecordInvalidList.AddRange(invalidList);
        }

        /// <summary>
        /// 
        /// 描  述：在学习无效记录列表中插入数据集(无效学习时间段）
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月16日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="type"></param>
        private void AddRangeToInvalidListForTime(InvalidType type)
        {
            var invalidList = studyRecordList.Where(o => o.ValidFlag == (int)type)
            .Select(o => new StudyRecordInvalid
            {
                StudyRecordID = o.StudyRecordID,
                InvalidActualTime = o.SR_ActualTime,
                TypeName = RemarkAttribute.GetEnumRemark(type),
                Remark = string.Format("无效的学习时间段：{0}-{1}", o.SR_BeginTime.TimeOfDay.ToString(), o.SR_EndTime.TimeOfDay.ToString())
            });
            studyRecordInvalidList.AddRange(invalidList);
        }

        #endregion
    }
}
