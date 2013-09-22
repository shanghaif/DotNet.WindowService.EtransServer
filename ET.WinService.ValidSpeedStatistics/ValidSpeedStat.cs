using System;
using System.Collections.Generic;
using System.Linq;
using ET.WinService.ValidSpeedStatistics.DAL;
using ET.WinService.ValidSpeedStatistics.Entity;
using log4net;

namespace ET.WinService.ValidSpeedStatistics
{
    /// <summary>
    /// 检查学员学车是否达到对速度限制的要求
    /// </summary>
    public class ValidSpeedStat
    {
        protected ILog log = LogManager.GetLogger(typeof(ValidSpeedStat));

        StudyRecordDAL studentResultDAL;
        VehicleGPSInfoDAL vehicleGPSInfoDAL;
        DateTime analyseStartTime;
        DateTime analyseEndTime;

        public ValidSpeedStat()
        {
            studentResultDAL = new StudyRecordDAL();
            vehicleGPSInfoDAL = new VehicleGPSInfoDAL();

            analyseStartTime = DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd ") + InitParam.Instance.DayStartTime + ":00");
            analyseEndTime = DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd ") + InitParam.Instance.DayEndTime + ":00");
        }

        public void Execute()
        {
            try
            {
                log.InfoFormat("有效速度=>分析开始(从{0}到{1}),现在时间:{2}", analyseStartTime, analyseEndTime, DateTime.Now);
                List<StudyRecord> studentRecords = studentResultDAL.GetUnValidStudentRecord(analyseStartTime, analyseEndTime);
                List<GpsInfo> yesterdayCarGpsInfos = null;

                log.InfoFormat("有效速度=>学习结果数量:{0}", studentRecords == null ? 0 : studentRecords.Count);

                if (studentRecords != null && studentRecords.Count > 0)
                {
                    foreach (var studentRecord in studentRecords)
                    {
                        log.InfoFormat("有效速度=>分析学时ID:{0},终端ID:{1},开始", studentRecord.ID, studentRecord.DeviceNo);
                        //根据学习记录的开始时间和结束时间，查询轨迹数据
                        yesterdayCarGpsInfos = vehicleGPSInfoDAL.GetYesterDayGpsInfo(studentRecord.DeviceNo, studentRecord.SR_BeginTime, studentRecord.SR_EndTime);

                        //分析当前学时记录，在轨迹表中的速度
                        Analyse(studentRecord, yesterdayCarGpsInfos);

                        yesterdayCarGpsInfos = null;
                        log.InfoFormat("有效速度=>分析学时ID:{0},终端ID:{1},结束", studentRecord.ID, studentRecord.DeviceNo);
                    }
                    studentRecords = null;
                }
                log.InfoFormat("有效速度=>分析结束,现在时间:{0}", DateTime.Now);
            }
            catch (Exception ex)
            {
                log.Error("有效速度=>分析错误:", ex);
            }
        }

        private void Analyse(StudyRecord studentRecord, List<GpsInfo> yesterdayCarGpsInfos)
        {
            if (yesterdayCarGpsInfos != null && yesterdayCarGpsInfos.Count > 0)
            {
                int gpsCount = yesterdayCarGpsInfos.Count;
                int gpsValidCount = 0;

                foreach (var gpsInfo in yesterdayCarGpsInfos)
                {
                    if (gpsInfo.Speed >= InitParam.Instance.MinSpeed && gpsInfo.Speed < InitParam.Instance.MaxSpeed)
                    {
                        gpsValidCount++;
                    }
                }

                //计算百分比
                float percent = ((float)gpsValidCount / (float)gpsCount);
                if (percent >= InitParam.Instance.ValidSpeedPercent)
                {
                    studentResultDAL.Update_ValidSpeed_HasCalcSpeed(studentRecord.ID, true);
                }
                else
                {
                    studentResultDAL.Update_ValidSpeed_HasCalcSpeed(studentRecord.ID, false);
                }
            }
            else
            {
                studentResultDAL.Update_ValidSpeed_HasCalcSpeed(studentRecord.ID, false);
            }

            log.InfoFormat("有效速度=>分析学时:{0},设备ID:{1},轨迹数量:{2}", studentRecord.ID, studentRecord.DeviceNo, yesterdayCarGpsInfos == null ? 0 : yesterdayCarGpsInfos.Count);


            studentRecord = null;
            yesterdayCarGpsInfos = null;
        }

    }
}
