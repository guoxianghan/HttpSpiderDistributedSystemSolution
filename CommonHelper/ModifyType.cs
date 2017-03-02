using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonHelper
{
    public enum ModifyType
    {
        /// <summary>
        /// 无效的设置项
        /// </summary>
        None = 0,
        /// <summary>
        /// 持平
        /// </summary>
        Impartial = 1,
        /// <summary>
        /// 超点
        /// </summary>
        OutNumber = 2,
        /// <summary>
        /// 设置为限制值
        /// </summary>
        MaxValue = 3,
        /// <summary>
        /// 不跟降,如果别家值很低,不跟降,如果别家值高的,按超点设置
        /// </summary>
        NoDown = 4, 
        /// <summary>
        /// 不比较,设置成固定值
        /// </summary>
        FixedValue = 5,       
    }
}
