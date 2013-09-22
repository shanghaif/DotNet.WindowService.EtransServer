using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.StudyStatistics.Entity
{
    /// <summary>
    /// 描  述：学员学习记录临时表
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 时  间：2013年1月21日
    /// 修  改：
    /// 原  因：
    /// </summary>
    public class RepRecordStudent
    {
        public int StudyRecordID { get; set; }          //对应w_StudyRecord_Result表的ID
        public int StuID { get; set; }
        public int SubjectID { get; set; }              //科目ID
        public DateTime SR_BeginTime { get; set; }
        public DateTime SR_EndTime { get; set; }
        public float SR_ActualTime { get; set; }        //实际学习时间(分钟)
        public int ValidFlag { get; set; }              //记录的有效性标识，参考表T_StudyRecord_InvalidType的BitFlag字段值
        public short HasStat { get; set; }

        public RepRecordStudent()
        {
            ValidFlag = 1;
            HasStat = 0;
        }
    }
}
