using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonHelper
{
   public static class StaticExpand
    {
       public static bool TryParse(this DateTime d,string value,out DateTime? Dt)
       { 
           DateTime ddd;
           if (DateTime.TryParse(value, out ddd))
           { Dt = ddd; return true; }
           else { Dt = null; return false; }
       } 
    }
}
