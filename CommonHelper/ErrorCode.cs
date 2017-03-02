using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CommonHelper
{
   public enum ErrorCode
    {
        [Description("正常")]
        Normal = 0,

        [Description("登陆超时")]
        TimeOut = 1,

        [Description("登陆失败")]
        LoginFailed = 2,

        [Description("未找到记录")]
        NotFount = 3,

        [Description("修改失败")]
        ModifyFailed = 4,

        [Description("查询失败")]
        QueryFailed = 5,
        [Description("需要退出")]
        NeedQuit = 6,
    }
}
