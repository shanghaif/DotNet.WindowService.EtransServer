using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ET.WinService.MileageStatistics.Entity;
using ET.WinService.MileageStatistics.DAL;
using log4net;

namespace ET.WinService.MileageStatistics
{
    /// <summary>
    /// 学习时间段内车辆行驶的里程数据
    /// </summary>
    public class MileageStat
    {
        protected ILog log = LogManager.GetLogger(typeof(MileageStat));

        StudyRecordDAL studentResultDAL;
        VehicleGPSInfoDAL vehicleGPSInfoDAL;
        DateTime analyseStartTime;
        DateTime analyseEndTime;

        public MileageStat()
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
                log.InfoFormat("行驶里程=>分析开始(从{0}到{1}),现在时间:{2}", analyseStartTime, analyseEndTime, DateTime.Now);
                List<StudyRecord> studentRecords = studentResultDAL.GetUnValidStudentRecord(analyseStartTime, analyseEndTime);
                int mileage = 0;
                int maxSpeed = 0;

                log.InfoFormat("行驶里程=>学习结果数量:{0}", studentRecords == null ? 0 : studentRecords.Count);

                if (studentRecords != null && studentRecords.Count > 0)
                {
                    foreach (var studentRecord in studentRecords)
                    {
                        log.InfoFormat("行驶里程=>分析学时ID:{0},终端ID:{1},开始", studentRecord.ID, studentRecord.DeviceNo);
                        //根据学习记录的开始时间和结束时间，查询轨迹数据
                        vehicleGPSInfoDAL.GetYesterDayGpsInfo(studentRecord.DeviceNo, studentRecord.SR_BeginTime, studentRecord.SR_EndTime, out maxSpeed, out mileage);

                        //分析当前学时记录，在轨迹表中的里程
                        studentResultDAL.Update_MaxSpeed_Mileage_HasCalcMileage(studentRecord.ID, maxSpeed, mileage);
                        log.InfoFormat("行驶里程=>分析学时ID:{0},终端ID:{1},结束", studentRecord.ID, studentRecord.DeviceNo);
                    }
                    studentRecords = null;
                }
                log.InfoFormat("行驶里程=>分析结束,现在时间:{0}", DateTime.Now);
            }
            catch (Exception ex)
            {
                log.Error("行驶里程=>分析错误:", ex);
            }
        }

    }
}
