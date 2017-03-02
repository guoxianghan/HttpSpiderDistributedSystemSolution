using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonHelper
{
    public class UnixTimeStamp
    {
        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public static DateTime GetTime(string timeStamp, bool isMsec = false)
        {
            if (isMsec)
            {//移除后三位
                Regex x = new Regex("[\\d+]{3}$");
                timeStamp = x.Replace(timeStamp, "");
            }
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
        public static DateTime ConvertJSTime(long milliTime)
        {
            long timeTricks = new DateTime(1970, 1, 1).Ticks + milliTime * 10000 + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours * 3600 * (long)10000000;
            return new DateTime(timeTricks);
        }

        ///<summary>
        /// 将c# DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>double</returns>
        public static string ConvertDateTimeIntd(System.DateTime time)
        {
            double intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            intResult = (time - startTime).TotalSeconds;
            string d = intResult.ToString("0.000").Replace(".", "").Substring(0, 13);
            return d;
        }

        public static string ConvertDateTimeUTC(DateTime dt)
        { 
            string t = dt.ToString("R").Replace(",", "");
            string[] d = t.Split(' ');
            string a = d[1];
            string b = d[2];
            d[1] = b;
            d[2] = a;
            StringBuilder sb = new StringBuilder();
            foreach (var i in d)
            {
                sb.Append(i + " ");
            }
            //sb.Append("0800 (中国标准时间)");
            string s = sb.ToString()+ "0800 (中国标准时间)";
            //Thu Dec 10 2015 08:43:09 GMT 0800 (中国标准时间)
            return s;
        }
    }
}
