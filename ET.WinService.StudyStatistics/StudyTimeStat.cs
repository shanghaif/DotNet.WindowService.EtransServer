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
    /// 对学习时间进行统计
    /// </summary>
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：
    /// 版本号：1.0.0.1
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 日  期：2013年1月21日
    /// 修  改：
    /// 原  因：
    /// ------------------------------------------------------------------------------
    /// </remarks>
    /// <example>
    /// [示例代码在这里写入]
    /// </example>
    public class StudyTimeStat
    {
        // private Logger log = new Logger("StudyStat");
        private ILog log = LogManager.GetLogger(typeof(StudyTimeStat));
        #region 数据操作类
        //数据操作类
        private SubjectParamDAL subjectParamDAL;
        private StudyRecordResultDAL studyRecordResultDAL;
        private StudyStatStudentDAL studyStatStudentDAL;
        private StudyStatByDayDAL studyStatByDayDAL;
        private StudyDrivingSubjectDAL studyDrivingSubjectDAL;
        private VehMapDAL vehMapDAL;
        private DataSourceDAL dataSourceDal;
        private ANADrivingDAL aNADrivingDAL;
        private StudyRecordResultAdditionDAL studyRecordResultAdditionDAL;
        private StudyRecordInvalidDAL studyRecordInvalidDAL;
        #endregion

        #region 数据集
        //数据集
        List<SubjectParam> subjectParamList;                    //科目学时参数列表
        List<StudyRecord> studyRecordList;                      //学时列表
        List<StudyRecord> studyRecordOtherList;                 //学时列表2
        List<StudyStatStudent> studyStatStudentList;            //学员学习汇总列表
        List<StudyStatByDay> studyStatList;                     //学习统计
        List<StudyRecordInvalid> studyRecordInvalidList;        //学习无效记录列表
        List<VehMap> vehMapList;                                //车辆对照列表
        List<ANADriving> aNADrivingValidList;                   //道路及区域进出分析记录
        List<ANADriving> aNADrivingValidOtherList;              //道路及区域进出分析记录
        List<ANADriving> aNADrivingInvalidList;                 //道路及区域停留超时分析记录
        List<ANADriving> aNADrivingInvalidOtherList;            //道路及区域停留超时分析记录
        List<ANADriving> aNADrivingInvalidMoreList;            //道路及区域停留超时分析记录
        #endregion


        /// <summary>
        /// 构造函数
        /// </summary>
        public StudyTimeStat()
        {
            subjectParamDAL = new SubjectParamDAL();
            studyRecordResultDAL = new StudyRecordResultDAL();
            studyStatStudentDAL = new StudyStatStudentDAL();
            studyStatByDayDAL = new StudyStatByDayDAL();
            studyDrivingSubjectDAL = new StudyDrivingSubjectDAL();
            vehMapDAL = new VehMapDAL();
            dataSourceDal = new DataSourceDAL();
            aNADrivingDAL = new ANADrivingDAL();
            studyRecordResultAdditionDAL = new StudyRecordResultAdditionDAL();
            studyRecordInvalidDAL = new StudyRecordInvalidDAL();

            //数据集
            aNADrivingValidList = new List<ANADriving>();
            aNADrivingValidOtherList = new List<ANADriving>();
            aNADrivingInvalidList = new List<ANADriving>();
            aNADrivingInvalidOtherList = new List<ANADriving>();
            aNADrivingInvalidMoreList = new List<ANADriving>();
            studyRecordInvalidList = new List<StudyRecordInvalid>();
        }

        /// <summary>
        /// 描  述：公开方法
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月23日
        /// 修  改：
        /// 原  因：
        /// </summary>
        public void Stat()
        {
            try
            {
                log.Info("******开始对学习时间进行统计******");
                //获取科目学时参数配置表
                log.Debug("########获取科目学时参数配置表########");
                subjectParamList = subjectParamDAL.GetList();
                //获取还没有统计的记录列表
                log.Debug("########获取还没有统计的记录列表########");
                studyRecordList = studyRecordResultDAL.GetNotStatList();
                //没有待统计的记录即退出
                if (studyRecordList == null)
                {
                    log.Info("没有待统计的记录");
                    throw new NullException();
                }

                //科目一的StudyType=0,前台程序没有填写(BUG)
                studyRecordList.ForEach(item =>
                {
                    if (item.SubjectID == 1)
                    {
                        item.StudyType = 0;
                    }
                });
                //获取所有stuID，以逗号隔开
                string stuIDs = string.Join(",", studyRecordList.Select(o => o.StuID).Distinct());

                //构造学员学习汇总表
                log.Debug("########构造学员学习汇总表########");
                SetStudyStatStudent(stuIDs);
                //车辆行驶路线及行驶区域
                log.Debug("########车辆行驶路线及行驶区域########");
                SetVehicleRouteAndRegional();
                //已经存在的日统计记录
                log.Debug("########已经存在的日统计记录########");
                SetStudyStat(stuIDs);
                //
                log.Debug("########设置学时记录临时表########");
                SetStudyRecordOtherList();
                log.Info("******结束对学习时间进行统计******");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 描  述：构造学员学习汇总表
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月21日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void SetStudyStatStudent(string stuIDs)
        {
            List<StudyStatStudent> tempList = studyStatStudentDAL.GetLisByStuIDs(stuIDs);
            if (tempList == null)
            {
                log.Info("学员学习汇总表为空");
                return;
            }
            var list1 = studyRecordList.Select(o => new { o.StuID, o.SubjectID, o.StudyType }).Distinct();
            studyStatStudentList = (from t1 in list1
                                    join t2 in tempList
                                    on new { StuID = t1.StuID, SubjectID = t1.SubjectID, StudyType = t1.StudyType } equals new { StuID = t2.StuID, SubjectID = t2.SubjectID, StudyType = t2.StudyType } into temp
                                    from t in temp.DefaultIfEmpty()
                                    select new StudyStatStudent
                                    {
                                        ID = t == null ? 0 : t.ID,
                                        StuID = t1.StuID,
                                        SubjectID = t1.SubjectID,
                                        StudyType = t1.StudyType,
                                        SR_RealTime = t == null ? 0 : t.SR_RealTime,
                                        SR_ActualTime = t == null ? 0 : t.SR_ActualTime,
                                        ValidActualTime = t == null ? 0 : t.ValidActualTime,
                                        InvalidActualTime = t == null ? 0 : t.InvalidActualTime,
                                        ValidStudyTime_Exam = t == null ? 0 : t.ValidStudyTime_Exam
                                    }).ToList();

        }

        /// <summary>
        /// 描  述：车辆行驶路线及行驶区域
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月24日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void SetVehicleRouteAndRegional()
        {
            var deviceNoList = studyRecordList.Where(o => o.ValidFlag == 1).Select(o => string.Format("'{0}'", o.DeviceNo)).Distinct();
            string deviceNos = string.Join(",", deviceNoList);
            string gpsDBName = dataSourceDal.GetDbNameByCode("GPSDB"); //GPS系统数据库名称
            if (!string.IsNullOrWhiteSpace(gpsDBName))
            {
                vehMapList = vehMapDAL.GetLisByDeviceNos(gpsDBName, deviceNos);
            }
            else
            {
                return;
            }

            if (vehMapList == null)
            {
                log.Info("车辆行驶路线及行驶区域数据为空");
                return;
            }

            //数据存在
            if (vehMapList.Any())
            {
                //区域进出分析记录
                CheckVehicleInAreaDaliy(gpsDBName);

                //不在设定线路或区域内的无效时间
                SetANADrivingValidList();
            }

        }

        /// <summary>
        /// 描  述：已经存在的日统计记录
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月21日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="stuIDs"></param>
        private void SetStudyStat(string stuIDs)
        {
            List<StudyStatByDay> tempList = studyStatByDayDAL.GetLisByStuIDs(stuIDs);
            if (tempList == null)
            {
                log.Info("有学员ID获取的数据列表为空");
                return;
            }
            var list1 = studyRecordList.Select(o => new { o.StuID, o.SubjectID, o.StudyType, StatDate = o.SR_BeginTime.Date }).Distinct();
            studyStatList = (from t1 in tempList
                             join t2 in list1
                             on new { StuID = t1.StuID, SubjectID = t1.SubjectID, StudyType = t1.StudyType, StatDate = t1.StatDate } equals new { StuID = t2.StuID, SubjectID = t2.SubjectID, StudyType = t2.StudyType, StatDate = t2.StatDate }
                             select t1).ToList();
        }

        private void SetStudyRecordOtherList()
        {
            try
            {
                #region 获取数据
                studyRecordOtherList = studyRecordList.Where(o => o.ValidFlag == 1 && (o.CalcFlag_Add & 4) != 4)
                     .OrderBy(o => o.StuID)
                     .ThenBy(o => o.SubjectID)
                     .ThenBy(o => o.StudyType)
                     .ThenBy(o => o.SR_BeginTime)
                     .Select(o => new StudyRecord
                     {
                         StudyRecordID = o.StudyRecordID,
                         StuID = o.StuID,
                         SubjectID = o.SubjectID,
                         SR_RealTime = o.SR_RealTime,
                         SR_ActualTime = o.SR_ActualTime,
                         SR_BeginTime = o.SR_BeginTime,
                         SR_EndTime = o.SR_EndTime,
                         DeviceNo = o.DeviceNo,
                         CalcFlag_Add = o.CalcFlag_Add,
                         CalcFlag = 1,
                         StudyType = (o.StudyType ?? 1)
                     }).ToList();

                if (!studyRecordOtherList.Any()) return;
                #endregion

                var temp = studyRecordOtherList.First();
                TempEntity tempEntity = new TempEntity();
                tempEntity.PriorStatDate = temp.SR_BeginTime.Date;
                tempEntity.PirorStuID = temp.StuID;
                tempEntity.PirorSubjectID = temp.SubjectID;
                tempEntity.PirorStudyType = temp.StudyType ?? 1;
                //初始参数
               tempEntity= SetLocalField(tempEntity);

                #region 循环列表
                studyRecordOtherList.ForEach(item =>
                {
                    tempEntity.RecordValidActualTime = 0;     //当前记录的有效学习时间(分钟)

                    //科目学时每天限制参数
                    subjectParamList.Where(o => o.SubjectID == item.SubjectID
                                          && o.StudyType == item.StudyType
                                          && TimeSpan.Parse(o.StartTime) <= item.SR_BeginTime.TimeOfDay
                                          && TimeSpan.Parse(o.EndTime) >= item.SR_EndTime.TimeOfDay)
                                          .Update(p =>
                                          {
                                              tempEntity.MinTime = p.MinTime;
                                              tempEntity.ValidStudyTime = p.MaxTime;
                                              tempEntity.ValidStartTime = p.StartTime;
                                              tempEntity.ValidEndTime = p.EndTime;
                                          });

                    if (tempEntity.PriorStatDate != item.SR_BeginTime.Date || tempEntity.PirorStuID != item.StuID ||
                        tempEntity.PirorSubjectID != item.SubjectID || tempEntity.PirorStudyType != item.StudyType)
                    {
                        //写入到日统计表
                        SetStudyStatByDay(tempEntity);

                        #region 重设字段值
                        tempEntity.PriorStatDate = item.SR_BeginTime.Date;
                        tempEntity.PirorStuID = item.StuID;
                        tempEntity.PirorSubjectID = item.SubjectID;
                        tempEntity.PirorStudyType = item.StudyType;
                        tempEntity.TotalRealTime = 0;
                        tempEntity.TotalActualTime = 0;
                        tempEntity.TotalValidActualTime = 0;
                        tempEntity.TotalInvalidActualTime = 0;
                        tempEntity.ValidStudyTime_Exam = 0;
                        tempEntity.SR_RealTime_Operate = 0;
                        tempEntity.SR_ActualTime_Operate = 0;
                        tempEntity.ValidActualTime_Operate = 0;
                        tempEntity.InvalidActualTime_Operate = 0;
                        tempEntity.SR_RealTime_Simulate = 0;
                        tempEntity.SR_ActualTime_Simulate = 0;
                       // tempEntity.ValidActualTime_Simulate = 0;
                        tempEntity.InvalidActualTime_Operate_Sub = 0;

                        #endregion

                        //初始参数
                        tempEntity = SetLocalField(tempEntity);

                    }

                    studyStatStudentList.Where(o => o.StuID == item.StuID && o.SubjectID == item.SubjectID && o.StudyType == item.StudyType)
                        .Update(p =>
                            {
                                tempEntity.StuStatValidActualTime = p.ValidActualTime;
                                tempEntity.StuStatInvalidActualTime = p.InvalidActualTime;
                                tempEntity.StuStatValidStudyTime_Exam = p.ValidStudyTime_Exam;
                            });

                    //无效的学习时间
                    tempEntity.InValidFlag = 0;
                    tempEntity.vdt_s = Convert.ToDateTime(item.SR_BeginTime.Date.ToShortDateString() + " " + tempEntity.ValidStartTime);
                    tempEntity.vdt_e = Convert.ToDateTime(item.SR_BeginTime.Date.ToShortDateString() + " " + tempEntity.ValidEndTime);

                    //有效时间段之前
                    BeforeValidTime(item, ref tempEntity);

                    //有效时间段之内
                    WithinValidTime(item, ref tempEntity);

                    //有效时间段之后
                    AfterValidTime(item, ref tempEntity);


                    tempEntity.TotalRealTime = tempEntity.TotalRealTime + item.SR_RealTime;
                    tempEntity.TotalActualTime = tempEntity.TotalActualTime + item.SR_ActualTime;
                    tempEntity.RecordInvalidActualTime = item.SR_ActualTime - tempEntity.RecordValidActualTime;
                    
                    //回写入到有效学习时间及系数到学习记录表
                    studyRecordList.Where(o => o.StudyRecordID == item.StudyRecordID).Update(p =>
                        {
                            p.StudyRate = tempEntity.StudyRate;
                            p.ValidActualTime = p.ValidActualTime + tempEntity.RecordValidActualTime;
                        });
                    //回写入到学员学习汇总表
                    studyStatStudentList.Where(o => o.StuID == item.StuID && o.SubjectID == item.SubjectID && o.StudyType == item.StudyType)
                        .Update(p =>
                            {
                                p.SR_RealTime = p.SR_RealTime + item.SR_RealTime;
                                p.SR_ActualTime = p.SR_ActualTime + item.SR_ActualTime;
                                p.ValidActualTime = p.ValidActualTime + tempEntity.RecordValidActualTime;
                                p.ValidStudyTime_Exam = p.ValidStudyTime_Exam + Math.Round((double)((tempEntity.RecordValidActualTime ?? 0) * tempEntity.StudyRate));
                                p.InvalidActualTime = p.InvalidActualTime + tempEntity.RecordInvalidActualTime;
                            });
                });
                #endregion

                //写入到日统计表
                updateStudyStatByDay(tempEntity);

                //更新数据库数据
                UpdateDBData();

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        #region 写数据
        /// <summary>
        /// 描  述：更新数据库数据 
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月18日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void UpdateDBData()
        {
            //studyRecordList.RemoveAll(o => o.ValidFlag == 1);

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
                        if (item.ValidFlag != 1)
                        {
                            rtnValue = studyRecordResultAdditionDAL.UpdateStatState(item, transaction, DBO.GetInstance());
                        }

                    });
                    //写入到无效的学习记录表
                    studyRecordInvalidList.ForEach(item =>
                    {
                        rtnValue = studyRecordInvalidDAL.Add(item, transaction, DBO.GetInstance());
                    });

                    //写入到日统计表
                    studyStatList.ForEach(item =>
                    {
                        rtnValue = studyStatByDayDAL.UpdateStatState(item, transaction, DBO.GetInstance());
                    });

                    //写入到学员学习汇总表
                    studyStatStudentList.ForEach(item =>
                    {
                        if (item.ID > 0)
                        {
                            rtnValue = studyStatStudentDAL.UpdateStatState(item, transaction, DBO.GetInstance());
                        }
                        else
                        {
                            rtnValue = studyStatStudentDAL.Add(item, transaction, DBO.GetInstance());
                        }
                    });

                    //结束事务
                    transaction.Commit();

                    log.Info("++++++结束批量更新数据++++++");
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
        /// 描  述：区域进出分析记录
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月24日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="gpsDBName"></param>
        private void CheckVehicleInAreaDaliy(string gpsDBName)
        {
            var list = from t in studyRecordList
                       group t by t.DeviceNo into g
                       select new
                       {
                           DeviceNo = g.Key,
                           StartStudyTime = g.Min(p => p.SR_BeginTime),
                           EndStudyTime = g.Max(p => p.SR_EndTime)
                       };
            var list2 = from t1 in vehMapList
                        join t2 in list
                        on t1.DeviceNo equals t2.DeviceNo
                        select t1.DeviceNo;
            vehMapList.Where(o => list2.Contains(o.DeviceNo))
                .Update(item =>
                {
                    var temp = list.Where(p => p.DeviceNo == item.DeviceNo).First();
                    item.StartStudyTime = temp.EndStudyTime.AddDays(-1);
                    item.EndStudyTime = temp.EndStudyTime;
                });
            string vehicleIDs = string.Join(",", vehMapList.Select(o => o.MapVehID));
            List<ANADrivingCheckVehicleInAreaDaliy> aNACheckList = aNADrivingDAL.GetLisByVehicleIDs(gpsDBName, vehicleIDs);
            var list3 = from t1 in vehMapList
                        join t2 in aNACheckList
                        on t1.MapVehID equals t2.VehicleID
                        where (t2.InsideBeginCheckTime > t1.StartStudyTime && t2.InsideBeginCheckTime < t1.EndStudyTime)
                        || (t2.InsideEndCheckTime > t1.StartStudyTime && t2.InsideEndCheckTime < t1.EndStudyTime)
                        || (t1.StartStudyTime > t2.InsideBeginCheckTime && t1.StartStudyTime < t2.InsideEndCheckTime)
                        || (t1.EndStudyTime > t2.InsideBeginCheckTime && t1.EndStudyTime < t2.InsideEndCheckTime)
                        select new
                        {
                            DeviceNo = t1.DeviceNo,
                            InsideBeginCheckTime = t2.InsideBeginCheckTime,
                            InsideEndCheckTime = t2.InsideEndCheckTime,
                            DataType = 3  //区域进出分析记录(每天)
                        };
            foreach (var item in list3)
            {
                aNADrivingValidList.Add(new ANADriving
                {
                    DeviceNo = item.DeviceNo,
                    StartTime = item.InsideBeginCheckTime,
                    EndTime = item.InsideEndCheckTime,
                    DataType = item.DataType
                });
            }
            DateTime dt = DateTime.Now.AddDays(1).Date;
            aNADrivingValidList.Where(o => (o.DataType == 1 || o.DataType == 3) && o.EndTime == null)
                .Update(item =>
                {
                    item.EndTime = dt;
                });

        }

        /// <summary>
        /// 描  述：不在设定线路或区域内的无效时间
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月24日
        /// 修  改：
        /// 原  因：
        /// </summary>
        private void SetANADrivingValidList()
        {
            vehMapList.ForEach(item =>
            {
                aNADrivingValidOtherList.Clear();
                aNADrivingValidOtherList.AddRange(aNADrivingValidList.Where(o => o.DeviceNo == item.DeviceNo).OrderBy(o => o.StartTime));
                if (aNADrivingValidOtherList.Count() <= 0)
                {
                    aNADrivingInvalidList.Add(new ANADriving()
                    {
                        DeviceNo = item.DeviceNo,
                        StartTime = item.StartStudyTime,
                        EndTime = item.EndStudyTime,
                        DataType = 3
                    });
                }
                else
                {
                    var firstItem = aNADrivingValidOtherList.First();
                    //不到一分钟不算
                    if (item.StartStudyTime.DateDiff(DateInterval.Second,firstItem.StartTime) > 61)
                    {
                        aNADrivingInvalidList.Add(new ANADriving()
                        {
                            DeviceNo = item.DeviceNo,
                            StartTime = item.StartStudyTime,
                            EndTime = firstItem.StartTime.AddSeconds(-1),
                            DataType = firstItem.DataType
                        });
                    }

                    aNADrivingValidOtherList.Skip(1).ToList().ForEach(obj =>
                    {
                        if (firstItem.EndTime >= obj.EndTime)
                        {

                        }
                        else
                        {
                            //不到一分钟不算
                            if (firstItem.EndTime.DateDiff(DateInterval.Second,obj.StartTime) > 61)
                            {
                                aNADrivingInvalidList.Add(new ANADriving()
                                {
                                    DeviceNo = item.DeviceNo,
                                    StartTime = firstItem.EndTime.AddSeconds(1),
                                    EndTime = obj.StartTime.AddSeconds(-1),
                                    DataType = firstItem.DataType
                                });
                            }
                            firstItem.StartTime = obj.StartTime;
                            firstItem.EndTime = obj.EndTime;
                            firstItem.DataType = obj.DataType;
                        }
                    });

                    //不到一分钟不算
                    if (firstItem.EndTime.DateDiff(DateInterval.Second,item.EndStudyTime) > 61)
                    {
                        aNADrivingInvalidList.Add(new ANADriving()
                        {
                            DeviceNo = item.DeviceNo,
                            StartTime = firstItem.EndTime.AddSeconds(1),
                            EndTime = item.EndStudyTime,
                            DataType = firstItem.DataType
                        });
                    }
                }

                aNADrivingInvalidOtherList.Clear();
                aNADrivingInvalidOtherList.AddRange(aNADrivingInvalidList.Where(o => o.DeviceNo == item.DeviceNo).OrderBy(o => o.StartTime));
                if (aNADrivingInvalidOtherList.Count > 0)
                {
                    var fItem = aNADrivingInvalidOtherList.First();
                    aNADrivingInvalidOtherList.Skip(1).ToList().ForEach(obj =>
                    {
                        if (fItem.StartTime > obj.EndTime)
                        {
                        }
                        else
                        {
                            //不到一分钟不算
                            if (fItem.StartTime.DateDiff(DateInterval.Second,fItem.EndTime) >= 60)
                            {
                                aNADrivingInvalidMoreList.Add(new ANADriving()
                                {
                                    DeviceNo = item.DeviceNo,
                                    StartTime = fItem.StartTime,
                                    EndTime = fItem.EndTime,
                                    DataType = fItem.DataType
                                });
                                fItem.StartTime = obj.StartTime;
                                fItem.EndTime = obj.EndTime;
                                fItem.DataType = obj.DataType;
                            }
                        }
                    });
                    //最后一条
                    aNADrivingInvalidMoreList.Add(new ANADriving()
                    {
                        DeviceNo = item.DeviceNo,
                        StartTime = fItem.StartTime,
                        EndTime = fItem.EndTime,
                        DataType = fItem.DataType
                    });
                }
            });
        }

        /// <summary>
        /// 描  述：获取类型名称
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月25日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private string getTypeName(int dataType)
        {
            string typeName = string.Empty;
            switch (dataType)
            {
                case 1:
                    typeName = "偏离规定行驶路线";
                    break;
                case 2:
                    typeName = "路线超时停车";
                    break;
                case 3:
                    typeName = "偏离规定学车区域";
                    break;
                case 4:
                    typeName = "区域超时停车";
                    break;
            }
            return typeName;
        }

        /// <summary>
        /// 描  述：写入到日统计表
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月25日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="tempEntity"></param>
        private void updateStudyStatByDay(TempEntity tempEntity)
        {
            var sList = studyStatList.Where(o => o.StatDate == tempEntity.PriorStatDate && o.StuID == tempEntity.PirorStuID && o.SubjectID == tempEntity.PirorSubjectID && o.StudyType == tempEntity.PirorStudyType);
            if (sList.Any())
            {
                sList.Update(p =>
                {
                    p.SR_RealTime = tempEntity.TotalRealTime;
                    p.SR_ActualTime = tempEntity.TotalActualTime;
                    p.ValidActualTime = tempEntity.TotalValidActualTime;
                    p.InvalidActualTime = tempEntity.TotalInvalidActualTime;
                    p.ValidStudyTime_Exam = tempEntity.ValidStudyTime_Exam;
                });
            }
            else
            {
                studyStatList.Add(new StudyStatByDay()
                {
                    StuID = tempEntity.PirorStuID,
                    StatDate = tempEntity.PriorStatDate,
                    SubjectID = tempEntity.PirorSubjectID,
                    SR_RealTime = tempEntity.TotalRealTime,
                    SR_ActualTime = tempEntity.TotalActualTime,
                    ValidActualTime = tempEntity.TotalValidActualTime,
                    InvalidActualTime = tempEntity.TotalInvalidActualTime,
                    ValidStudyTime_Exam = tempEntity.ValidStudyTime_Exam,
                    StudyRate = tempEntity.StudyRate,
                    StudyType = tempEntity.PirorStudyType
                });
            }
        }
        #endregion

        #region 循环学习记录内部方法
        /// <summary>
        /// 描  述：设置本地变量
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月23日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <returns></returns>
        private TempEntity SetLocalField(TempEntity tEntity)
        {
            TempEntity tempEntity = tEntity;

            StudyDrivingSubject studyDrivingSubject = studyDrivingSubjectDAL.GetEntity(tempEntity.PirorStuID, tempEntity.PirorSubjectID);
            var tempStatList = studyStatList.Where(o => o.StatDate == tempEntity.PriorStatDate && o.StuID == tempEntity.PirorStuID && o.SubjectID == tempEntity.PirorSubjectID && o.StudyType == tempEntity.PirorStudyType);
            tempEntity.StudyRate_Theoretical = studyDrivingSubject.StudyRate_Theoretical;
            tempEntity.StudyTime_Theoretical = studyDrivingSubject.StudyTime_Theoretical;//理论学时参数
            tempEntity.StudyRate_Operate = studyDrivingSubject.StudyRate_Operate;
            tempEntity.StudyTime_Operate = studyDrivingSubject.StudyTime_Operate;//实操学时参数
            tempEntity.StudyRate_Simulate = studyDrivingSubject.StudyRate_Simulate;
            tempEntity.StudyTime_Simulate = studyDrivingSubject.StudyTime_Simulate;//模拟学时参数
            if (tempStatList.Count() > 0)
            {
                StudyStatByDay entity = tempStatList.First();
                tempEntity.TotalRealTime = entity.SR_RealTime;
                tempEntity.TotalActualTime = entity.SR_ActualTime;
                tempEntity.TotalValidActualTime = entity.ValidActualTime;
                tempEntity.TotalInvalidActualTime = entity.InvalidActualTime;
                tempEntity.StudyRate = entity.StudyRate;
                tempEntity.ValidStudyTime_Exam = entity.ValidStudyTime_Exam;
                tempEntity.SR_ActualTime_Operate = entity.SR_ActualTime;
                tempEntity.InvalidActualTime_Operate = entity.InvalidActualTime;
            }
            else
            {
                switch (tempEntity.PirorStudyType)
                {
                    case 0:
                        tempEntity.StudyRate = tempEntity.StudyRate_Theoretical;
                        break;
                    case 1:
                        tempEntity.StudyRate = tempEntity.StudyRate_Operate;
                        break;
                    case 2:
                        tempEntity.StudyRate = tempEntity.StudyRate_Simulate;
                        break;
                    default:
                        tempEntity.StudyRate = 1;
                        break;
                }
            }

            tempEntity.StudyRate = tempEntity.StudyRate ?? 1;
            tempEntity.StudyRate_Theoretical = tempEntity.StudyRate_Theoretical ?? 1;
            tempEntity.StudyTime_Theoretical = tempEntity.StudyTime_Theoretical ?? 88888888;
            tempEntity.StudyRate_Operate = tempEntity.StudyRate_Operate ?? 1;
            tempEntity.StudyTime_Operate = tempEntity.StudyTime_Operate ?? 88888888;
            tempEntity.StudyRate_Simulate = tempEntity.StudyRate_Simulate ?? 1;
            tempEntity.StudyTime_Simulate = tempEntity.StudyTime_Simulate ?? 8888888;

            //subjectParamList.Where(o => o.SubjectID == tempEntity.PirorSubjectID && o.StudyType == tempEntity.PirorStudyType
            //                        && TimeSpan.Parse(o.StartTime) <= temp.SR_BeginTime.TimeOfDay
            //                        && TimeSpan.Parse(o.EndTime) >= temp.SR_EndTime.TimeOfDay)
            //                        .Update(p =>
            //                            {
            //                                tempEntity.MinTime = p.MinTime;
            //                                tempEntity.ValidStudyTime = p.MaxTime;
            //                                tempEntity.ValidStartTime = p.StartTime;
            //                                tempEntity.ValidEndTime = p.EndTime;
            //                            });
            return tempEntity;
        }

        /// <summary>
        /// 描  述：写入到日统计表
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月23日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="tempEntity"></param>
        private void SetStudyStatByDay(TempEntity tempEntity)
        {
            //写入到日统计表
            var sList = studyStatList.Where(o => o.StatDate == tempEntity.PriorStatDate
                                             && o.StuID == tempEntity.PirorStuID
                                             && o.SubjectID == tempEntity.PirorSubjectID
                                             && o.StudyType == tempEntity.PirorStudyType);
            if (sList.Count() > 0)
            {
                sList.Update(p =>
                {
                    p.SR_RealTime = tempEntity.TotalRealTime;
                    p.SR_ActualTime = tempEntity.TotalActualTime;
                    p.ValidActualTime = tempEntity.TotalValidActualTime;
                    p.InvalidActualTime = tempEntity.TotalInvalidActualTime;
                    p.ValidStudyTime_Exam = tempEntity.ValidStudyTime_Exam;
                });
            }
            else
            {
                studyStatList.Add(new StudyStatByDay
                {
                    StuID = tempEntity.PirorStuID,
                    StatDate = tempEntity.PriorStatDate,
                    SubjectID = tempEntity.PirorSubjectID,
                    SR_RealTime = tempEntity.TotalRealTime,
                    SR_ActualTime = tempEntity.TotalActualTime,
                    ValidActualTime = tempEntity.TotalValidActualTime,
                    InvalidActualTime = tempEntity.TotalInvalidActualTime,
                    ValidStudyTime_Exam = tempEntity.ValidStudyTime_Exam,
                    StudyType = tempEntity.PirorStudyType
                });
            }
        }

        /// <summary>
        /// 描  述：有效时间段之前
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月24日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="item"></param>
        /// <param name="tempEntity"></param>
        private void BeforeValidTime(StudyRecord item, ref TempEntity tempEntity)
        {

            //不在规定的时间段内学习
            if (item.SR_BeginTime < tempEntity.vdt_s)
            {
                tempEntity.StartStudyTime = item.SR_BeginTime;
                tempEntity.TypeName = "不在规定的时间段内学习";
                tempEntity.InValidFlag = 1;
                if (item.SR_EndTime < tempEntity.vdt_s)
                {
                    tempEntity.EndStudyTime = item.SR_EndTime;
                    tempEntity.InvalidActualTime = item.SR_ActualTime;
                    tempEntity.TotalInvalidActualTime = tempEntity.TotalInvalidActualTime + tempEntity.InvalidActualTime;
                    tempEntity.Remark = string.Format("无效的学习时间段：{0}~{1}", tempEntity.StartStudyTime, tempEntity.EndStudyTime);
                    tempEntity.StartStudyTime = tempEntity.EndStudyTime.AddSeconds(1);
                }
                else
                {
                    tempEntity.EndStudyTime = tempEntity.vdt_s.AddSeconds(-1);
                    tempEntity.InvalidActualTime = Math.Round(tempEntity.StartStudyTime.DateDiff(DateInterval.Second,tempEntity.EndStudyTime) / 60.0);
                    tempEntity.TotalInvalidActualTime = tempEntity.TotalInvalidActualTime + tempEntity.InvalidActualTime;
                    tempEntity.Remark = string.Format("无效的学习时间段：{0}~{1}", tempEntity.StartStudyTime, tempEntity.EndStudyTime);
                    tempEntity.StartStudyTime = tempEntity.EndStudyTime.AddSeconds(1);
                }
                //添加到无效记录表
                studyRecordInvalidList.Add(new StudyRecordInvalid
                {
                    StudyRecordID = item.StudyRecordID,
                    InvalidActualTime = tempEntity.InvalidActualTime,
                    TypeName = tempEntity.TypeName,
                    Remark = tempEntity.Remark
                });
            }
            else
            {
                tempEntity.StartStudyTime = item.SR_BeginTime;
                tempEntity.EndStudyTime = tempEntity.vdt_e > item.SR_EndTime ? item.SR_EndTime : tempEntity.vdt_e;
            }
        }

        /// <summary>
        /// 描  述：有效时间段之内
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月25日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="item"></param>
        /// <param name="tempEntity"></param>
        private void WithinValidTime(StudyRecord item, ref TempEntity tempEntity)
        {
            if (tempEntity.StartStudyTime <= tempEntity.vdt_e && tempEntity.StartStudyTime <= item.SR_EndTime)
            {
                tempEntity.EndStudyTime = tempEntity.vdt_e > item.SR_EndTime ? item.SR_EndTime : tempEntity.vdt_e;
                //超出每天学习总时间
                if (tempEntity.TotalValidActualTime > tempEntity.ValidStudyTime)
                {
                    tempEntity.TypeName = "超出每天学习总时间";
                    tempEntity.InValidFlag = 1;
                    tempEntity.InvalidActualTime = Math.Round(tempEntity.StartStudyTime.DateDiff(DateInterval.Second,tempEntity.EndStudyTime) / 60.0);
                    tempEntity.TotalInvalidActualTime = tempEntity.TotalInvalidActualTime + tempEntity.InvalidActualTime;
                    tempEntity.Remark = string.Format("无效的学习时间段：{0}~{1}", tempEntity.StartStudyTime, tempEntity.EndStudyTime);
                    tempEntity.StartStudyTime = tempEntity.EndStudyTime.AddSeconds(1);

                    //添加到无效记录表
                    studyRecordInvalidList.Add(new StudyRecordInvalid
                    {
                        StudyRecordID = item.StudyRecordID,
                        InvalidActualTime = tempEntity.InvalidActualTime,
                        TypeName = tempEntity.TypeName,
                        Remark = tempEntity.Remark
                    });
                }
                else //超出每天学习总时间
                {
                    BeyondRegion(item, ref tempEntity);

                    #region 最后一小段学习时间
                    // 最后一小段学习时间
                    if (tempEntity.StartStudyTime <= tempEntity.EndStudyTime)
                    {
                        if (item.StudyType == 0 || item.StudyType == 1)
                        {
                            tempEntity.ValidActualTime = Math.Round(tempEntity.StartStudyTime.DateDiff(DateInterval.Second,tempEntity.EndStudyTime) / 60.0);
                            //超出每天学习总时间
                            if ((tempEntity.TotalValidActualTime + tempEntity.ValidActualTime) > tempEntity.ValidStudyTime)
                            {
                                tempEntity.InValidFlag = 1;
                                tempEntity.TypeName = "超出每天学习总时间";
                                tempEntity.ValidActualTime = tempEntity.ValidStudyTime - tempEntity.TotalValidActualTime;
                                tempEntity.StartStudyTime = tempEntity.StartStudyTime.AddMinutes((double)(tempEntity.ValidStudyTime - tempEntity.TotalValidActualTime));
                                tempEntity.InvalidActualTime = Math.Round(tempEntity.StartStudyTime.DateDiff(DateInterval.Second,tempEntity.EndStudyTime) / 60.0);
                                tempEntity.TotalInvalidActualTime = tempEntity.TotalInvalidActualTime + tempEntity.InvalidActualTime;
                                tempEntity.Remark = string.Format("无效的学习时间段：{0}~{1}", tempEntity.StartStudyTime, tempEntity.EndStudyTime);
                                tempEntity.StartStudyTime = tempEntity.EndStudyTime.AddSeconds(1);

                                //添加到无效记录表
                                studyRecordInvalidList.Add(new StudyRecordInvalid
                                {
                                    StudyRecordID = item.StudyRecordID,
                                    InvalidActualTime = tempEntity.InvalidActualTime,
                                    TypeName = tempEntity.TypeName,
                                    Remark = tempEntity.Remark
                                });
                            }
                            else //合法的学习时间
                            {
                                tempEntity.ValidActualTime = Math.Round(tempEntity.StartStudyTime.DateDiff(DateInterval.Second,tempEntity.EndStudyTime) / 60.0);
                                tempEntity.StartStudyTime = tempEntity.EndStudyTime.AddSeconds(1);
                            }

                            if (tempEntity.ValidActualTime < 0)
                            {
                                tempEntity.ValidActualTime = 0;
                            }
                            tempEntity.TotalValidActualTime = tempEntity.TotalValidActualTime + tempEntity.ValidActualTime;
                            tempEntity.ValidStudyTime_Exam = tempEntity.ValidStudyTime_Exam + (double)Math.Round((decimal)(tempEntity.ValidActualTime * tempEntity.StudyRate));

                            tempEntity.RecordValidActualTime = tempEntity.RecordValidActualTime + tempEntity.ValidActualTime;
                        }

                        if (item.StudyType == 2)
                        {
                            tempEntity.ValidActualTime = Math.Round(tempEntity.StartStudyTime.DateDiff(DateInterval.Second, tempEntity.EndStudyTime) / 60.0);
                            #region 超出模拟学习规定的总时间
                            if ((tempEntity.StuStatValidActualTime + tempEntity.ValidActualTime) > tempEntity.StudyTime_Simulate)
                            {
                                tempEntity.InValidFlag = 1;
                                tempEntity.TypeName = "超出规定的总学习时间";
                                tempEntity.ValidActualTime = tempEntity.StudyTime_Simulate - tempEntity.StuStatValidActualTime;
                                tempEntity.StartStudyTime = tempEntity.StartStudyTime.AddMinutes((double)(tempEntity.StudyTime_Simulate - tempEntity.StuStatValidActualTime));

                                tempEntity.InvalidActualTime = Math.Round(tempEntity.StartStudyTime.DateDiff(DateInterval.Second, tempEntity.EndStudyTime) / 60.0);
                                tempEntity.TotalInvalidActualTime = tempEntity.TotalInvalidActualTime + tempEntity.InvalidActualTime;
                                tempEntity.Remark = string.Format("无效的学习时间段：{0}~{1}", tempEntity.StartStudyTime, tempEntity.EndStudyTime);
                                tempEntity.StartStudyTime = tempEntity.EndStudyTime.AddSeconds(1);
                                //添加到无效记录表
                                studyRecordInvalidList.Add(new StudyRecordInvalid
                                {
                                    StudyRecordID = item.StudyRecordID,
                                    InvalidActualTime = tempEntity.InvalidActualTime,
                                    TypeName = tempEntity.TypeName,
                                    Remark = tempEntity.Remark
                                });
                            }
                            //超出每天学习总时间
                            else if ((tempEntity.TotalValidActualTime + tempEntity.ValidActualTime) > tempEntity.ValidStudyTime)
                            {
                                tempEntity.InValidFlag = 1;
                                tempEntity.TypeName = "超出每天学习总时间";
                                tempEntity.ValidActualTime = tempEntity.ValidStudyTime - tempEntity.TotalValidActualTime;
                                tempEntity.StartStudyTime = tempEntity.StartStudyTime.AddMinutes((double)(tempEntity.ValidStudyTime - tempEntity.TotalValidActualTime));

                                tempEntity.InvalidActualTime = Math.Round(tempEntity.StartStudyTime.DateDiff(DateInterval.Second,tempEntity.EndStudyTime) / 60.0);
                                tempEntity.TotalInvalidActualTime = tempEntity.TotalInvalidActualTime + tempEntity.InvalidActualTime;
                                tempEntity.Remark = string.Format("无效的学习时间段：{0}~{1}", tempEntity.StartStudyTime, tempEntity.EndStudyTime);
                                tempEntity.StartStudyTime = tempEntity.EndStudyTime.AddSeconds(1);
                                //添加到无效记录表
                                studyRecordInvalidList.Add(new StudyRecordInvalid
                                {
                                    StudyRecordID = item.StudyRecordID,
                                    InvalidActualTime = tempEntity.InvalidActualTime,
                                    TypeName = tempEntity.TypeName,
                                    Remark = tempEntity.Remark
                                });
                            }
                            else//合法的学习时间
                            {
                                // tempEntity.ValidActualTime_Simulate_Sub = Math.Round((tempEntity.EndStudyTime.Second - tempEntity.StartStudyTime.Second) / 60.0);
                                tempEntity.StartStudyTime = tempEntity.EndStudyTime.AddSeconds(1);
                            }
                            #endregion

                            if (tempEntity.ValidActualTime < 0)
                            {
                                tempEntity.ValidActualTime = 0;
                            }
                            tempEntity.TotalValidActualTime = tempEntity.TotalValidActualTime + tempEntity.ValidActualTime;
                            tempEntity.ValidStudyTime_Exam = tempEntity.ValidStudyTime_Exam + (double)Math.Round((decimal)(tempEntity.ValidActualTime * tempEntity.StudyRate));
                            tempEntity.RecordValidActualTime = tempEntity.RecordValidActualTime + tempEntity.ValidActualTime;
                        }
                    }
                    #endregion
                }
                tempEntity.StartStudyTime = tempEntity.EndStudyTime.AddSeconds(1);
                tempEntity.ValidActualTime = 0;
                tempEntity.InvalidActualTime = 0;
            }
        }

        /// <summary>
        /// 描  述：有效时间段之后
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月25日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="item"></param>
        /// <param name="tempEntity"></param>
        private void AfterValidTime(StudyRecord item, ref TempEntity tempEntity)
        {
            //不在规定的时间段内学习
            if (tempEntity.StartStudyTime > tempEntity.vdt_e && tempEntity.StartStudyTime <= item.SR_EndTime)
            {
                tempEntity.EndStudyTime = item.SR_EndTime;
                tempEntity.TypeName = "不在规定的时间段内学习";
                tempEntity.InValidFlag = 1;
                tempEntity.InvalidActualTime = Math.Round(tempEntity.StartStudyTime.DateDiff(DateInterval.Second, tempEntity.EndStudyTime) / 60.0);
                tempEntity.TotalInvalidActualTime = tempEntity.TotalInvalidActualTime + tempEntity.InvalidActualTime;
                tempEntity.Remark = string.Format("无效的学习时间段：{0}~{1}", tempEntity.StartStudyTime, tempEntity.EndStudyTime);
                tempEntity.StartStudyTime = tempEntity.EndStudyTime.AddSeconds(1);

                //添加到无效记录表
                studyRecordInvalidList.Add(new StudyRecordInvalid
                {
                    StudyRecordID = item.StudyRecordID,
                    InvalidActualTime = tempEntity.InvalidActualTime,
                    TypeName = tempEntity.TypeName,
                    Remark = tempEntity.Remark
                });
            }
        }

        /// <summary>
        /// 描  述：超出区域且没有特殊处理的记录要剔除掉无效的学习时间
        /// 作  者：黄冠群 (hgq@e-trans.com.cn)
        /// 时  间：2013年1月25日
        /// 修  改：
        /// 原  因：
        /// </summary>
        /// <param name="item"></param>
        /// <param name="temp"></param>
        private void BeyondRegion(StudyRecord item, ref TempEntity temp)
        {
            TempEntity tempEntity = temp;
            if ((item.CalcFlag_Add & 2) != 2                    //超出区域且没有特殊处理的记录要剔除掉无效的学习时间
                 && item.SubjectID == 2 && item.StudyType == 1)  //偏离规定学车区域>的逻辑处理：只对科目二的实操学习处理
            {
                aNADrivingInvalidOtherList.Clear();
                var tempList = aNADrivingInvalidMoreList.Where(o => o.DeviceNo == item.DeviceNo &&
                    (
                        (o.StartTime > tempEntity.StartStudyTime && o.StartTime < tempEntity.EndStudyTime)
                     || (o.EndTime > tempEntity.StartStudyTime && o.EndTime < tempEntity.EndStudyTime)
                     || (tempEntity.StartStudyTime > o.StartTime && tempEntity.StartStudyTime < o.EndTime)
                     || (tempEntity.EndStudyTime > o.StartTime && tempEntity.EndStudyTime < o.EndTime)
                    )).OrderBy(o => o.DeviceNo).ThenBy(o => o.StartTime);
                aNADrivingInvalidOtherList.AddRange(tempList);
                aNADrivingInvalidOtherList.ForEach(obj =>
                    {
                        ResetField(ref tempEntity);
                        if (obj.StartTime > tempEntity.StartStudyTime)
                        {
                            tempEntity.ValidActualTime = Math.Round(tempEntity.StartStudyTime.DateDiff(DateInterval.Second, obj.StartTime.AddSeconds(-1)) / 60.0);
                            //超出每天学习总时间
                            if ((tempEntity.TotalValidActualTime + tempEntity.ValidActualTime) > tempEntity.ValidStudyTime)
                            {
                                tempEntity.InValidFlag = 1;
                                tempEntity.TypeName = "超出每天学习总时间";
                                tempEntity.ValidActualTime = tempEntity.ValidStudyTime - tempEntity.TotalValidActualTime;
                                tempEntity.StartStudyTime = tempEntity.StartStudyTime.AddMinutes((double)(tempEntity.ValidStudyTime - tempEntity.TotalValidActualTime));
                                tempEntity.InvalidActualTime = Math.Round(tempEntity.StartStudyTime.DateDiff(DateInterval.Second,obj.StartTime.AddSeconds(-1)) / 60.0);
                                tempEntity.TotalInvalidActualTime = tempEntity.TotalInvalidActualTime + tempEntity.InvalidActualTime;
                                tempEntity.Remark = string.Format("无效的学习时间段：{0}~{1}", tempEntity.StartStudyTime, obj.StartTime.AddSeconds(-1));
                                tempEntity.StartStudyTime = obj.StartTime.AddSeconds(1);

                                //添加到无效记录表
                                studyRecordInvalidList.Add(new StudyRecordInvalid
                                {
                                    StudyRecordID = item.StudyRecordID,
                                    InvalidActualTime = tempEntity.InvalidActualTime,
                                    TypeName = tempEntity.TypeName,
                                    Remark = tempEntity.Remark
                                });
                            }
                            else   //合法的学习时间
                            {
                                tempEntity.ValidActualTime = Math.Round(tempEntity.StartStudyTime.DateDiff(DateInterval.Second,obj.StartTime.AddSeconds(-1)) / 60.0);
                                tempEntity.StartStudyTime = obj.StartTime;
                                tempEntity.TotalValidActualTime = tempEntity.TotalValidActualTime + tempEntity.ValidActualTime;
                            }
                        }

                        #region 无效学习时间的分析记录
                        //1:道路进出分析记录;2:道路停留超时分析记录;3:区域进出分析记录;4:区域停留超时分析记录。
                        tempEntity.TypeName = getTypeName(obj.DataType);
                        tempEntity.InValidFlag = 1;
                        DateTime dtTemp = (tempEntity.EndStudyTime > obj.EndTime) ? obj.EndTime : tempEntity.EndStudyTime;

                        tempEntity.InvalidActualTime = Math.Round(tempEntity.StartStudyTime.DateDiff(DateInterval.Second,dtTemp) / 60.0);
                        tempEntity.TotalInvalidActualTime = tempEntity.TotalInvalidActualTime + tempEntity.InvalidActualTime;
                        tempEntity.Remark = string.Format("无效的学习时间段：{0}~{1}", tempEntity.StartStudyTime, dtTemp);
                        tempEntity.StartStudyTime = obj.EndTime.AddSeconds(1);

                        //添加到无效记录表
                        studyRecordInvalidList.Add(new StudyRecordInvalid
                        {
                            StudyRecordID = item.StudyRecordID,
                            InvalidActualTime = tempEntity.InvalidActualTime,
                            TypeName = tempEntity.TypeName,
                            Remark = tempEntity.Remark
                        });

                        tempEntity.ValidStudyTime_Exam = tempEntity.ValidStudyTime_Exam +
                            (double)Math.Round((decimal)(tempEntity.ValidActualTime * tempEntity.StudyRate));
                        tempEntity.RecordValidActualTime = tempEntity.RecordValidActualTime + tempEntity.ValidActualTime;
                        #endregion
                    });
            }
            temp = tempEntity;
        }

        private void ResetField(ref TempEntity tempEntity)
        {
            tempEntity.ValidActualTime_Operate = 0;
            tempEntity.InvalidActualTime_Operate = 0;
            // tempEntity.ValidActualTime_Simulate = 0;
            // tempEntity.InvalidActualTime_Simulate = 0;
            // tempEntity.ValidActualTime_Operate_Sub = 0;
            // tempEntity.InvalidActualTime_Operate_Sub = 0;
            // tempEntity.ValidActualTime_Simulate_Sub = 0;
            // tempEntity.InvalidActualTime_Simulate_Sub = 0;
        }

        #endregion
    }
}
