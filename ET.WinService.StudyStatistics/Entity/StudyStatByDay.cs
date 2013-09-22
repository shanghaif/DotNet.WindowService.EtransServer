using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.StudyStatistics.Entity
{
    public class StudyStatByDay
    {
        public int ID { get; set; }
        public int StuID { get; set; }
        /// <summary>
        /// 统计日期
        /// </summary>
        public DateTime StatDate { get; set; }
        /// <summary>
        /// 科目ID
        /// </summary>
        public int SubjectID { get; set; }

        /// <summary>
        /// 学习时间
        /// </summary>
        public double SR_RealTime { get; set; }
        /// <summary>
        /// 实际学习时间(分钟)
        /// </summary>
        public double SR_ActualTime { get; set; }
        /// <summary>
        /// 有效学习时间(分钟)
        /// </summary>
        public double? ValidActualTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double? InvalidActualTime { get; set; }
        /// <summary>
        /// 系数(理论、实操)
        /// </summary>
        public double? StudyRate { get; set; }
        public DateTime ModifyDatetime { get; set; }
        public bool HasStatExam { get; set; }
        public double ValidStudyTime_Exam { get; set; }
       // public double SR_RealTime_Operate { get; set; }
       // public double SR_ActualTime_Operate { get; set; }
        //public double ValidActualTime_Operate { get; set; }
        //public double? InvalidActualTime_Operate { get; set; }
        //public double SR_RealTime_Simulate { get; set; }
        //public double? SR_ActualTime_Simulate { get; set; }
        //public double? ValidActualTime_Simulate { get; set; }
        //public double? InvalidActualTime_Simulate { get; set; }
        //public double? StudyRate_Simulate { get; set; }
        public short CalcFlag { get; set; }
        public int? StudyType { get; set; }      //学习类型标识 0 理论; 1 实操; 2 模拟

    }
}
