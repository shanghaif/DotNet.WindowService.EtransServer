using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ET.WinService.Core.Extension
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 把小于1800/01/01日期转化成1800/01/01
        /// </summary>
        /// <param name="InputDateTime">原日期值</param>
        /// <returns></returns>
        public static DateTime ToSafeDateTime(this  System.DateTime InputDateTime)
        {
            string minDate = "1800/01/01";
            if (InputDateTime < Convert.ToDateTime(minDate))
            {
                InputDateTime = Convert.ToDateTime(minDate);
            }
            return InputDateTime;

        }

        public static long DateDiff(this DateTime startDate, DateInterval Interval, DateTime endDate)
        {
            long lngDateDiffValue = 0;
            System.TimeSpan TS = new System.TimeSpan(endDate.Ticks - startDate.Ticks);
            switch (Interval)
            {
                case DateInterval.Second:
                    lngDateDiffValue = (long)TS.TotalSeconds;
                    break;
                case DateInterval.Minute:
                    lngDateDiffValue = (long)TS.TotalMinutes;
                    break;
                case DateInterval.Hour:
                    lngDateDiffValue = (long)TS.TotalHours;
                    break;
                case DateInterval.Day:
                    lngDateDiffValue = (long)TS.Days;
                    break;
                case DateInterval.Week:
                    lngDateDiffValue = (long)(TS.Days / 7);
                    break;
                case DateInterval.Month:
                    lngDateDiffValue = (long)(TS.Days / 30);
                    break;
                case DateInterval.Quarter:
                    lngDateDiffValue = (long)((TS.Days / 30) / 3);
                    break;
                case DateInterval.Year:
                    lngDateDiffValue = (long)(TS.Days / 365);
                    break;
            }
            return (lngDateDiffValue);
        }
    }


    public enum DateInterval
    {
        Second, Minute, Hour, Day, Week, Month, Quarter, Year
    }

}
