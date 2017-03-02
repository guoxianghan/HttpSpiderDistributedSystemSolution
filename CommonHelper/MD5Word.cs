using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace CommonHelper
{
   public class MD5Word
    {
        /// <summary>
        /// MD5 32位加密 UTF8 转码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UserMd5(string str)
        {
            byte[] result = Encoding.Default.GetBytes(str);    //tbPass为输入密码的文本框
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            string re = BitConverter.ToString(output).Replace("-", "");  //tbMd5pass为输出加密文本
            return re;
        }
    }
}
