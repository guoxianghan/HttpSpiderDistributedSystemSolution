using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections;
using System.IO;

namespace CommonHelper
{
    public class CookieHelper
    {
        /// 遍历CookieContainer
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public static List<Cookie> GetAllCookies(CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();
            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }
            return lstCookies;
        }
        /// <summary>
        /// 添加Cookie 到 CookieContainer 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="cc"></param>
        /// <param name="cookie">k0=v0;k1=v1;k2=v2;.....</param>
        /// <returns></returns>
        public static void addCookieToContainer(string cookie, string domain, ref CookieContainer cc)
        {
            string[] tempCookies = cookie.Split(';');
            string tempCookie = null;
            int Equallength = 0;//  =的位置
            string cookieKey = null;
            string cookieValue = null;
            //qg.gome.com.cn  cookie
            for (int i = 0; i < tempCookies.Length; i++)
            {
                if (!string.IsNullOrEmpty(tempCookies[i]))
                {
                    tempCookie = tempCookies[i];
                    Equallength = tempCookie.IndexOf("=");
                    if (Equallength != -1)       //有可能cookie 无=，就直接一个cookiename；比如:a=3;ck;abc=;
                    {
                        cookieKey = tempCookie.Substring(0, Equallength).Trim();
                        //cookie=

                        if (Equallength == tempCookie.Length - 1)    //这种是等号后面无值，如：abc=;
                        {
                            cookieValue = "";
                        }
                        else
                        {
                            cookieValue = tempCookie.Substring(Equallength + 1, tempCookie.Length - Equallength - 1).Trim();
                        }
                    }
                    else
                    {
                        cookieKey = tempCookie.Trim();
                        cookieValue = "";
                    }
                    cc.Add(new Cookie(cookieKey, cookieValue, "", domain));
                }
            }
        }

        public static string CookieValue(string key, CookieContainer cc)
        {
            List<Cookie> list = GetAllCookies(cc);
            Cookie coo = list.FirstOrDefault(i => i.Name == key);
            if (coo == null)
                return "";
            else return coo.Value;
        }

        public static CookieContainer CreateCookieContriner(List<Cookie> list, string domain = "")
        {
            string dom = string.IsNullOrEmpty(domain) ? list[0].Domain : domain;
            CookieContainer cc = new CookieContainer();
            foreach (Cookie i in list)
            {
                cc.Add(new Cookie() { Name = i.Name, Value = i.Value, Domain = dom });
            }
            return cc;
        }

        public static byte[] PostByte(string fullpath, Encoding encode, string header, string footer)
        {
            byte[] bytes = File.ReadAllBytes(fullpath);
            byte[] byteheader = encode.GetBytes(header);
            byte[] bytefooter = encode.GetBytes(footer);
            byte[] all = new byte[byteheader.Length + bytes.Length + bytefooter.Length];
            byteheader.CopyTo(all, 0);
            bytes.CopyTo(all, byteheader.Length);
            bytefooter.CopyTo(all, byteheader.Length + bytes.Length);
            return all;
        }
        public static CookieCollection CookieToCollection(CookieContainer cc)
        {
            List<Cookie> c = GetAllCookies(cc);
            CookieCollection list = new CookieCollection();
            foreach (var i in c)
            {
                list.Add(i);
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public static string CookieContainerToString(CookieContainer cc)
        {
            if (cc == null)
                return "";
            StringBuilder sb = new StringBuilder();
            List<Cookie> lstCookies = new List<Cookie>();
            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies)
                    {
                        //var h = lstCookies.FirstOrDefault(x => x.Name == c.Name);
                        //if (h != null)
                        //    continue;
                        lstCookies.Add(c);
                        sb.Append("NAME:" + c.Name + ";VALUE:" + c.Value + ";DOMAIN:" + c.Domain + "|");
                    }
            }
            return sb.ToString();
        }
        public static CookieContainer StringToCookie(string cookie, string doman = "")
        {
            CookieContainer cc = null;
            string[] coarr = cookie.TrimEnd('|').Split('|');
            string[] f = null;

            Cookie c = null;
            cc = new CookieContainer();
            foreach (string i in coarr)
            {
                try
                {
                    f = i.Split(';');
                    c = new Cookie();
                    c.Name = f[0].Split(':')[1];
                    c.Value = f[1].Split(':')[1];
                    if (f.Length >= 3)
                        c.Domain = f[2].Split(':')[1];
                    else
                    {
                        if (!string.IsNullOrEmpty(doman))
                            c.Domain = doman;
                    }
                    cc.Add(c);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            return cc;
        }
    }
}
