using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using ET.WinService.AreaStatistics.Entity;
using ET.WinService.AreaStatistics.DAL;
using System.Linq;
using System.Threading;
using System.Configuration;
using ET.WinService.Core.Task;
using System.Xml.Linq;
using System.Collections;
namespace ET.WinService.AreaStatistics
{
    /// <summary>
    /// 车辆进出区域统计
    /// </summary>
    /// <remarks>
    /// ------------------------------------------------------------------------------
    /// Copyright:Copyright (c) 2013,广州亿程交通信息有限公司 All rights reserved.
    /// 描  述：
    /// 版本号：1.0.0.1
    /// 作  者：陈炯炯 (chenjiongjiong@e-trans.com.cn)
    /// 日  期：2013年1月15日
    /// 修  改：
    /// 原  因：
    /// ------------------------------------------------------------------------------
    /// </remarks>
    /// <example>
    /// [示例代码在这里写入]
    /// </example>
    public class AreaStat
    {
        /// <summary>
        /// 分析条件操作类
        /// </summary>
        private AnalyserAreaEnterAndLeaveDAL analyserConditionDal;

        private AnalyserRoadEnterAndLeaveMonitorDAL analyserRoadEnterAndLeaveMonitorDAL;
        /// <summary>
        /// 区域节点操作类
        /// </summary>
        private ParamPolygonDetailDAL polygonDetailDAL;

        /// <summary>
        /// 道路节点操作类
        /// </summary>
        private ParamRoadSegmentDetailDAL roadpolygonDetailDAL;

        /// <summary>
        /// 组与车辆关系操作类
        /// </summary>
        private AnalyseGroupVehiclesDAL analyseGroupVehiclesDAL;

        /// <summary>
        /// 车辆轨迹数据操作类
        /// </summary>
        private VehicleGpsInfoDAL vehicleGpsInfoDAL;

        /// <summary>
        /// 进出区域操作类
        /// </summary>
        private DrivingAreaEnterAndLeaveMonitorDAL drivingAreaEnterAndLeaveMonitorDAL;


        /// <summary>
        /// 进出道路操作类
        /// </summary>
        private DrivingRoadEnterAndLeaveMonitorDAL drivingRoadEnterAndLeaveMonitorDAL;

        /// <summary>
        /// 车辆操作类
        /// </summary>
        private VehicleDAL vehicleDAL;

        /// <summary>
        /// 车辆条件信息缓冲区(区域)
        /// </summary>
        private Dictionary<string, string> AreaCache;

        /// <summary>
        /// 车辆条件信息缓冲区(路段)
        /// </summary>
        private Dictionary<string, string> LoadCache;
        //private Queue AreaVehicleQueue;


        //所有车辆的缓冲队列，所有的程序公用的
        private Queue VehicleQueue;

        /// <summary>
        /// 分析组与路段关系操作类
        /// </summary>
        private ParamRoadSegmentsDAL paramRoadSegmentsDAL;


        /// <summary>
        /// 分析个数
        /// </summary>
        int AnalyseCount = 0;

        //分析线程
        private Thread[] analyseThread;

        /// <summary>
        /// 已经分析数目
        /// </summary>
        private int HaveAnalyseCount = 0;
        public AreaStat()
        {
            analyserConditionDal = new AnalyserAreaEnterAndLeaveDAL();
            polygonDetailDAL = new ParamPolygonDetailDAL();
            roadpolygonDetailDAL = new ParamRoadSegmentDetailDAL();
            analyseGroupVehiclesDAL = new AnalyseGroupVehiclesDAL();
            vehicleGpsInfoDAL = new VehicleGpsInfoDAL();

            drivingAreaEnterAndLeaveMonitorDAL = new DrivingAreaEnterAndLeaveMonitorDAL();

            drivingRoadEnterAndLeaveMonitorDAL = new DrivingRoadEnterAndLeaveMonitorDAL();
            vehicleDAL = new VehicleDAL();

            analyserRoadEnterAndLeaveMonitorDAL = new AnalyserRoadEnterAndLeaveMonitorDAL();

            paramRoadSegmentsDAL = new ParamRoadSegmentsDAL();
        }
        protected ILog log = LogManager.GetLogger(typeof(AreaStat));
        public void Execute()
        {
            try
            {
                this.AnalyseCount = 0;
                this.HaveAnalyseCount = 0;
                log.Info("开始进行车辆进出区域分析以及路段分析###########");
                //获取配置文件配置的车辆组
                long AreaGroupID = long.Parse(ConfigurationManager.AppSettings["AreaGroupID"].ToString());
                #region 获取区域分析的所有条件
                List<AnalyserAreaEnterAndLeaveMonitor> analyserConditionList;
                if (AreaGroupID == 0)
                    //首先获取所有的分析条件
                    analyserConditionList = GetAnalyseContion();
                else
                    analyserConditionList = GetAnalyseContion(AreaGroupID);
                //由于加了路段设置，假如没有设置区域分析条件，必须jixu
                //if (analyserConditionList == null)
                //    return;
                #endregion
                #region 获取路段分析的所有条件
                List<AnalyserRoadEnterAndLeaveMonitor> analyserRoadConditionList;
                analyserRoadConditionList = GetLoadAnalyseContion(AreaGroupID);
                //if (analyserRoadConditionList == null)
                //    return;
                #endregion
                this.AreaCache = new Dictionary<string, string>();
                this.LoadCache = new Dictionary<string, string>();
                this.VehicleQueue = new Queue();

                //获取所有的车辆信息，存入缓存队列VehicleQueue
                List<Vehicle> VehicleList = vehicleDAL.GetList();
                foreach (Vehicle perVehicle in VehicleList)
                {
                    //将每一辆车辆信息入列,车辆信息队列，队列存储的试试为:车辆ID+CarID
                    //if (perVehicle.VehicleID == 676)
                    this.VehicleQueue.Enqueue(perVehicle.VehicleID + "," + perVehicle.CarId);
                    //AnalysePerVehicle(perVehicle, perContion, polygonDetailList);
                }

                //根据条件设置分析每一个车辆组
                #region 存储区域分析条件下每一辆车的信息
                if (analyserConditionList != null)
                {
                    foreach (AnalyserAreaEnterAndLeaveMonitor perContion in analyserConditionList)
                    {
                        SaveAreaAnalyse(perContion);
                    }
                }
                #endregion
                #region 存储路段分析条件下每一辆车的信息
                if (analyserRoadConditionList != null)
                {
                    foreach (AnalyserRoadEnterAndLeaveMonitor perRoadContion in analyserRoadConditionList)
                    {
                        SaveLoadAnalyse(perRoadContion);
                    }
                }
                #endregion
                log.Info(string.Format("区域分析一共要分析{0}辆车", this.AreaCache.Count));
                //采用多线程分析车辆的电子围栏进出区域
                //workThread = new Thread(new ParameterizedThreadStart());
                //workThread.IsBackground = true;
                //workThread.Start();
                //线程数
                int TheadCount = int.Parse(ConfigurationManager.AppSettings["TheadCount"].ToString());
                analyseThread = new Thread[TheadCount];
                for (int i = 0; i < analyseThread.Length; i++)
                {
                    analyseThread[i] = new Thread(new ThreadStart(AnalysePerVehicle));
                    analyseThread[i].IsBackground = true;
                    analyseThread[i].Start();
                }
                //当队列为为空时，释放线程
                //while (this.AreaVehicleQueue.Count == 0 && IfWhile==1)
                //{
                //    log.Info("完成车辆进出区域分析###########");
                //    //睡眠五分钟
                //    Thread.Sleep(300000);
                //    for (int i = 0; i < analyseThread.Length; i++)
                //    {
                //        if (analyseThread[i]!=null)
                //        analyseThread[i].Abort();
                //    }
                //    IfWhile = 0;
                //}

            }
            catch (Exception ex)
            {
                log.Error("区域出入分析执行出错,错误原因:", ex);
            }
        }

        /// <summary>
        /// 获取所有的分析条件 add by chenjiongjiong 2013-01-15
        /// </summary>
        /// <returns></returns>
        private List<AnalyserAreaEnterAndLeaveMonitor> GetAnalyseContion()
        {
            return analyserConditionDal.GetList();
        }
        //获取指定车辆组的分析条件
        private List<AnalyserAreaEnterAndLeaveMonitor> GetAnalyseContion(long AreaGroupID)
        {
            return analyserConditionDal.GetList(AreaGroupID);
        }
        private List<AnalyserRoadEnterAndLeaveMonitor> GetLoadAnalyseContion(long AreaGroupID)
        {
            if (AreaGroupID == 0)
            {
                return analyserRoadEnterAndLeaveMonitorDAL.GetList();
            }
            else
            {
                return analyserRoadEnterAndLeaveMonitorDAL.GetList(AreaGroupID);
            }
        }
        /// <summary>
        /// 存错区域分析的所有车辆信息以及条件信息
        /// </summary>
        /// <param name="perContion"></param>
        private void SaveAreaAnalyse(AnalyserAreaEnterAndLeaveMonitor perContion)
        {
            try
            {

                //log.Info(string.Format("开始分析车辆组AnalyseGroupID为{0}的车组", perContion.AnalyseGroupID));
                int AreaID = perContion.AreaID;
                long AnalyseGroupID = perContion.AnalyseGroupID;
               
                //根据AnalyseGroupID获取分析的车辆列表
                List<AnalyseGroupVehicles> GroupVehiclesList = analyseGroupVehiclesDAL.GetList(AnalyseGroupID);
                if (GroupVehiclesList == null) return;
                string Value = string.Empty;
                //针对每一辆车进行分析
                foreach (AnalyseGroupVehicles perVehicle in GroupVehiclesList)
                {
                    //将每一辆车辆信息存入，字典的格式为:车辆ID，车辆ID+CarID+区域ID+最大的出区域时间
                    Value = perVehicle.VehicleID.ToString() + "," + perVehicle.CarId.ToString() + "," + AreaID.ToString() + "," + perContion.AllowLeaveMaxTime.ToString() + "," + perContion.AnalyseGroupID.ToString();
                    this.AreaCache.Add(perVehicle.VehicleID.ToString(), Value);
                    //AnalysePerVehicle(perVehicle, perContion, polygonDetailList);
                }
                //根据车辆ID获取车辆的轨迹数据，获取的是前一天的
                //log.Info(string.Format("开始分析车辆组AnalyseGroupID为{0}的车组", perContion.AnalyseGroupID));
            }
            catch (Exception ex)
            {
                log.Error("存储区域条件信息时出错,出错原因是:", ex);
            }
        }

        private void SaveLoadAnalyse(AnalyserRoadEnterAndLeaveMonitor perRoadContion)
        {
            try
            {
                long AnalyseGroupID = perRoadContion.AnalyseGroupID;
                //根据分析组获取路段集合
                IList<ParamRoadSegments> paramRoadSegmentsList = paramRoadSegmentsDAL.GetList(AnalyseGroupID);
                if (paramRoadSegmentsList == null) return;
                //根据AnalyseGroupID获取分析的车辆列表
                List<AnalyseGroupVehicles> GroupVehiclesList = analyseGroupVehiclesDAL.GetList(AnalyseGroupID);
                if (GroupVehiclesList == null) return;
              
            
                string value = string.Empty;
                //循环分析组的所有车辆
                foreach (AnalyseGroupVehicles perVehicle in GroupVehiclesList)
                {
                    //循环分析组的所有路段
                    foreach (ParamRoadSegments perRoadSegments in paramRoadSegmentsList)
                    {
                        //将每一辆车辆信息存入，字典的格式为:车辆ID，车辆ID+CarID+路段ID+最大的出区域时间+最大距离+分析组ID
                        value = perVehicle.VehicleID.ToString() + "," + perVehicle.CarId.ToString() + "," + perRoadSegments.ID.ToString().Trim() + "," + perRoadContion.AllowLeaveMaxTime.ToString() + "," + perRoadContion.ValidDistance.ToString() + "," + perRoadContion.AnalyseGroupID.ToString();
                        this.LoadCache.Add(perVehicle.VehicleID.ToString(), value);
                    }
                   
                }
            }
            catch (Exception ex)
            {
                log.Error("存储路段条件信息时出错,出错原因是:", ex);
            }

        }
        //判断点是否在区域内
        private bool IsPointInRegion(List<ParamPolygonDetail> polygonDetailList, VehicleGpsInfo vihicleGps)
        {
            common.GisPolyGon PolyGon = new common.GisPolyGon();
            //矩形点集合
            PolyGon.LGis_Point = new List<common.TGis_Point>();
            foreach (ParamPolygonDetail perPolygon in polygonDetailList)
            {
                PolyGon.LGis_Point.Add(new common.TGis_Point(perPolygon.Longitude, perPolygon.Latitude));
            }
            //判断点经纬度
            common.TGis_Point NowPoint = new common.TGis_Point();
            NowPoint.X = vihicleGps.Longitude;
            NowPoint.Y = vihicleGps.Latitude;
            //举行点经纬度极值
            PolyGon.polyextent = new common.TGIS_Extent();
            string aa = polygonDetailList.Min(O => O.Longitude).ToString();
            PolyGon.polyextent.XMin = polygonDetailList.Min(O => O.Longitude);
            PolyGon.polyextent.YMin = polygonDetailList.Min(O => O.Latitude);
            PolyGon.polyextent.XMax = polygonDetailList.Max(O => O.Longitude);
            PolyGon.polyextent.YMax = polygonDetailList.Max(O => O.Latitude);

            return PolyGon.IsPointInRegion(NowPoint);
        }

        /// <summary>
        /// 判断是否在道路内
        /// </summary>
        /// <param name="polygonDetailList">道路点围成的经纬度</param>
        /// <param name="vihicleGps">判断的点经纬度</param>
        /// <param name="AnalyserGroupID">分析组ID</param>
        /// <param name="Radius">道路允许偏离的值</param>
        /// <returns></returns>
        private bool IsPointInLoad(List<ParamRoadSegmentDetail> polygonDetailList, VehicleGpsInfo vihicleGps,long AnalyserGroupID,int Radius)
        {
            //添加道路信息
            common.GisRoadInfo gisRoadInfo = new common.GisRoadInfo();
            List<common.TGis_Point> TGis_Points=new List<common.TGis_Point>();
            common.TGis_Point perPoint;
            gisRoadInfo.RoadID = polygonDetailList[0].RoadID;
            gisRoadInfo.AnalyserGroupID=AnalyserGroupID;
            common.TaGisRoadManager tagisRoadManage = new common.TaGisRoadManager();
            foreach(ParamRoadSegmentDetail perParamRoadSegment in polygonDetailList)
            {
              perPoint=new common.TGis_Point();
              perPoint.X=perParamRoadSegment.Longitude;
              perPoint.Y=perParamRoadSegment.Latitude;
              TGis_Points.Add(perPoint);
            }
            tagisRoadManage.addRoad(gisRoadInfo, TGis_Points);
             //判断点经纬度
            common.TGis_Point NowPoint = new common.TGis_Point();
            NowPoint.X = vihicleGps.Longitude;
            NowPoint.Y = vihicleGps.Latitude;
            common.GisRoadResult gisRoadResult=tagisRoadManage.AnalyseRoadsDivergedByGroupID(NowPoint, AnalyserGroupID, gisRoadInfo.RoadID, Radius);
            return gisRoadResult.IsDiverged;
        }

        //道路分析
        private void AnalyseVehicleLoad(string Vehicleinfo, List<VehicleGpsInfo> vehicleGpsList)
        {
            //上次道路状态
            int LastIfInLoad = -1;
            //本次区域状态
            int NowIfInLoad = -1;
            //上次GPS时间
            DateTime LastGpsTime = DateTime.Now;
            //本次GPS时间
            DateTime NowGpsTime;
            //在道路外的时间
            int AllOutTime = -1;
            //是否曾经在区域内
            int IfOnceInLoad = -1;
            //进出道路的时间
            DateTime EnterTime = DateTime.Now;
            DateTime LeaveTime = DateTime.Now;
            //进出区域实体
            DrivingRoadEnterAndLeaveMonitor nowRoadMonitor;
            //道路ID
            int LoadID = -1;
            //离开道路最大时间
            int AllowLeaveMaxTime = -1;
            //分析组ID
            long AnalyseGroupID = -1;

            //车辆ID
            long VehicleID = -1;

            //允许最大的偏移值
            int ValidDistance=-1;
            //
            LoadID = int.Parse(Vehicleinfo.Split(',')[2].ToString());
            AllowLeaveMaxTime = int.Parse(Vehicleinfo.Split(',')[3].ToString());
            ValidDistance = int.Parse(Vehicleinfo.Split(',')[4].ToString());
            AnalyseGroupID = long.Parse(Vehicleinfo.Split(',')[5].ToString());
            VehicleID = long.Parse(Vehicleinfo.Split(',')[0].ToString());
            try
            {

                log.Info(string.Format("开始分析车辆ID为{0}的路段分析###########", VehicleID));
                //记录开始分析的时间
                DateTime AnalyseStartTime = DateTime.Now;
                //根据LoadID获取道路的所有节点ID
                List<ParamRoadSegmentDetail> polygonDetailList = roadpolygonDetailDAL.GetList(LoadID);
                //搜索出当前车辆的GPS数据，前一天的
                for (int index = 0; index < vehicleGpsList.Count; index++)
                {
                    //分析区域进出
                    if (index == 0)
                    {
                        //如果一开始就在道路内，将此时的时间作为进入道路的时间
                        if (!IsPointInLoad(polygonDetailList, vehicleGpsList[index], AnalyseGroupID, ValidDistance))
                        {
                            EnterTime = vehicleGpsList[index].GpsTime;
                            IfOnceInLoad = 1;
                        }
                    }
                    else
                    {
                        if (!IsPointInLoad(polygonDetailList, vehicleGpsList[index - 1], AnalyseGroupID, ValidDistance))
                        {
                            LastIfInLoad = 1;
                        }
                        else
                        {
                            LastIfInLoad = 0;
                        }
                        LastGpsTime = vehicleGpsList[index - 1].GpsTime;
                    }
                    //保存当前道路状态
                    if (!IsPointInLoad(polygonDetailList, vehicleGpsList[index], AnalyseGroupID, ValidDistance))
                    {
                        NowIfInLoad = 1;
                    }
                    else
                    {
                        NowIfInLoad = 0;
                    }
                    //当前gps时间
                    NowGpsTime = vehicleGpsList[index].GpsTime;
                    //如果当前在区域外，上一次也是在区域外
                    if (LastIfInLoad == 0 && NowIfInLoad == 0)
                    {
                        //如果是在道路内开出，必须累加在道路外的时间
                        if (IfOnceInLoad == 1)
                        {
                            AllOutTime = (int)(NowGpsTime - LastGpsTime).TotalSeconds;
                            //如果在道路外的时间超过条件规定的时间，则累计在外时间清零，并且产生一条出入道路记录,记录下出道路的时间
                            if (AllOutTime >= AllowLeaveMaxTime)
                            {
                                AllOutTime = 0;
                                IfOnceInLoad = 0;
                                LeaveTime = NowGpsTime;
                                nowRoadMonitor = new DrivingRoadEnterAndLeaveMonitor();
                                nowRoadMonitor.VehicleID = VehicleID;
                                nowRoadMonitor.AnalyseGroupID = AnalyseGroupID;
                                nowRoadMonitor.GenerateTime = DateTime.Now;
                                nowRoadMonitor.RoadID = LoadID;
                                nowRoadMonitor.EnterTime = EnterTime;
                                nowRoadMonitor.LeaveTime = LeaveTime;
                                nowRoadMonitor.TotalTime = (int)(LeaveTime - EnterTime).TotalSeconds;
                                drivingRoadEnterAndLeaveMonitorDAL.InserDatat(nowRoadMonitor);
                            }
                        }
                    }
                    //如果当前在道路内，上一次在道路外
                    if (LastIfInLoad == 0 && NowIfInLoad == 1)
                    {
                        if (IfOnceInLoad == 0)
                        {
                            IfOnceInLoad = 1;
                            //记录进入道路的时间
                            EnterTime = NowGpsTime;
                        }
                    }
                    //如果当前是最后一条数据
                    if (index + 1 == vehicleGpsList.Count)
                    {
                        //如果当前在道路内，上一次也是在道路内
                        if (LastIfInLoad == 1 && NowIfInLoad == 1)
                        {
                            //插入一条进出道路的数据
                            LeaveTime = NowGpsTime;
                            nowRoadMonitor = new DrivingRoadEnterAndLeaveMonitor();
                            nowRoadMonitor.VehicleID = VehicleID;
                            nowRoadMonitor.AnalyseGroupID = AnalyseGroupID;
                            nowRoadMonitor.GenerateTime = DateTime.Now;
                            nowRoadMonitor.RoadID = LoadID;
                            nowRoadMonitor.EnterTime = EnterTime;
                            nowRoadMonitor.LeaveTime = LeaveTime;
                            nowRoadMonitor.TotalTime = (int)(LeaveTime - EnterTime).TotalSeconds;
                            drivingRoadEnterAndLeaveMonitorDAL.InserDatat(nowRoadMonitor);
                        }
                    }
                }
                this.HaveAnalyseCount++;
                log.Info(string.Format("结束分析车辆ID为{0}的路段分析,当前已分析数{1}##########，分析用时:{2}秒", VehicleID, this.HaveAnalyseCount, (DateTime.Now - AnalyseStartTime).TotalSeconds));

            }
            catch (Exception ex)
            {
                log.Error(string.Format("分析车辆ID为{0}的路段分析时出错,当前分析数目{1},错误原因是###########", VehicleID, this.AnalyseCount), ex);
            }
        }
        //区域分析
        private void AnalyseVehicleArea(string Vehicleinfo, List<VehicleGpsInfo> vehicleGpsList)
        {

            //上次区域状态
            int LastIfInArea = -1;
            //本次区域状态
            int NowIfInArea = -1;
            //上次GPS时间
            DateTime LastGpsTime = DateTime.Now;
            //本次GPS时间
            DateTime NowGpsTime;
            //在区域外的时间
            int AllOutTime = -1;
            //是否曾经在区域内
            int IfOnceInArea = -1;
            //进出区域的时间
            DateTime EnterTime = DateTime.Now;
            DateTime LeaveTime = DateTime.Now;
            //进出区域实体
            DrivingAreaEnterAndLeaveMonitor nowAreaMonitor;
            //区域ID
            int AreaID = -1;
            //离开区域最大时间
            int AllowLeaveMaxTime = -1;
            //分析组ID
            long AnalyseGroupID = -1;

            //车辆ID
            long VehicleID = -1;
            //
            AreaID = int.Parse(Vehicleinfo.Split(',')[2].ToString());
            AllowLeaveMaxTime = int.Parse(Vehicleinfo.Split(',')[3].ToString());
            AnalyseGroupID = long.Parse(Vehicleinfo.Split(',')[4].ToString());
            VehicleID = long.Parse(Vehicleinfo.Split(',')[0].ToString());
            try
            {

                log.Info(string.Format("开始分析车辆ID为{0}的进出区域轨迹###########", VehicleID));
                //记录开始分析的时间
                DateTime AnalyseStartTime = DateTime.Now;
                //根据AreaID获取区域的所有节点ID
                List<ParamPolygonDetail> polygonDetailList = polygonDetailDAL.GetList(AreaID);
                //搜索出当前车辆的GPS数据，前一天的
                for (int index = 0; index < vehicleGpsList.Count; index++)
                {
                    //分析区域进出
                    if (index == 0)
                    {
                        //如果一开始就在区域内，将此时的时间作为进入区域的时间
                        if (IsPointInRegion(polygonDetailList, vehicleGpsList[index]))
                        {
                            EnterTime = vehicleGpsList[index].GpsTime;
                            IfOnceInArea = 1;
                        }
                    }
                    else
                    {
                        if (IsPointInRegion(polygonDetailList, vehicleGpsList[index - 1]))
                        {
                            LastIfInArea = 1;
                        }
                        else
                        {
                            LastIfInArea = 0;
                        }
                        LastGpsTime = vehicleGpsList[index - 1].GpsTime;
                    }
                    //保存当前区域状态
                    if (IsPointInRegion(polygonDetailList, vehicleGpsList[index]))
                    {
                        NowIfInArea = 1;
                    }
                    else
                    {
                        NowIfInArea = 0;
                    }
                    //当前gps时间
                    NowGpsTime = vehicleGpsList[index].GpsTime;
                    //如果当前在区域外，上一次也是在区域外
                    if (LastIfInArea == 0 && NowIfInArea == 0)
                    {
                        //如果是在区域内开出，必须累加在区域外的时间
                        if (IfOnceInArea == 1)
                        {
                            AllOutTime = (int)(NowGpsTime - LastGpsTime).TotalSeconds;
                            //如果在区域外的时间超过条件规定的时间，则累计在外时间清零，并且产生一条出入区域记录,记录下出区域的时间
                            if (AllOutTime >= AllowLeaveMaxTime)
                            {
                                AllOutTime = 0;
                                IfOnceInArea = 0;
                                LeaveTime = NowGpsTime;
                                nowAreaMonitor = new DrivingAreaEnterAndLeaveMonitor();
                                nowAreaMonitor.VehicleID = VehicleID;
                                nowAreaMonitor.AnalyseGroupID = AnalyseGroupID;
                                nowAreaMonitor.GenerateTime = DateTime.Now;
                                nowAreaMonitor.AreaID = AreaID;
                                nowAreaMonitor.EnterTime = EnterTime;
                                nowAreaMonitor.LeaveTime = LeaveTime;
                                nowAreaMonitor.TotalTime = (int)(LeaveTime - EnterTime).TotalSeconds;
                                drivingAreaEnterAndLeaveMonitorDAL.InserDatat(nowAreaMonitor);
                            }
                        }
                    }
                    //如果当前在区域内，上一次在区域外
                    if (LastIfInArea == 0 && NowIfInArea == 1)
                    {
                        if (IfOnceInArea == 0)
                        {
                            IfOnceInArea = 1;
                            //记录下入区域的时间
                            EnterTime = NowGpsTime;
                        }
                    }
                    //如果当前是最后一条数据
                    if (index + 1 == vehicleGpsList.Count)
                    {
                        //如果当前在区域内，上一次也是在区域内
                        if (LastIfInArea == 1 && NowIfInArea == 1)
                        {
                            //插入一条进出区域的数据
                            LeaveTime = NowGpsTime;
                            nowAreaMonitor = new DrivingAreaEnterAndLeaveMonitor();
                            nowAreaMonitor.VehicleID = VehicleID;
                            nowAreaMonitor.AnalyseGroupID = AnalyseGroupID;
                            nowAreaMonitor.GenerateTime = DateTime.Now;
                            nowAreaMonitor.AreaID = AreaID;
                            nowAreaMonitor.EnterTime = EnterTime;
                            nowAreaMonitor.LeaveTime = LeaveTime;
                            nowAreaMonitor.TotalTime = (int)(LeaveTime - EnterTime).TotalSeconds;
                            drivingAreaEnterAndLeaveMonitorDAL.InserDatat(nowAreaMonitor);
                        }
                    }
                }
                this.HaveAnalyseCount++;
                log.Info(string.Format("结束分析车辆ID为{0}的进出区域轨迹,当前已分析数{1}##########，分析用时:{2}秒", VehicleID, this.HaveAnalyseCount, (DateTime.Now - AnalyseStartTime).TotalSeconds));

            }
            catch (Exception ex)
            {
                log.Error(string.Format("分析车辆ID为{0}的进出区域轨迹时出错,当前分析数目{1},错误原因是###########", VehicleID, this.AnalyseCount), ex);
            }
        }
        private void AnalysePerVehicle()
        {
            //循环分析
            while (this.VehicleQueue.Count > 0)
            {

                //队列出列，返回要分析的车辆信息
                long VehicleID = 0;
                long CarId = 0;
                string Vehicleinfo;

                Monitor.Enter(this);

                Vehicleinfo = this.VehicleQueue.Dequeue().ToString();
                Monitor.Exit(this);
                try
                {
                    VehicleID = long.Parse(Vehicleinfo.Split(',')[0].ToString());
                    CarId = long.Parse(Vehicleinfo.Split(',')[1].ToString());
                    //下载gps数据
                    string AreaStartTime = ConfigurationManager.AppSettings["AreaStartTime"].ToString();
                    string AreaEndTime = ConfigurationManager.AppSettings["AreaEndTime"].ToString();
                    string AreaDate = ConfigurationManager.AppSettings["AreaDate"].ToString();
                    DateTime StartTime;
                    DateTime EndTime;
                    if (string.IsNullOrEmpty(AreaDate))
                    {
                        StartTime = DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd ") + AreaStartTime + ":00");
                        EndTime = DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd ") + AreaEndTime + ":00");
                    }
                    else
                    {
                        StartTime = DateTime.Parse(DateTime.Parse(AreaDate).ToString("yyyy-MM-dd ") + AreaStartTime + ":00");
                        EndTime = DateTime.Parse(DateTime.Parse(AreaDate).ToString("yyyy-MM-dd ") + AreaEndTime + ":00");
                    }
                    log.Info(string.Format("开始下载车辆ID为{0}的轨迹数据###########", VehicleID));
                    //记录下开始下载的时间
                    DateTime DownLoadStartTime = DateTime.Now;
                    List<VehicleGpsInfo> vehicleGpsList = vehicleGpsInfoDAL.GetList(CarId, StartTime, EndTime);

                    this.AnalyseCount++;
                    log.Info(string.Format("完成下载车辆ID为{0}的轨迹数据###########,下载用时:{1}秒", VehicleID, (DateTime.Now - DownLoadStartTime).TotalSeconds));
                    if (vehicleGpsList == null)
                    {
                        this.HaveAnalyseCount++;
                        log.Info(string.Format("车辆ID为{0}一共下载0条轨迹数据,当前分析数,{1}", VehicleID, this.HaveAnalyseCount));
                        continue;
                    }
                    log.Info(string.Format("车辆ID为{0}一共下载{1}条轨迹数据", VehicleID, vehicleGpsList.Count));
                    //如果区域分析条件里面有包含当前车辆的分析
                    string vehicleInfo = string.Empty;
                    if (this.AreaCache.Keys.Contains(VehicleID.ToString()))
                    {
                        vehicleInfo = this.AreaCache[VehicleID.ToString()].ToString();
                        AnalyseVehicleArea(vehicleInfo, vehicleGpsList);
                    }
                    //如果路段分析条件里面有包含当前车辆的分析
                    if (this.LoadCache.Keys.Contains(VehicleID.ToString()))
                    {
                        vehicleInfo = this.LoadCache[VehicleID.ToString()].ToString();
                        AnalyseVehicleLoad(vehicleInfo, vehicleGpsList);
                    }
                    //}
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("分析车辆ID为{0}的时候出错,出错原因是:", VehicleID), ex);
                }
            }

        }


    }
}
