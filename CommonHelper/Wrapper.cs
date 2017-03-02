using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CommonHelper
{
        public class Wrapper
        {
            // Methods
            [DllImport("UUWiseHelper.dll")]
            public static extern int uu_getScore(string u, string p);
            [DllImport("UUWiseHelper.dll")]
            public static extern int uu_login(string u, string p);
            [DllImport("UUWiseHelper.dll")]
            public static extern int uu_pay(string u, string card, int softId, string softKey);
            [DllImport("UUWiseHelper.dll")]
            public static extern int uu_recognizeByCodeTypeAndBytes(byte[] picContent, int picLength, int codeType, StringBuilder result);
            [DllImport("UUWiseHelper.dll")]
            public static extern int uu_recognizeByCodeTypeAndPath(string path, int codeType, StringBuilder result);
            [DllImport("UUWiseHelper.dll")]
            public static extern int uu_regUser(string u, string p, int softid, string softKey);
            [DllImport("UUWiseHelper.dll")]
            public static extern int uu_reportError(int nCodeID);
            [DllImport("UUWiseHelper.dll")]
            public static extern void uu_setSoftInfo(int softId, string softKey);
        }

}
