using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.AreaStatistics.common
{
    /// <summary>
    /// 描  述：判断点是否在区域内的工具类，x是经度，y是纬度
    /// 作  者：陈炯炯 (chenjiongjiong@e-trans.com.cn)
    /// 时  间：2013年1月15日
    /// 修  改：
    /// 原  因：
    /// </summary>
    /// <returns></returns>
    public class TGis_Point
    {
        public TGis_Point(double X,double Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public TGis_Point()
        {
           
        }
        private double x;

        public double X
        {
            get { return x; }
            set { x = value; }
        }
        private double y;

        public double Y
        {
            get { return y; }
            set { y = value; }
        }
       
    }
    public class TGIS_Extent
    {
        private double xmin;

        public double XMin
        {
            get { return xmin; }
            set { xmin = value; }
        }
        private double ymin;

        public double YMin
        {
            get { return ymin; }
            set { ymin = value; }
        }
        private double xmax;

        public double XMax
        {
            get { return xmax; }
            set { xmax = value; }
        }
        private double ymax;

        public double YMax
        {
            get { return ymax; }
            set { ymax = value; }
        }
      
    }

    public class GisPolyGon
    {
        public TGIS_Extent polyextent = null;
        //public TGIS_Extent PolyExtent
        //{
        //    get
        //    {
        //        return polyextent;
        //    }
        //    set
        //    {
        //        value = polyextent;
        //    }
        //}
        public List<TGis_Point> LGis_Point;
        /// <summary>
        /// 判断是否在区域内
        /// </summary>
        /// <param name="_gis_point"></param>
        /// <returns></returns>
        public bool IsPointInRegion(TGis_Point _gis_point)
        {
           
            if (LGis_Point.Count() == 0)
            {
                return false;
            }
            // 点在多边形 左、上、右、下 时，不用再看
            if(polyextent.XMin>_gis_point.X || polyextent.YMin>_gis_point.Y
            || polyextent.XMax<_gis_point.X ||polyextent.YMax<_gis_point.Y)
            {
                return false;
            }

            ;
            int points_count ;
            int next_pt ;
            int npar ;
            points_count  = LGis_Point.Count();
            next_pt       = points_count - 1;
            npar = 0;
            TGis_Point line_a = new TGis_Point();
            TGis_Point line_b = new TGis_Point();
            for (int point_no = 0; point_no < points_count - 1; point_no++)
            {
                // 基于多边形的每一条边，依据 奇偶算法 对点进行测试
                line_a = LGis_Point[point_no];
                line_b = LGis_Point[next_pt];
                next_pt = point_no;
                 
                try
                {
                    if (((line_a.Y <= _gis_point.Y) && (_gis_point.Y < line_b.Y) ||
                    (line_b.Y <= _gis_point.Y) && (_gis_point.Y < line_a.Y)) &&
                    ((_gis_point.X < (line_b.X - line_a.X) * (_gis_point.Y - line_a.Y) /
                    (line_b.Y - line_a.Y) + line_a.X)))
                    {
                        npar++;
                    }

                }
                catch
                {

                }

            }
            // 偶数，点在 内部 的一边
            if ((npar %2)!=1)
            {
                return false;
            }
            return true;
        }
    }
}
