using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
namespace CommonHelper
{
    public static class HtmlDocumentHelper
    {
        public static string RFID(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode tbNode = doc.GetElementbyId("tablemoveeffect");
            HtmlNodeCollection trNodes = tbNode.SelectNodes("tr");
            if (trNodes.Count == 0)
                return null;
            HtmlNode trNode = trNodes[1].SelectNodes("td")[0];
            if (trNode == null)
                return null;
            return trNode.InnerText.Trim();
        }
        public static RefundInfo GetRefundInfo(string html, string ticketno)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            string s = Regex.Match(html, "v_refundinfo(.*?);").Groups[1].Value.TrimStart('=', ' ');
            List<RefundInfo> ddd = JsonConvert.DeserializeObject<List<RefundInfo>>(s);
            doc.LoadHtml(html);
            HtmlNodeCollection tb_Nodes = doc.DocumentNode.SelectNodes("//table[@class=\"tableinfo1\"]");
            HtmlNode t_tbNode = tb_Nodes[0];
            HtmlNodeCollection trs = t_tbNode.SelectNodes("tr");
            HtmlNodeCollection tds = null;
            RefundInfo tar = null;
            foreach (var i in trs)
            {
                tds = i.SelectNodes("td");
                if (tds != null && tds.Count >= 8 && ticketno == tds[4].InnerText.Trim())
                {//取状态
                    //ca tds[0].ChildNodes[1].Attributes[3]
                    string refundradio = tds[0].ChildNodes[1].Attributes[3].Value;
                    tar = ddd.FirstOrDefault(a => a.refundid == refundradio);
                    if (tar != null)
                    {
                        tar.TuiKuanState = tds[7].InnerText;
                        tar.HangTuiMoney = Convert.ToDecimal(tds[9].InnerText.Trim());
                    }
                    break;
                }
                #region MyRegion
                //this._Target.TuiKuanState = tds[7].InnerText.Trim();
                //this._Target.HangTuiMoney = Convert.ToDecimal(jine);
                //if (tar != null)
                //{
                //    _Target.OnceCheckDate = Convert.ToDateTime(tar.firstdate);
                //    _Target.OnceCheckMemo = tar.firstremark;
                //    _Target.TwiceCheckDate = Convert.ToDateTime(tar.seconddate);
                //    _Target.TwiceCheckMemo = tar.secondremark;
                //}
                //_Target.IsSuccess = 1; 
                #endregion 

            }
            return tar;
        }
    }
}
