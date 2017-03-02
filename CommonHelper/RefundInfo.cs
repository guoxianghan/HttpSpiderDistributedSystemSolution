using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonHelper
{
   public class RefundInfo
    {
        public string refundid { get; set; }
        public string applyid { get; set; }
        public string pplydate { get; set; }
        public string applyremark { get; set; }
        public string applyreason { get; set; }
        public string firstid { get; set; }
        public string firstdate { get; set; }
        public string firstremark { get; set; }
        public string fauditchange { get; set; }
        public string sauditchange { get; set; }
        public string secondid { get; set; }
        public string seconddate { get; set; }
        public string secondremark { get; set; }
        public string thirdid { get; set; }
        public string thirdremark { get; set; }
        public string thirddate { get; set; }
        public string rstatus { get; set; }
        public string issameday { get; set; }
        public string suppapplicant { get; set; }
        public string suppapplydate { get; set; }
        public string suppamount { get; set; }
        public string suppstatus { get; set; }
        public string suppreason { get; set; }
        public string suppremark { get; set; }
        public string suppapplyfirstremark { get; set; }
        public string suppapplyfirsttime { get; set; }
        public string suppapplysecondremark { get; set; }
        public string suppapplysecondtime { get; set; }
        public string suppapplythirdremark { get; set; }
        public string suppapplythirdtime { get; set; }
        public string refundrefid { get; set; }
        public string sustatusname { get; set; }
        public string suppapplyfrom { get; set; }
        public string pnrtype { get; set; }
        public string sickreason { get; set; }
        public Object remarkVos { get; set; }
        public string TuiKuanState { get; set; }
        public decimal HangTuiMoney { get; set; }
    }

   public class RefundInfoList 
    {
        public List<RefundInfo> RefundInfo { get; set; }
    }
}
