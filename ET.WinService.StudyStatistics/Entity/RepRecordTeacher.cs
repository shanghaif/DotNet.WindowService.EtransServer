using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.StudyStatistics.Entity
{
    /// <summary>
    /// 描  述：教练教学记录临时表
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 时  间：2013年1月17日
    /// 修  改：
    /// 原  因：
    /// </summary>
    public class RepRecordTeacher
    {
        public int StudyRecordID { get; set; }          //对应w_StudyRecord_Result表的ID
        public int StuID { get; set; }
        public int SubjectID { get; set; }              //科目ID
        public DateTime SR_BeginTime { get; set; }
        public DateTime SR_EndTime { get; set; }
        public float SR_ActualTime { get; set; }        //实际学习时间(分钟)
        public int ValidFlag { get; set; }              //记录的有效性标识，参考表T_StudyRecord_InvalidType的BitFlag字段值
        public string T_Teacher { get; set; }           //教练编号
        public short HasStat { get; set; }
        public short TeacherType { get; set; }            //教练类型:1表示理论教练;2:表示实操教练;3:表示理论、实操教练
        public int StudyType { get; set; }

        public RepRecordTeacher()
        {
            ValidFlag = 1;
            HasStat = 0;
            TeacherType = 0;
            StudyType = 1;
        }
    }
}
