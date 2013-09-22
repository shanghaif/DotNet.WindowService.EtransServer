using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using ET.WinService.VehicleStatusStatistics.DAL;
using ET.WinService.VehicleStatusStatistics.Entity;

namespace ET.WinService.VehicleStatusStatistics
{
    /// <summary>
    /// 车辆状态(如ACC、发动机状态)的分析数据，用于学时分析中的有效区域检测
    /// </summary>
    public class VehicleStatusStat
    {
        protected ILog log = LogManager.GetLogger(typeof(VehicleStatusStat));

        StudyRecordDAL studentResultDAL;
        VehicleGPSInfoDAL vehicleGPSInfoDAL;
        DateTime analyseStartTime;
        DateTime analyseEndTime;

        public VehicleStatusStat()
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
                log.InfoFormat("车辆状态=>分析开始(时间段从{0}到{1}),现在时间:{2}", analyseStartTime, analyseEndTime, DateTime.Now);
                List<StudyRecord> studentRecords = studentResultDAL.GetUnValidStudentRecord(analyseStartTime, analyseEndTime);
                List<GpsInfo> yesterdayCarGpsInfos = null;

                log.InfoFormat("车辆状态=>学习结果:{0}", studentRecords == null ? 0 : studentRecords.Count);

                if (studentRecords != null && studentRecords.Count > 0)
                {
                    foreach (var studentRecord in studentRecords)
                    {
                        log.InfoFormat("车辆状态=>分析学时ID:{0},终端ID:{1},开始", studentRecord.ID, studentRecord.DeviceNo);
                        //根据学习记录的开始时间和结束时间，查询轨迹数据
                        yesterdayCarGpsInfos = vehicleGPSInfoDAL.GetYesterDayGpsInfo(studentRecord.DeviceNo, studentRecord.SR_BeginTime, studentRecord.SR_EndTime);

                        //分析当前学时记录，在轨迹表中的车辆状态
                        Analyse(studentRecord, yesterdayCarGpsInfos);

                        yesterdayCarGpsInfos = null;
                        log.InfoFormat("车辆状态=>分析学时ID:{0},终端ID:{1},结束", studentRecord.ID, studentRecord.DeviceNo);
                    }
                    studentRecords = null;
                }
                log.InfoFormat("车辆状态=>分析结束,现在时间:{0}", DateTime.Now);
            }
            catch (Exception ex)
            {
                log.Error("车辆状态=>分析错误:", ex);
            }
        }

        private void Analyse(StudyRecord studentRecord, List<GpsInfo> yesterdayCarGpsInfos)
        {
            if (yesterdayCarGpsInfos != null && yesterdayCarGpsInfos.Count > 0)
            {
                int gpsCount = yesterdayCarGpsInfos.Count;

                //取得ACC状态和ENG状态
                List<VehicleGPSInfo> vehicleGpsInfo = new List<VehicleGPSInfo>();
                bool accState = false, engState = false;
                foreach (var gpsInfo in yesterdayCarGpsInfos)
                {
                    //获取ACC状态、ENG状态
                    Get_ACC_ENG_State(gpsInfo.GpsState, out accState, out engState);

                    //添加到缓冲
                    vehicleGpsInfo.Add(new VehicleGPSInfo() { GpsTime = gpsInfo.GpsTime, ACC = accState, ENG = engState });
                }

                //检查连续时间
                VehicleGPSInfo lastVehicleGPSInfo;
                VehicleGPSInfo nowVehicleGPSInfo;
                double accCloseTime = 0;    //ACC关的时间总长
                double engCloseTime = 0;    //ENG关的时间总长
                double intervalTime = 0;    //上一次GPS时间与本次GPS时间的间隔
                for (int i = 1; i < vehicleGpsInfo.Count; i++)
                {
                    lastVehicleGPSInfo = vehicleGpsInfo[i - 1];
                    nowVehicleGPSInfo = vehicleGpsInfo[i];
                    intervalTime = (nowVehicleGPSInfo.GpsTime - lastVehicleGPSInfo.GpsTime).TotalSeconds;

                    if (!lastVehicleGPSInfo.ACC && !nowVehicleGPSInfo.ACC)
                        accCloseTime += intervalTime;
                    else
                        accCloseTime = 0;

                    if (!lastVehicleGPSInfo.ENG && !nowVehicleGPSInfo.ENG)
                        accCloseTime += intervalTime;
                    else
                        engCloseTime = 0;
                }
                nowVehicleGPSInfo = null;
                lastVehicleGPSInfo = null;

                //判断最终结果
                bool validACCVehStatus = false;
                bool validENGVehStatus = false;
                if (accCloseTime <= InitParam.Instance.CheckACCDuration * 60)
                    validACCVehStatus = true;
                else
                    validACCVehStatus = false;

                if (engCloseTime <= InitParam.Instance.CheckENGDuration * 60)
                    validENGVehStatus = true;
                else
                    validENGVehStatus = false;

                bool result = false;
                //如果启用ACC检测和发动机检测
                if (InitParam.Instance.CheckACCIsEnabled && InitParam.Instance.CheckENGsEnabled)
                {
                    result = validACCVehStatus && validENGVehStatus;
                }
                else if (InitParam.Instance.CheckACCIsEnabled)  //如果只启用ACC检测
                {
                    result = validACCVehStatus;
                }
                else if (InitParam.Instance.CheckENGsEnabled)   //如果只发动机检测
                {
                    result = validENGVehStatus;
                }
                //输出结果
                studentResultDAL.Update_ValidVehStatus_HasCalcVehStatus(studentRecord.ID, result);

                //不通过检查的学习记录，写入无效信息
                if (!result)
                {
                    //无效的学习记录
                    studentResultDAL.Update_HasStat(studentRecord.ID);

                    //无效类型
                    studentResultDAL.Insert_StudyRecord_Invalid(studentRecord.ID, studentRecord.SR_BeginTime, studentRecord.SR_EndTime);
                }
            }
        }

        /// <summary>
        /// 获取ACC状态和ENG状态
        /// </summary>
        /// <param name="gpsState">GPS状态</param>
        /// <param name="accState">返回ACC状态</param>
        /// <param name="engState">返回ENG状态</param>
        private void Get_ACC_ENG_State(string gpsState, out bool accState, out bool engState)
        {
            accState = false;
            engState = false;

            string[] gpsStatusArray = gpsState.Split('、');
            for (int i = 0; i < gpsStatusArray.Length; i++)
            {
                if (gpsStatusArray[i].IndexOf("ACC") != -1)
                {
                    accState = (gpsStatusArray[i] == "ACC开");
                }
                if (gpsStatusArray[i].IndexOf("发动机") != -1)
                {
                    engState = (gpsStatusArray[i] == "发动机开");
                }
            }
        }
    }
}

