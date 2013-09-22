using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.StudyStatistics.Entity
{
    public class TempEntity
    {
        public double? StudyTime_Simulate { get; set; } //int
        /// <summary>
        /// 理论学习系数
        /// </summary>
        public double? StudyRate_Theoretical { get; set; }

        /// <summary>
        /// 理论学习时间(小时)
        /// </summary>
        public double? StudyTime_Theoretical { get; set; }

        /// <summary>
        /// 实操学习系数
        /// </summary>
        public double? StudyRate_Operate { get; set; }

        /// <summary>
        /// 实操学习时间(小时)
        /// </summary>
        public double? StudyTime_Operate { get; set; }

        /// <summary>
        /// 汇总表有效学时成绩(分钟)
        /// </summary>
        public double? StuStatValidStudyTime_Exam { get; set; }

        /// <summary>
        /// 学习时长_实操
        /// </summary>
        public double SR_RealTime_Operate { get; set; }
        public double SR_ActualTime_Operate { get; set; }
        public double ValidActualTime_Operate { get; set; }
        public double? InvalidActualTime_Operate { get; set; }
        public double SR_RealTime_Simulate { get; set; }
        public double? SR_ActualTime_Simulate { get; set; }
        //public double? ValidActualTime_Simulate { get; set; }
       // public double? InvalidActualTime_Simulate { get; set; }
       // public double ValidActualTime_Operate_Sub { get; set; }
        public double InvalidActualTime_Operate_Sub { get; set; }
       // public double? ValidActualTime_Simulate_Sub { get; set; }
       // public double InvalidActualTime_Simulate_Sub { get; set; }
        /// <summary>
        /// 系数(理论、实操)
        /// </summary>
        public double? StudyRate { get; set; }
        public int PirorStuID { get; set; }
        public int PirorSubjectID { get; set; }
        public int? PirorStudyType { get; set; }
        public DateTime PriorStatDate { get; set; }
        public DateTime StatDate { get; set; }
        public DateTime StartStudyTime { get; set; }
        public DateTime EndStudyTime { get; set; }



        /// <summary>
        /// 学习时间(分钟)
        /// </summary>
        public double TotalRealTime { get; set; }

        /// <summary>
        /// 实际学习时间(分钟)
        /// </summary>
        public double TotalActualTime { get; set; }

        /// <summary>
        /// 有效学习时间(分钟)
        /// </summary>
        public double? TotalValidActualTime { get; set; }

        /// <summary>
        /// 无效学习时间(分钟)
        /// </summary>
        public double? TotalInvalidActualTime { get; set; }

        public double? InvalidActualTime { get; set; }

        public int InValidFlag { get; set; }
        //public DateTime SR_BeginTime { get; set; }
       // public DateTime SR_EndTime { get; set; }
        /// <summary>
        /// 未计算过学习成绩的有效学习时间(分钟),通过原始记录的有效学习时间*系数统计出来
        /// </summary>
        public double ValidStudyTime_Exam { get; set; }

        /// <summary>
        /// 终端号后8位
        /// </summary>
        public string PirorDeviceNo { get; set; }
        public DateTime PirorStartTime { get; set; }
        public DateTime PirorEndTime { get; set; }
        public int PirorDataType { get; set; }
        //public string DeviceNo { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DataType { get; set; }
        //public int CalcFlag_Add { get; set; }
        /// <summary>
        /// 系数(模拟)
        /// </summary>
        public double? StudyRate_Simulate { get; set; }
        /// <summary>
        /// 汇总表有效学习时间(分钟)
        /// </summary>
        public double? StuStatValidActualTime { get; set; }
        /// <summary>
        /// 汇总表无效学习时间(分钟)
        /// </summary>
        public double? StuStatInvalidActualTime { get; set; }
        public double? ValidActualTime { get; set; }
        //public double InvalidActualTime { get; set; }

        public int MinTime { get; set; }
        public int MaxTime { get; set; }
        /// <summary>
        /// 有效学习时间(分钟)，超出该部分视为无效的学习时间
        /// </summary>
        public int ValidStudyTime { get; set; }
        /// <summary>
        /// 有效开始时间，格式'hh:mm:ss'
        /// </summary>
        public string ValidStartTime { get; set; }

        /// <summary>
        /// 当前记录的有效学习时间(分钟)
        /// </summary>
        public double? RecordValidActualTime { get; set; }
        /// <summary>
        /// 当前记录的无效学习时间(分钟)
        /// </summary>
        public double? RecordInvalidActualTime { get; set; }

        /// <summary>
        /// 有效结束时间，格式'hh:mm:ss'
        /// </summary>
        public string ValidEndTime { get; set; }

        public DateTime vdt_s { get; set; }
        public DateTime vdt_e { get; set; }
        public string TypeName { get; set; }
        public string Remark { get; set; }

        public TempEntity()
        {
            TotalRealTime = 0;
            TotalRealTime = 0;
            TotalActualTime = 0;
            ValidActualTime = 0;
            TotalValidActualTime = 0;
            TotalInvalidActualTime = 0;
            ValidStudyTime_Exam = 0;
            SR_RealTime_Operate = 0;
            SR_ActualTime_Operate = 0;
            ValidActualTime_Operate = 0;
            InvalidActualTime_Operate = 0;
            SR_RealTime_Simulate = 0;
            SR_ActualTime_Simulate = 0;
            //ValidActualTime_Simulate = 0;
           // InvalidActualTime_Simulate = 0;
           // ValidActualTime_Operate_Sub = 0;
            InvalidActualTime_Operate_Sub = 0;
           // ValidActualTime_Simulate_Sub = 0;
           // InvalidActualTime_Simulate_Sub = 0;
            RecordValidActualTime = 0;
            RecordInvalidActualTime = 0;
            StudyTime_Simulate = 0;
        }
    }
}
