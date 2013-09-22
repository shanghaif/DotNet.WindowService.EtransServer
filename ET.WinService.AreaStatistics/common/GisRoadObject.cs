using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.AreaStatistics.common
{
    public class GisRoadInfo
    {
      
        //路段编号
        public int RoadID
        {
            get;
            set;
        }
        //所属用户组编号
        public int UserGroupID
        {
            get;
            set;
        }
        //所属分析组编号
        public long AnalyserGroupID
        {
            get;
            set;
        }
        //道路名称
        public string RoadName
        {
            get;
            set;
        }
        //道路描述
        public string Description
        {
            get;
            set;
        }
    }
    public class GisRoadResult
    {
        //路段编号
        public int RoadID;
        //是否偏离
        public bool IsDiverged;
        //偏离最近的值
        public int NearestDiverged;
    }
    class GisRoadObject
    {
        private GisRoadInfo gisroadinfo = new GisRoadInfo();
        private List<GisRoadInfo> lgisroadinfo= new List<GisRoadInfo>();
        private List<TGis_Point> lgispoint= new List<TGis_Point>();
        private TGIS_Extent roadextent;
        private TGis_Point getPoint(int _index)
        {
            TGis_Point p_node= new TGis_Point();
            p_node = null;
            if (_index>-1 & _index<lgisroadinfo.Count())
            {
                return p_node;
            }
            p_node = lgispoint[_index];
            return p_node;
            
        }
        public GisRoadInfo Info
        {
            get
            {
                return gisroadinfo;
            }
            set
            {
                gisroadinfo = value;
            }

        }
        public TGIS_Extent RoadExtent
        {
            get
            {
                return roadextent;
            }
        }
        public List<TGis_Point> NodeList
        {
            get
            {
                return lgispoint;
            }
        }
        public TGIS_Extent GisExtent(double _x_min, double _y_min, double _x_max, double _y_max)
        {
            TGIS_Extent gisextent = new TGIS_Extent();
            gisextent.XMin = _x_min;
            gisextent.YMin = _y_min;
            gisextent.XMax = _x_max;
            gisextent.YMax = _y_max;
            return gisextent;
        }
        public double GisDMin(double _value1, double _value2)
        {
            if (_value1 < _value2)
            {
                return _value1;
            }
            else
            {
                return _value2;
            }
        }
        public double GisDMax(double _value1, double _value2)
        {
            if (_value1 > _value2)
            {
                return _value1;
            }
            else
            {
                return _value2;
            }

        }
        public double GisPoint2Point(TGis_Point _point_1, TGis_Point _point_2)
        {
            double d = 0;
            double x = _point_1.X - _point_2.X;
            double y = _point_1.Y - _point_2.Y; 
            d = Math.Sqrt(x*x + y*y);
            return d;
            
        }
        public double GisLine2Point(TGis_Point _line_1, TGis_Point _line_2, TGis_Point _gis_point)
        {
            double s = 0;
            double r = 0;
            double m = 0;
            double m2 = 0;
            double d1 = 0;
            double d2 = 0;
            double res = 0;
            if ((_line_1.X == _line_2.X) & (_line_1.Y == _line_2.Y))
            {
                res = GisPoint2Point(_line_1, _gis_point);
            }
            else
            {
                m2 = (_line_2.X - _line_1.X) * (_line_2.X - _line_1.X) + (_line_2.Y - _line_1.Y) * (_line_2.Y - _line_1.Y);
                r = ((_line_1.Y - _gis_point.Y) * (_line_1.Y - _line_2.Y) - (_line_1.X - _gis_point.X) * (_line_2.X - _line_1.X)) / m2;
                if ((r >= 0) & (r <= 1))
                {
                    s = ((_line_1.Y - _gis_point.Y) * (_line_2.X -_line_1.X) -(_line_1.X - _gis_point.X) * (_line_2.Y -_line_1.Y)) / m2;
                    m = Math.Sqrt(m2);
                    res = Math.Abs(s*m);
                }
                else
                {
                    d1 = GisPoint2Point(_line_1, _gis_point);
                    d2 = GisPoint2Point(_line_2, _gis_point);
                    res = GisDMin(d1, d2);
                }
            }
            return res;

        }
        public double GetDistance(double lon1, double lon2, double lat1, double lat2)
        {
            const int a = 6378245;
            const double f = 1 / 298.3;
            const double b = a * (1 - f);
            int i = 0;
            double w = 0;
            double tanU1 = 0;
            double U1 = 0;
            double sinU1 = 0;
            double cosU1 = 0;
            double tanU2 = 0;
            double U2 = 0;
            double sinU2 = 0;
            double cosU2 = 0;
            double y = 0;
            double sin2_a = 0;
            double cos_a = 0;
            double tan_a = 0;
            double _a = 0;
            double sin_a = 0;
            double sina = 0;
            double cosa = 0;
            double cos2_am = 0;
            double _u2 = 0;
            double C = 0;
            double BA = 0;
            double BB = 0;
            double Delta_a = 0;
            double s = 0;
            lat1 = lat1 * (Math.PI / 180);
            lat2 = lat2 * (Math.PI / 180);
            lon1 = lon1 * (Math.PI / 180);
            lon2 = lon2 * (Math.PI / 180);
            if ((lat1 == lat2) & (lon1 == lon2))
            {
                return 0;
            }
            w = lon2 - lon1;
            U1 = Math.Atan(tanU1);
            sinU1 = Math.Sin(U1);
            cosU1 = Math.Cos(U1);
            tanU2 = (1 - f) * Math.Tan(lat2);
            U2 = Math.Atan(tanU2);
            sinU2 = Math.Sin(U2);
            cosU2 = Math.Cos(U2);
            y = w;
            for (i = 0; i < 99; i++)
            {
                sin2_a = cosU2 * Math.Sin(y) * cosU2 * Math.Sin(y) + (cosU1 * sinU2 - sinU1 * cosU2 * Math.Cos(y)) * (cosU1 * sinU2 - sinU1 * cosU2 * Math.Cos(y));
                cos_a = sinU1 * sinU2 + cosU1 * cosU2 * Math.Cos(y);
                tan_a = Math.Pow(sin2_a, 0.5);
                _a = Math.Atan(tan_a);
                sina = cosU1 * cosU2 * Math.Sin(y) / sin_a;
                cosa = Math.Cos(Math.Asin(sina));
                cos2_am = cos_a - 2 * sinU1 * sinU2 / (cosa * cosa);
                C = (f / 16) * cosa * cosa * (4 + f * (4 - 3 * cosa * cosa));
                y = w + (1 - C) * f * sina*(_a + C * sin_a * (cos2_am + C * cos_a * (-1 + 2 * cos2_am * cos2_am)));

            }
            _u2 = cosa * cosa * ((a / b) * (a / b) - 1);
            BA = 1 + (_u2 / 16384) * (4096 + _u2 * (-768 + _u2 * (320 - 175 * _u2)));
            BB = (_u2 / 1024) * (256 + _u2 * (-128 + _u2 * (74 - 47 * _u2)));
            Delta_a = BB * sin_a * (cos2_am + (BB / 4) * (cos_a * (-1 + 2 * cos2_am * cos2_am) - (BB / 6) * cos2_am * (-3 + 4 * sin2_a) * (-3 + 4 * cos2_am * cos2_am)));    
            s = b * BA * (_a - Delta_a);

            return s;
        }
        public TGis_Point GisPointOnLine(TGis_Point _line_1, TGis_Point _line_2, TGis_Point _gis_point)
        {
            TGis_Point project_point = new TGis_Point();
            double r = 0;
            double m2 = 0;
            double d1 = 0;
            double d2 = 0;
            if ((_line_1.X == _line_2.X) & (_line_1.Y == _line_2.Y))
            {
                project_point = _line_1;
            }
            else
            {
                m2 = (_line_2.X - _line_1.X) * (_line_2.X - _line_1.X) + (_line_2.Y - _line_1.Y) * (_line_2.Y - _line_1.Y);
                r = ((_line_1.Y - _gis_point.Y) * (_line_1.Y - _line_2.Y) - (_line_1.X - _gis_point.X) * (_line_2.X - _line_1.X)) / m2;
                if ((r >= 0) & (r <= 1))
                {
                    project_point.X = _line_1.X + r * (_line_2.X - _line_1.X);
                    project_point.Y = _line_1.Y + r * (_line_2.Y - _line_1.Y);
                }
                else
                {
                    d1 = GisPoint2Point(_line_1, _gis_point);
                    d2 = GisPoint2Point(_line_2, _gis_point);
                }
                if (d1 < d2)
                {
                    project_point = _line_1;
                }
                else
                {
                    project_point = _line_2;
                }

            }
            return project_point;

 
        }
        /// <summary>
        /// 获取路段点信息
        /// </summary>
        /// <param name="_node"></param>
        public void AddNote(TGis_Point _node)
        {
            if (!lgispoint.Contains(_node))
            {
                lgispoint.Add(_node);
                if (lgispoint.Count() == 1)
                {
                    roadextent = GisExtent(_node.X, _node.Y, _node.X, _node.Y);
                }
                else
                {
                    roadextent.XMin = GisDMin(roadextent.XMin, _node.X);
                }
            }
        }
        /// <summary>
        /// 点到线的距离
        /// </summary>
        /// <param name="_gis_point">给定的点</param>
        /// <returns>距离</returns>
        public double DistanceToPoint(TGis_Point _gis_point)
        {
            int point_no = 0;
            int next_pt = 0;
            int points_count = 0;
            double dist = 0;
            double tmp = 0;
            double res = 0;
            TGis_Point line_a = new TGis_Point();
            TGis_Point line_b = new TGis_Point();
            TGis_Point project = new TGis_Point();
            project = _gis_point;
            dist = 2139062143;
            points_count = lgispoint.Count();
            if (points_count == 1)
            {
                dist = GisPoint2Point(getPoint(0), _gis_point);
                project = getPoint(0);
            }
            else
            {
                for (point_no = 0; point_no < points_count - 2;point_no++)
                {
                    next_pt = point_no + 1;
                    line_a = getPoint(point_no);
                    line_b = getPoint(next_pt);
                    tmp = GisLine2Point(line_a, line_b, _gis_point);
                    if (tmp < dist)
                    {
                        dist = tmp;
                        project = GisPointOnLine(line_a, line_b, _gis_point);
                    }
                }
            }
            res = GetDistance(_gis_point.X, project.X, _gis_point.Y, project.Y);
            return res;
        }

    }
}
