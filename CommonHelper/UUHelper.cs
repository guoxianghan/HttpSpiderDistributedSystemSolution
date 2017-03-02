using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Security.Cryptography;
using Microsoft.VisualBasic;

namespace CommonHelper
{
    public class UUHelper
    {
      protected static string UserName = "wangqian2";
      protected static string Pwd = "qdhj+2015+yyy";
        /// <summary>
        /// 通过UU打码器 
        /// </summary>
        /// <param name="picbyte"></param>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <param name="codeType"></param>
        /// <returns></returns>
        public static string GetYzmByUU(byte[] picbyte,int codeType,out int codeid,out string info)
        {
            info = "";
            try
            {
                MemoryStream stream = new MemoryStream(picbyte);
                Bitmap bitmap = new Bitmap(stream);
                StringBuilder result = new StringBuilder();
                //通过UU云进行识别
                codeid = Wrapper.uu_recognizeByCodeTypeAndBytes(picbyte, picbyte.Length, codeType, result);
                string code = result.ToString();
                //64位DLL 返回格式：EA8480D620B01EE2BBAEDE79FD94B781_向产亮
                if (code.Contains("_"))
                {
                    code = code = code.Substring(code.IndexOf("_") + 1, code.Length - code.IndexOf("_") - 1);
                }
                return code;
            }
            catch (Exception ex)
            {
                codeid = 0;
                info = ex.Message;
                return "";
            }
        }
        /// <summary>
        /// UU初始化
        /// </summary>
        /// <returns></returns>
        public static string LoadUU()
        {
            string path = Logger.AppPath + "UUWiseHelper.dll";
            try
            {
                string user = UserName;
                string pwd = Pwd;
                string msg = "";
                Wrapper.uu_setSoftInfo(105253, "4b22a6c65bc9475096f8ab8fb9e33436");
                if (CheckMD5(path))
                {
                    MessageBox.Show("您的DLL版本与软件认证DLL版本不符，请使用1.1.0.9动态链接库版DLL进行替换");
                }
                else
                {
                    switch (Wrapper.uu_login(user, pwd))
                    {
                        #region MyRegion
                        case -1001:
                            msg = "连接失败";
                            break;

                        case -1002:
                            msg = "网络传输超时";
                            break;

                        case -1003:
                            msg = "文件访问失败";
                            break;

                        case -1004:
                            msg = "图像内存流无效";
                            break;

                        case -1005:
                            msg = "服务器返回内容错误";
                            break;

                        case -1006:
                            msg = "服务器状态错误";
                            break;

                        case -1007:
                            msg = "内存分配失败";
                            break;

                        case -1008:
                            msg = "没有取到验证码结果，返回此值指示codeID已返回";
                            break;

                        case -1009:
                            msg = "此时不允许进行该操作";
                            break;

                        case -1010:
                            msg = "图片过大，限制10MB";
                            break;

                        case -1:
                            msg = "软件ID或KEY无效或者用户名为空或密码为空";
                            break;

                        case -2:
                            msg = "用户不存在";
                            break;

                        case -3:
                            msg = "密码错误";
                            break;

                        case -4:
                            msg = "账户被锁定";
                            break;

                        case -5:
                            msg = "非法登录";
                            break;

                        case -6:
                            msg = "用户点数不足，请及时充值";
                            break;

                        case -8:
                            msg = "系统维护中";
                            break;

                        case -9:
                            msg = "其他";
                            break; 
                        #endregion

                        default:
                            msg = "登录成功";
                            break;
                    }
                }
                return msg;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();// "UU初始化失败";
            }
        }
        /// <summary>
        /// 返回题分
        /// </summary>
        /// <param name="yzmid"></param>
        public static void BackScore(string  yzmid)
        {
            Wrapper.uu_reportError(Convert.ToInt32(yzmid));
        }
        public static bool CheckMD5(string Path)
        {
            string str2 = GetFileMD5(Path);
            string str = "22843357-82F7-44DB-947B-A6169B1DADDF";
            if (str2 != str)
            {
                return false;
            }
            return true;
        }
        public static string GetFileMD5(string Path)
        {
            FileStream inputStream = new FileStream(Path, FileMode.Open, FileAccess.Read);
            byte[] buffer = new MD5CryptoServiceProvider().ComputeHash(inputStream);
            inputStream.Close();
            string str2 = "";
            int num2 = buffer.Length - 1;
            for (int i = 0; i < num2; i++)
            {
                if (Conversion.Hex(buffer[i]).Length == 1)
                {
                    str2 = str2 + "0" + Conversion.Hex(buffer[i]);
                }
                else
                {
                    str2 = str2 + Conversion.Hex(buffer[i]);
                }
            }
            return str2;
        }
        public static string CheckCode(MemoryStream ms, int codetype, out int codeid,out string info)
        {
            Byte[] imagebytes = ms.ToArray();
            return GetYzmByUU(imagebytes, codetype, out codeid,out info);
        }
    }
}
