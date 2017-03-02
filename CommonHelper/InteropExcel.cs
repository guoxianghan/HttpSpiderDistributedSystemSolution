using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
//using Excel;
using System.IO;
namespace CommonHelper
{
    public class InteropExcel
    {
        //out string strError = "";
        /// <summary>
        /// 通过向单元格写数据的方法将数据写入Excel
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="strExcelFileName">Excel文件的路径名</param>
        /// <param name="strTableName">文件名(sheet名)</param>
        /// <param name="strTitle">数据表的标题(空或null则不写)</param>
        /// <param name="bIsOpenExcel">是否立即打开Excel表</param>
        /// <returns>0--成功,-1--导入到Excel失败,-2--销毁进程失败,-3打开Excel表失败</returns>
        public static int WriteToExcelByCell(System.Data.DataTable dt, string strExcelFileName, string strTableName, string strTitle, bool bIsOpenExcel, out string strError)
        {
            strError = "";
            DateTime datetime = DateTime.Now;
            try
            {

                Application excelApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
                excelApp.DisplayAlerts = true;
                excelApp.SheetsInNewWorkbook = 1;
                Workbook excelBook = excelApp.Workbooks.Add(Type.Missing);
                Worksheet excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelBook.ActiveSheet;
                excelSheet.Name = strTableName;
                int nRowIndex = 1;//行号
                if (string.IsNullOrEmpty(strTitle) == false)
                {
                    string a = (Convert.ToChar(64 + dt.Columns.Count)).ToString() + "1";
                    Range rH = excelSheet.get_Range("A1", a);
                    rH.Merge(0);
                    rH.HorizontalAlignment = XlVAlign.xlVAlignCenter;
                    rH.VerticalAlignment = XlVAlign.xlVAlignCenter;
                    excelApp.Cells[nRowIndex++, 1] = strTitle;
                }
                for (int i = 1; i <= dt.Columns.Count; i++)
                {
                    excelApp.Cells[nRowIndex, i] = dt.Columns[i - 1].ColumnName;
                }
                nRowIndex++;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        excelApp.Cells[nRowIndex, j + 1] = dt.Rows[i][j];
                    }
                    nRowIndex++;
                }
                excelBook.Saved = true;
                excelBook.SaveCopyAs(strExcelFileName);
                if (excelApp != null)
                {
                    excelApp.Workbooks.Close();
                    excelApp.Quit();
                    int generation = System.GC.GetGeneration(excelApp);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    excelApp = null;
                    System.GC.Collect(generation);
                }
                GC.Collect();//强行销毁
            }
            catch (Exception ex)
            {
                strError = "导入到Excel出错:" + ex.Message;
                return -1;
            }
            if (KillExcelProcess(datetime, out strError) == false)
            {
                return -2;
            }
            if (bIsOpenExcel == true)
            {
                try
                {
                    System.Diagnostics.Process.Start(strExcelFileName);
                }
                catch (Exception ex)
                {
                    strError = "打开Excel表失败:" + ex.Message;
                    return -3;
                }
            }
            return 0;

        }

        /// <summary>
        /// 销毁进程
        /// </summary>
        /// <param name="datetime">销毁进程的时间</param>
        /// <returns></returns>
        private static bool KillExcelProcess(DateTime datetime, out string strError)
        {
            strError = "";
            try
            {
                System.Diagnostics.Process[] excelProc = System.Diagnostics.Process.GetProcessesByName("EXCEL");
                for (int m = 0; m < excelProc.Length; m++)
                {
                    if (datetime < excelProc[m].StartTime)
                    {
                        excelProc[m].Kill();
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                strError = "销毁进程出错:" + ex.Message;
                return false;
            }
        }


        /// <summary>
        /// 通过向单元格写数据的方法将数据写入Excel
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="strExcelFileName">Excel文件的路径名</param>
        /// <param name="strTitle">数据表的标题(空或null则不写)</param>
        /// <param name="bIsOpenExcel">是否立即打开Excel表</param>
        /// <returns>0--成功,-1--导入到Excel失败,-2--销毁进程失败,-3打开Excel表失败</returns>
        public static int WriteToExcelByTab(System.Data.DataTable dt, string strExcelFileName, string strTitle, bool bIsOpenExcel, out string strError)
        {
            strError = "";
            DateTime datetime = DateTime.Now;
            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(strExcelFileName, System.IO.FileMode.Create);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.Unicode);
                if (string.IsNullOrEmpty(strTitle) == false)
                {
                    sw.WriteLine(strTitle);
                }
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sw.Write(dt.Columns[i].ColumnName + "\t");
                }
                sw.Write("\n");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        sw.Write(dt.Rows[i][j].ToString() + "\t");
                    }
                    sw.Write("\n");
                }
                sw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                strError = "导入到Excel出错:" + ex.Message;
                return -1;
            }
            if (KillExcelProcess(datetime, out strError) == false)
            {
                return -2;
            }

            if (bIsOpenExcel == true)
            {
                try
                {
                    System.Diagnostics.Process.Start(strExcelFileName);
                }
                catch (Exception ex)
                {
                    strError = "打开Excel表失败:" + ex.Message;
                    return -3;
                }
            }
            return 0;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt">需要填充的数据表</param>
        /// <param name="strExcelFileName">Excel文件的路径名</param>
        /// <param name="strTableName">文件名(sheet名)</param>
        /// <param name="strSelectField">需要查询的字段和要翻译过来的字段名,为null时选择所有并不进行字段重命名</param>
        /// <param name="strWhere">查询的限制条件</param>
        /// <param name="strOrderBy">排序</param>
        /// <returns></returns>
        public static int GetDataFromExcel(out System.Data.DataTable dt, string strExcelFileName, string strTableName, string[,] strSelectField, string strWhere, string strOrderBy, out string strError)
        {
            strError = "";
            string strSelect;
            dt = null;
            try
            {
                if (strSelectField == null)
                {
                    strSelect = "* ";
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < strSelectField.Length / 2; i++)
                    {
                        sb.Append(strSelectField[i, 0]);
                        sb.Append(" as ");
                        sb.Append(strSelectField[i, 1]);
                        sb.Append(", ");
                    }
                    sb.Remove(sb.Length - 2, 1);
                    strSelect = sb.ToString();
                }
            }
            catch
            {
                strError = "所传的选择字段参数错误!";
                return -1;
            }
            try
            {
                if (File.Exists(strExcelFileName) == false)
                {
                    strError = "文件 " + strExcelFileName + "不存在！";
                    return -2;

                }
                string strCommand = "SELECT " + strSelect + "FROM [" + strTableName + "$] " + strWhere + " " + strOrderBy;
                //new exportExcelDAL().exeCommand(strCommand,strExcelFileName );
            }
            catch (Exception ex)
            {
                strError = "导出Excel出错:" + ex.Message;
                return -1;
            }
            return 0;
        }
        public static bool InteropToExcel(System.Data.DataTable dt,string fullpath,  bool isShowExcel=true)
        {
            int titleColumnSpan = 0;//标题的跨列数
            //string fileName = "";//保存的excel文件名
            int columnIndex = 1;//列索引

            /*建立Excel对象*/
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            if (excel == null)
            {
                return false;
            }
            try
            {
                excel.Application.Workbooks.Add(true);
                excel.Visible = isShowExcel;
                /*分析标题的跨列数*/
                titleColumnSpan = dt.Columns.Count;
                /*合并标题单元格*/
                Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)excel.ActiveSheet;
                columnIndex = 1;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    excel.Cells[1, columnIndex] = dt.Columns[i].ColumnName;
                    (excel.Cells[1, columnIndex] as Range).HorizontalAlignment = XlHAlign.xlHAlignCenter;//字段居中
                   
                    columnIndex++;
                }
                //填充数据     

                #region MyRegion
                /*
		  for (int i = 0; i < dt.Rows.Count; i++)
                {
                    columnIndex = 1;
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (dt[j, i].ValueType == typeof(string))
                        {

                            if (dt[j, i].Value != null)
                                excel.Cells[i + 2, columnIndex] = "" + dt[j, i].Value.ToString();
                        }
                        else
                        {
                            if (dt[j, i].Value != null)
                                excel.Cells[i + 2, columnIndex] = dt[j, i].Value.ToString();
                        }
                        (excel.Cells[i + 2, columnIndex] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;//字段居中
                        columnIndex++;
                    } 

                }*/
                #endregion
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    columnIndex = 1;
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (dt.Rows[i][j] != null)
                            excel.Cells[i + 2, columnIndex] = "" + dt.Rows[i][j].ToString();
                        //if (dt[j, i].Value != null)
                        //    excel.Cells[i + 2, columnIndex] = dt.Rows[i][j].ToString();

                        (excel.Cells[i + 2, columnIndex] as Range).HorizontalAlignment = XlHAlign.xlHAlignLeft;//字段居中
                         //(excel.Cells[i + 2, columnIndex] as Range).
                        columnIndex++;
                    }
                } worksheet.Columns.EntireColumn.AutoFit();//列宽自适应 
                worksheet.SaveAs(fullpath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            catch { }
            finally
            {
                excel.Quit();
                excel = null;
                GC.Collect();
            }
            //KillProcess("Excel");
            return true;

        }
        private static void KillProcess(string processName)//杀死与Excel相关的进程
        {
            System.Diagnostics.Process myproc = new System.Diagnostics.Process();//得到所有打开的进程
            try
            {
                foreach (System.Diagnostics.Process thisproc in System.Diagnostics.Process.GetProcessesByName(processName))
                {
                    if (!thisproc.CloseMainWindow())
                    {
                        thisproc.Kill();
                    }
                }
            }
            catch (Exception Exc)
            {
                throw new Exception("", Exc);
            }
        }

    }

}
