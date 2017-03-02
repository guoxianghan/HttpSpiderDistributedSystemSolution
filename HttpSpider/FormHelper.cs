using System;
using System.Collections.Generic;
using System.Text;

namespace HttpSpider
{
    /// <summary>
    /// 获取表单中的隐藏域的键值对
    /// </summary>
    public static class FormHelper
    {
        /// <summary>
        /// 获取Form Post集合
        /// </summary>
        /// <param name="html">Html字符串</param>
        /// <param name="action">Form Action 开始字符串</param>
        /// <param name="urlEncodeDictionary">需要Url编码的集合</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetFormPostData(string html, string action, Dictionary<string, string> urlEncodeDictionary,out string err)
        {
            err = "";
            Dictionary<string, string> PostDict = new Dictionary<string, string>();
            try
            {
                HtmlAgilityPack.HtmlNode.ElementsFlags.Remove("form");//必须添加此句 
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);
                HtmlAgilityPack.HtmlNodeCollection PostItemNodeList = doc.DocumentNode.SelectNodes("//form[starts-with(@action, '" + action + "') and @method='post']");
                if (PostItemNodeList == null)
                {
                    PostItemNodeList = doc.DocumentNode.SelectNodes("//form[starts-with(@action, '" + action + "') and @method='POST']");
                }
                if (PostItemNodeList == null)
                {
                    return PostDict;
                }
                else
                {
                    foreach (HtmlAgilityPack.HtmlNode PostItemNode in PostItemNodeList)
                    {
                        string ItemDispatchUrl = PostItemNode.Attributes["action"].Value;
                        HtmlAgilityPack.HtmlNodeCollection InputTypeNodeList = PostItemNode.SelectNodes(".//input[@type='hidden' and @name and @value]");
                        foreach (HtmlAgilityPack.HtmlNode hidenode in InputTypeNodeList)
                        {
                            string key = hidenode.Attributes["name"].Value;
                            string value = hidenode.Attributes["value"].Value;
                            if (urlEncodeDictionary.ContainsKey(key))
                            {
                                value = System.Web.HttpUtility.UrlEncode(value);
                            }
                            PostDict.Add(key, value);
                        }
                    }
                }
                return PostDict;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return PostDict;
            }
        }
        /// <summary>
        /// 获取Form Post数据
        /// </summary>
        /// <param name="html">Html字符串</param>
        /// <param name="action">Form Action 开始字符串</param>
        /// <param name="urlEncodeDictionary">需要Url编码的集合</param>
        /// <returns></returns>
        public static string GetPostData(string html, string action, Dictionary<string, string> urlEncodeDictionary,out string err)
        {
            err = "";
            try
            {
                Dictionary<string, string> data_dic = GetFormPostData(html, action, urlEncodeDictionary,out err);
                Dictionary<string, string>.KeyCollection keys = data_dic.Keys;
                string data = "";
                int index = 0;
                foreach (string key in keys)
                {
                    if (data == "")
                    {
                        data = key + "=" + data_dic[key].ToString() + "";
                    }
                    else
                    {
                        data = data + "&" + key + "=" + data_dic[key].ToString() + "";
                    }
                    index++;
                }
                return data;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return "";
            }
        }

        public static Dictionary<string, string> FormParams(HtmlAgilityPack.HtmlNode node)
        {
            Dictionary<string, string> dicpara = new Dictionary<string, string>();
            HtmlAgilityPack.HtmlNodeCollection InputTypeNodeList = node.SelectNodes(".//input[@type='hidden' and @name and @value]");
            foreach (HtmlAgilityPack.HtmlNode hidenode in InputTypeNodeList)
            {
                string key = hidenode.Attributes["name"].Value;
                string value = hidenode.Attributes["value"].Value;
                dicpara.Add(key, value);
            }
            return dicpara;
        }
    }
}
