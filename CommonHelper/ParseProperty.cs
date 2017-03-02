using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonHelper
{
   public class ParseProperty
    {
       public static dynamic TryParsePropertyType(Type type, object obj, out string errinfo)
       {
           bool isnullable = IsNullableType(type);
           if (isnullable)
           { type = type.GetGenericArguments()[0]; }

           dynamic Obj = null;
           errinfo = "";

           try
           {
               switch (type.Name)
               {
                   case "Int32":
                       Obj = Convert.ToInt32(obj);
                       break;
                   case "Int16":
                       Obj = Convert.ToInt16(obj);
                       break;
                   case "Int64":
                       Obj = Convert.ToInt64(obj);
                       break;
                   case "Decimal":
                       Obj = Convert.ToDecimal(obj);
                       break;
                   case "decimal":
                       Obj = Convert.ToDecimal(obj);
                       break;
                   case "Char":
                       Obj = Convert.ToChar(obj);
                       break;
                   case "DateTime":
                       Obj = Convert.ToDateTime(obj);
                       break;
                   case "Boolean":
                       Obj = Convert.ToBoolean(obj);
                       break;
                   case "bool":
                       Obj = Convert.ToBoolean(obj);
                       break;
                   default:
                       Obj = obj.ToString();
                       break;
               }

           }
           catch (Exception ex)
           {
               errinfo = ex.Message;
           }
           return Obj;
       }
       /// <summary>
       /// 判断是否为可空类型
       /// </summary>
       /// <param name="theType"></param>
       /// <returns></returns>
       static bool IsNullableType(Type theType)
       {
           return (theType.IsGenericType && theType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
       }
    }
}
