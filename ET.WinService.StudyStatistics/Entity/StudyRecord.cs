using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.StudyStatistics.Entity
{

    /// <summary>
    /// 学习结果临时表
    /// </summary>
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：
    /// 版本号：1.0.0.1
    /// 作  者：黄冠群 (hgq@e-trans.com.cn)
    /// 日  期：2013年1月14日
    /// 修  改：
    /// 原  因：
    /// ------------------------------------------------------------------------------
    /// </remarks>
    /// <example>
    /// [示例代码在这里写入]
    /// </example>
    public class StudyRecord
    {
        public int StudyRecordID { get; set; }      //对应w_StudyRecord_Result表的ID
        public int StuID { get; set; }
        public int SubjectID { get; set; }          //科目ID
        public DateTime SR_BeginTime { get; set; }
        public DateTime SR_EndTime { get; set; }
        public float SR_RealTime { get; set; }      //学习时间(分钟)
        public float SR_ActualTime { get; set; }    //实际学习时间(分钟)
        public double? ValidActualTime { get; set; }  //实际有效学习时间(分钟)
        public string TypeName { get; set; }        //无效类型名称
        public string Remark { get; set; }          //说明
        public double? StudyRate { get; set; }        //系数
        public short HasStat { get; set; }          //是否已经统计过(T_StudyStatByDay)
        public int ValidFlag { get; set; }          //记录的有效性标识，参考表T_StudyRecord_InvalidType的BitFlag字段值
        public string T_Teacher { get; set; }       //教练编号
        public string DeviceNo { get; set; }        //终端号后8位
        public short IsValidArea { get; set; }      //是否为有效区域
        public short IsValidLine { get; set; }      //是否为有效线路
        public int CalcFlag_Add { get; set; }       //统计标识位,按位存储。从低到高各比特的置位分别代表(2的n次方)：1(第0位):  正常状态; 2(第1位):  是否统计超出区域学习记录; 4(第2位):  异常的学习记录。
        public int? StudyType { get; set; }          //学习类型标识 0 理论; 1 实操; 2 模拟
        public short ValidSpeed { get; set; }       //是否有效速度:0表示无效;1表示有效
        public short HasCalcVehStatus { get; set; } //是否已经统计过车辆状态
        /// <summary>
        /// 是否为合法车辆状态
        /// </summary>
        public short ValidVehStatus { get; set; }   
        public int TeacherType { get; set; }
        public int SR_StudyType { get; set; }
        public int Mileage { get; set; }            //里程数
        /// <summary>
        /// 1:当天的学习记录; 2:跨天的学习记录
        /// </summary>
        public int CalcFlag { get; set; }          

        public StudyRecord()
        {
            //设置默认值
            HasStat = 0;
            ValidFlag = 1;
            IsValidArea = 0;
            IsValidLine = 0;
            CalcFlag_Add = 1;
            ValidSpeed = 0;
            HasCalcVehStatus = 0;
            ValidVehStatus = 0;
            ValidActualTime = 0;
        }
    }
}
