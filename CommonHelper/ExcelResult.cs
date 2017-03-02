using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonHelper
{
   public class ExcelResult
   {
       public string agentName { get; set; }
       public DateTime? bookingEndDate { get; set; }
       public DateTime? bookingStartDate { get; set; }
       public string excelName { get; set; }
       public string excelPath { get; set; }
       public string flightFromDate { get; set; }
       public string flightToDate { get; set; }
       public string refundApplyFromDate { get; set; }
       public string refundApplyToDate { get; set; }
       public string report { get; set; }
       public string reportType { get; set; }
       /// <summary>
       /// -1,报表结果为空，没有可导出数据!;
       /// </summary>
       public string result { get; set; }
       public string subAgents { get; set; } 
    }
}
