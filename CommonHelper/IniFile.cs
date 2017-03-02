using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text; 

namespace CommonHelper
{
    /// <summary>
    /// INI配置文件读取器.
    /// </summary>
    public class IniFile
    {
        [DllImport("kernel32.dll")]
        private static extern Int32 GetPrivateProfileString(string lpApplicationName
                                                            , string lpKeyName
                                                            , string lpDefault
                                                            , StringBuilder lpReturnedString
                                                            , Int32 nSize
                                                            , string lpFileName);
        [DllImport("kernel32.dll")]
        private static extern Int32 WritePrivateProfileString(string lpApplicationName
                                                            , string lpKeyName
                                                            , string lpString
                                                            , string lpFileName);
        public static readonly IniFile _IniFile = new IniFile(Logger.AppPath+"Config.ini");

        private string xIniFileName;//ini 文件名
        private string xDefaultString;//没找到文件或没有返回结果时反回的默认串

        public IniFile()
        {
            xIniFileName = "";
            xDefaultString = "";
        }

        /// <summary>
        /// 设置INI文件名.
        /// </summary>
        /// <param name="IniFileName">INI文件名</param>
        public IniFile(string IniFileName)
        {
            xIniFileName = IniFileName;
            xDefaultString = "";
        }

        /// <summary>
        /// 设置INI文件名.与默认返回值.
        /// </summary>
        /// <param name="IniFileName">INI文件名</param>
        /// <param name="DefaultString">无结果时默认返回数据.</param>
        public IniFile(string IniFileName, string DefaultString)
        {
            xIniFileName = IniFileName;
            xDefaultString = DefaultString;
        }

        //REM Ini profiles Read and Write Functions默认没有找到任何一个段或值时返回默认值 xDefaultString
        /// <summary>
        /// 读取配置文件数据.
        /// </summary>
        /// <param name="sSection">段名.</param>
        /// <param name="sKeyName">键名.</param>
        /// <param name="sReturnedString">输出参数返回结果.</param>
        /// <param name="sIniFileName">配置文件名.</param>
        /// <returns>成功返回读取的字符数.</returns>
        public Int32 ProfileIniRead(string sSection, string sKeyName, out  string sReturnedString, string sIniFileName)
        {
            string sIniFullPathName;
            char[] sBuff = new char[128];
            StringBuilder xBuff = new StringBuilder(Convert.ToString(sBuff));
            Int32 lRet;
            if (sIniFileName.CompareTo("") == 0)
            {
                sIniFullPathName = xIniFileName;
            }
            else
            {
                sIniFullPathName = sIniFileName;
                xIniFileName = sIniFileName;
            }
            if (!System.IO.File.Exists(sIniFullPathName))
            {
                sReturnedString = xDefaultString;
                return -1;
            }
            lRet = GetPrivateProfileString(sSection, sKeyName, xDefaultString,xBuff, 256, sIniFullPathName);
            sReturnedString = xBuff.ToString().Substring(0, lRet);
            //MessageBox.Show(sReturnedString, lRet.ToString(), MessageBoxButtons.OK);
            return lRet;
        }

        //REM Ini profiles Read and Write Functions
        /// <summary>
        /// 写入配置文件数据.
        /// </summary>
        /// <param name="sSection">段名.</param>
        /// <param name="sKeyName">键名.</param>
        /// <param name="sSetString">设置的字符串值.</param>
        /// <param name="sIniFileName">配置文件名.</param>
        /// <returns></returns>
        public Int32 ProfileIniWrite(string sSection, string sKeyName, string sSetString, string sIniFileName)
        {
            string sIniFullPathName;
            Int32 lRet;
            if (sIniFileName.CompareTo("") == 0)
            {
                sIniFullPathName = xIniFileName;
            }
            else
            {
                sIniFullPathName = sIniFileName;
                xIniFileName = sIniFileName;
            }
            if (!System.IO.File.Exists(sIniFullPathName))
            {
                return -1;
            }
            if (sSetString.Length > 128)//设定的字符串过长,128
            {
                return -1;
            }
            lRet = WritePrivateProfileString(sSection, sKeyName, sSetString, sIniFullPathName);
            return lRet;
        }

        /// <summary>
        /// 读取配置文件数据.
        /// </summary>
        /// <param name="sSection">段名.</param>
        /// <param name="sKeyName">键名.</param>
        /// <param name="sReturnedString">输出参数返回结果.</param>
        /// <returns>成功返回读取的字符数.</returns>
        public Int32 ProfileIniRead(string sSection, string sKeyName, out string sReturnedString)
        {
            return ProfileIniRead(sSection, sKeyName,out sReturnedString, "");
        }

        //REM Ini profiles Read and Write Functions
        /// <summary>
        /// 写入配置文件数据.
        /// </summary>
        /// <param name="sSection">段名.</param>
        /// <param name="sKeyName">键名.</param>
        /// <param name="sSetString">设置的字符串值.</param>
        /// <returns></returns>
        public Int32 ProfileIniWrite(string sSection, string sKeyName, string sSetString)
        {
            return ProfileIniWrite(sSection, sKeyName, sSetString, "");
        }

        /// <summary>
        /// 配置文件名.
        /// </summary>
        public string IniFileName
        {
            get
            {
                return xIniFileName;
            }
            set
            {
                xIniFileName = value;
            }
        }

        /// <summary>
        /// 默认返回值.
        /// </summary>
        public string DefaultString
        {
            get
            {
                return xDefaultString;
            }
            set
            {
                xDefaultString = value;
            }
        } 
    }
}
