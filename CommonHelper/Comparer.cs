using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonHelper
{
   public class Comparer
    {

        /// <summary>
        /// 判断舱位是否一样
        /// </summary>
        /// <param name="a">从页面查出的舱位</param>
        /// <param name="b">导入的舱位</param>
        /// <returns></returns>
        public static bool IsSameCabins(string a, string b)
        {
            bool r = false;
            try
            {
                string a1 = a.Replace(",", "/").Trim('/');
                string b1 = b.Trim('/');
                string[] ad = a1.Split('/');
                string[] bd = b1.Split('/');
                if (ad.Length != bd.Length)
                { return false; }
                else
                {
                    foreach(string i in ad)
                    {
                        if (!bd.Contains(i))
                            return false;
                    }
                }
            }
            catch (Exception)
            {
                r = false;
                throw;
            }
            return r;
        }


        /// <summary>
        /// 比较日期
        /// </summary>
        /// <param name="sdate">平台政策开始日期</param>
        /// <param name="edate">平台政策结束日期</param>
        /// <param name="Sdate">导入政策开始日期</param>
        /// <param name="Edate">导入政策结束日期</param>
        /// <returns></returns>
        public static bool IsSuitDate(DateTime sdate, DateTime edate, DateTime Sdate, DateTime Edate)
        {
            bool r = false;
            if (sdate <= DateTime.Now)
            {
                if (edate == Edate)
                {
                    r = true;
                }
            }
            else
            {
                if (sdate == Sdate && edate == Edate)
                { r = true; }
            }
            return r;
        }

        /// <summary>
        /// 对比航班号,其中一个为空时返回true;都不为空时,前者若包含后者,则返回true,否则,返回false
        /// </summary>
        /// <param name="webflightno"></param>
        /// <param name="flightno"></param>
        /// <returns></returns>
        public static bool IsSuitFlightNo(string webflightno, string flightno)
        {
            string fno = "";
            if (flightno.Length > 4)
            {
                fno = flightno.Substring(2, 4);
            }
            else
            {
                fno = flightno;
            }
            if (string.IsNullOrEmpty(flightno) || string.IsNullOrEmpty(webflightno))
            {
                return true;
            }
            else
            {
                try
                {
                    webflightno.Contains(fno);
                }
                catch (Exception)
                {
                    return false;
                }
                return false;
            }
        }

    }
}
