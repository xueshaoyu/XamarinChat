using System;
using System.Collections.Generic;
using System.Text;

namespace Chat.Model
{
    /// <summary>
    /// 时间扩展类
    /// </summary>
    public static class Extentions
    {
        public static int ToTimeStamp(this System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
        public static DateTime ToDateTime(this int timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            // timeStamp = timeStamp * 10000000;
            TimeSpan toNow = new TimeSpan(0, 0, timeStamp);
            return dtStart.Add(toNow);
        }

    }
}
