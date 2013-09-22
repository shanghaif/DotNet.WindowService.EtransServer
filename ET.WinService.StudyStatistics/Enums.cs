using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ET.WinService.Core.Utility;

namespace ET.WinService.StudyStatistics
{
    public enum InvalidType
    {

        /// <summary>
        /// 正常
        /// </summary>
        [Remark("正常")]
        Normal=1,

        
        /// <summary>
        /// 不在准教车型范围内
        /// </summary>
        [Remark("不在准教车型范围内")]
        NotInTeachingRange=2,

        /// <summary>
        /// 教练教车重叠时间段
        /// </summary>
        [Remark("教练教车重叠时间段")]
        TeacherOverlappingTime=4,

        /// <summary>
        /// 学员学车重叠时间段
        /// </summary>
        [Remark("学员学车重叠时间段")]
        StudentOverlappingTime=8,

        /// <summary>
        /// 结束时间大于系统时间
        /// </summary>
        [Remark("结束时间大于系统时间")]
        EndTimeLargerSystemTime=16,

        /// <summary>
        /// 不在规定的时间段内学习
        /// </summary>
        [Remark("不在规定的时间段内学习")]
        NotInSpecifiedTime=32,

        /// <summary>
        /// 超出每天学习总时间
        /// </summary>
        [Remark("超出每天学习总时间")]
        BeyondStudyTotalTime=64,

        /// <summary>
        /// 异常数据
        /// </summary>
        [Remark("异常数据")]
        AbnormalData=128,

        /// <summary>
        /// 偏离规定行驶路线
        /// </summary>
        [Remark("偏离规定行驶路线")]
        DeviationPrescribedRoute=256,

        /// <summary>
        /// 偏离规定学车区域
        /// </summary>
        [Remark("偏离规定学车区域")]
        DeviationSpecifiedArea=512,

        /// <summary>
        /// 路线超时停车
        /// </summary>
        [Remark("路线超时停车")]
        RouteOvertimeParking=1024,

        /// <summary>
        /// 区域超时停车
        /// </summary>
        [Remark("区域超时停车")]
        RegionalOvertimeParking =2048,

        /// <summary>
        /// 非法车辆状态
        /// </summary>
        [Remark("非法车辆状态")]
        IllegalVehicleState = 4096,

        /// <summary>
        /// 学员卡未激活
        /// </summary>
        [Remark("学员卡未激活")]
        StudentCardNotActivated =8192,

        /// <summary>
        /// 教练卡未激活
        /// </summary>
        [Remark("教练卡未激活")]
        TeacherCardNotActivated = 16384,

        /// <summary>
        /// 科目未审批
        /// </summary>
        [Remark("科目未审批")]
        SubjectsNotApproval = 32768,

        /// <summary>
        /// 不在准教科目范围内
        /// </summary>
        [Remark("不在准教科目范围内")]
        NotInSubjectRange = 65536,

        /// <summary>
        /// 不在有效速度范围内
        /// </summary>
        [Remark("不在有效速度范围内")]
        NotInSpeedRange = 131072,

        /// <summary>
        /// 单次学时小于最小要求学时
        /// </summary>
        [Remark("单次学时小于最小要求学时")]
        LessThanMinimumHours = 262144
    }
}
