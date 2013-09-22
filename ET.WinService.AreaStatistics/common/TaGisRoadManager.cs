using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.AreaStatistics.common
{
    public class TaGisRoadManager
    {
        private List<GisRoadObject> RoadList = new List<GisRoadObject>();
        /// <summary>
        /// 获取路段信息
        /// </summary>
        /// <param name="_road_info"></param>
        public void addRoad(GisRoadInfo _road_info, List<TGis_Point> TGis_PointList)
        {
            GisRoadObject road_object = new GisRoadObject();
            foreach (TGis_Point perPoint in TGis_PointList)
            {
                road_object.AddNote(perPoint);
            }
            road_object.Info = _road_info;
            if (!RoadList.Contains(road_object))
            {
                RoadList.Add(road_object);
            }

        }
        /// <summary>
        /// 根据分析组分析点到附近的路
        /// </summary>
        /// <param name="_gis_point"></param>
        /// <param name="_group_id">分析组编号</param>
        /// <param name="_road_id">路段编号</param>
        /// <param name="_radius">偏离距离</param>
        /// <returns></returns>
        public GisRoadResult AnalyseRoadsDivergedByGroupID(TGis_Point _gis_point, long _group_id, int _road_id, int _radius)
        {
            int i = 0;
            GisRoadObject road_object = new GisRoadObject();
            GisRoadResult road_result = new GisRoadResult();
            double distanct_map = 0;
            double min_distance = 0;
            bool isOnTheRoad = false;
            double delta_x = 0;
            double delta_y = 0;
            TGIS_Extent delta_extent = new TGIS_Extent();
            road_result.RoadID = -1;
            road_result.IsDiverged = false;
            road_result.NearestDiverged = 0;
            isOnTheRoad = false;
            // 遍历该全部道路
            if (_road_id != -1)
            {
                for (i = 0; i < RoadList.Count(); i++)
                {
                    road_object = RoadList[i];
                    if (road_object.Info.RoadID!=_road_id)
                    {
                        continue;
                    }
                    // 找线和点距离
                    distanct_map = road_object.DistanceToPoint(_gis_point);
                    min_distance = Math.Round(distanct_map);
                    // 如算出来的距离大于指定的半径
                    if (distanct_map > _radius)
                    {
                        if ((road_result.NearestDiverged == 0) ||
                        (min_distance < road_result.NearestDiverged))
                        {
                            road_result.RoadID = road_object.Info.RoadID;
                            road_result.IsDiverged = true;
                            road_result.NearestDiverged = int.Parse(min_distance.ToString());
                        }
                        continue;
                    }
                    road_result.RoadID = road_object.Info.RoadID;
                    road_result.IsDiverged = false;
                    isOnTheRoad = true;
                    break;
                }
            }
            if (!isOnTheRoad)
            {
                for (i = 0; i < RoadList.Count() - 1; i++)
                {
                    road_object = RoadList[i];
                    if ((road_object.Info.AnalyserGroupID != _group_id)
                        || (_road_id != -1) || (road_object.Info.RoadID == _road_id))
                    {
                        continue;
                    }
                    // 取路外框
                    delta_x = _radius * 1 / 50000;
                    delta_y = _radius * 1 / 108000;
                    delta_extent.XMin = delta_extent.XMin - delta_x;
                    delta_extent.XMax = delta_extent.XMax + delta_x;
                    delta_extent.YMin = delta_extent.YMin - delta_x;
                    delta_extent.YMax = delta_extent.YMax + delta_x;
                    // 不在框内，不用算
                    if ((_gis_point.X < delta_extent.XMin)
                    || (_gis_point.X > delta_extent.XMax)
                    || (_gis_point.Y < delta_extent.YMin)
                    || (_gis_point.Y > delta_extent.YMax))
                    {
                        continue;
                    }
                    // 找线和点距离
                   distanct_map          = road_object.DistanceToPoint(_gis_point);

                   min_distance          = Math.Round(distanct_map);

                  // 如算出来的距离大于指定的半径
                  if (distanct_map > _radius)
                  {
                    if ((road_result.NearestDiverged == 0)||
                      (min_distance < road_result.NearestDiverged))
                     {
                      road_result.RoadID = road_object.Info.RoadID;
                      road_result.IsDiverged = true;
                      road_result.NearestDiverged = int.Parse(min_distance.ToString());
                     }
                    continue;
                  }

                  road_result.RoadID = road_object.Info.RoadID;
                  road_result.IsDiverged = false;
                  break;
                }
            }
            return road_result;
           
        }

    }
}
