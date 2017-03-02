#region Copyright (c) 2015 青岛华俊航空旅游服务有限公司 All Rights Reserved
/* ********************************************************************************************
* 文件名	: NPOIExcelHelper.cs
* 创建日期	: 2015-11-19
* 作者		: 王剑
* 版本		: 1.0
* 说明    	: Excel操作类，第二版（基于NPOI）
*----------------------------------------------------------------------------------------------
* 修改记录 	:
* 日  期             版本             修改人        修改内容
* 2016-1-26      1.1               王剑           通过DataTable的ExtendedProperties属性中加入DataType节点的Date属性，控制日期类型的输出格式是否带时间
***********************************************************************************************/
#endregion
using System;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using NPOI.XSSF.UserModel;

namespace CommonHelper
{
    public class NPOIExcelHelperV2 : IDisposable
    {
        private string fileName = null; //文件名
        private IWorkbook workbook = null;
        private FileStream fs = null;
        private bool disposed;

        public NPOIExcelHelperV2(string fileName)
        {
            this.fileName = fileName;
            disposed = false;
        }



        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="data">要导入的数据</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        public int DataTableToExcel(DataTable data, string sheetName, bool isColumnWritten)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;
            fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //if (fileName.IndexOf(".xlsx") > 0) // 2007版本
            //    workbook = new NPOI. XSSFWorkbook();
            //else if (fileName.IndexOf(".xls") > 0) // 2003版本
            workbook = new HSSFWorkbook();

            try
            {
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet(sheetName);
                }
                else
                {
                    return -1;
                }

                if (isColumnWritten == true) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                    }
                    count = 1;
                }
                else
                {
                    count = 0;
                }

                for (i = 0; i < data.Rows.Count; ++i)
                {
                    IRow row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }
                    ++count;
                }
                workbook.Write(fs); //写入到excel
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return -1;
            }
        }

        public static int DataTable2Excel(DataTable dt, string fileName)
        {
            return DataTable2Excel(dt, fileName, "sheet1", "");
        }

        public static int DataTable2Excel(DataTable dt, string fileName, string sheetName, string exportColList)
        {

            int exitCode = 0;
            string[] cols;
            if (fileName.EndsWith(".xls") && dt.Rows.Count >= 65535)
            {
                fileName += "x";
            }
            FileStream fsexcel = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            {
                dynamic workbook = null;
                if (fileName.EndsWith(".xls"))
                {

                    if (File.Exists(fileName))
                    {
                        workbook = new HSSFWorkbook();
                    }
                    else
                    {
                        workbook = new HSSFWorkbook();
                    }
                }
                else
                {
                    if (File.Exists(fileName))
                    {
                        workbook = new XSSFWorkbook();
                    }
                    else
                    {
                        workbook = new XSSFWorkbook();
                    }

                }
                ISheet sheet = null;

                HashSet<string> hsColList = new HashSet<string>();
                if (string.IsNullOrEmpty(exportColList))
                {
                    cols = new string[dt.Columns.Count];

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        cols[i] = dt.Columns[i].ColumnName;
                    }
                }
                else
                {
                    cols = exportColList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                }

                for (int i = 0; i < cols.Length; i++)
                {
                    hsColList.Add(cols[i]);
                }
                if (workbook.GetSheet(sheetName) != null)
                {
                    sheet = workbook.CreateSheet(sheetName + "_ex");
                }
                else
                {
                    sheet = workbook.CreateSheet(sheetName);
                }

                workbook.SetSheetOrder(sheet.SheetName, 0);
                workbook.SetActiveSheet(0);
                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i);
                }

                IRow headerRow = sheet.GetRow(0);
                for (int i = 0; i < cols.Length; i++)
                {
                    ICell cell = headerRow.CreateCell(i);
                    cell.SetCellValue(cols[i]);
                }

                int colIndex = 0;
                for (int i = 0; i < cols.Length; i++)
                {
                    if (dt.Columns.Contains(cols[i]))
                    {
                        string dataType = "";
                        if (dt.Columns[cols[i]].ExtendedProperties["DataType"] != null)
                            dataType = dt.Columns[cols[i]].ExtendedProperties["DataType"].ToString();
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            ICell cell = sheet.GetRow(j + 1).CreateCell(colIndex);
                            switch (dt.Columns[i].DataType.ToString())
                            {

                                case ("DBNull"):
                                    cell.SetCellType(CellType.Blank);
                                    break;
                                case ("System.Byte[]"):
                                    break;
                                case ("SByte"):
                                case ("UInt16"):
                                case ("UInt32"):
                                case ("UInt64"):
                                    if (dt.Rows[j][cols[i]] != DBNull.Value)
                                    {
                                        cell.SetCellType(CellType.Numeric);
                                        cell.SetCellValue(Convert.ToUInt64(dt.Rows[j][cols[i]]));
                                    }
                                    break;
                                case ("System.Int16"):
                                case ("System.Int32"):
                                case ("System.Int64"):
                                    if (dt.Rows[j][cols[i]] != DBNull.Value)
                                    {
                                        cell.SetCellType(CellType.Numeric);
                                        cell.SetCellValue(Convert.ToInt64(dt.Rows[j][cols[i]]));
                                    }
                                    break;
                                case ("Char"):
                                case ("System.String"):
                                    if (dt.Rows[j][cols[i]] != DBNull.Value)
                                    {
                                        cell.SetCellType(CellType.String);
                                        cell.SetCellValue(dt.Rows[j][cols[i]] as string);
                                    }
                                    break;
                                case ("System.Boolean"):
                                    if (dt.Rows[j][cols[i]] != DBNull.Value)
                                    {
                                        cell.SetCellType(CellType.Boolean);
                                        cell.SetCellValue(Convert.ToBoolean(dt.Rows[j][cols[i]]));
                                    }
                                    break;

                                case ("System.DateTime"):
                                    if (dt.Rows[j][cols[i]] != DBNull.Value)
                                    {
                                        cell.SetCellType(CellType.String);
                                        if ("DATE".Equals(dataType, StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            cell.SetCellValue(Convert.ToDateTime(dt.Rows[j][cols[i]]).ToString("yyyy-MM-dd"));
                                        }
                                        else
                                        {
                                            cell.SetCellValue(Convert.ToDateTime(dt.Rows[j][cols[i]]).ToString());
                                        }

                                    }
                                    break;

                                case ("System.Double"):
                                    if (dt.Rows[j][cols[i]] != DBNull.Value)
                                    {
                                        cell.SetCellType(CellType.Numeric);
                                        cell.SetCellValue(Convert.ToDouble(dt.Rows[j][cols[i]]));
                                    }
                                    break;

                                case ("System.Decimal"):
                                    if (dt.Rows[j][cols[i]] != DBNull.Value)
                                    {
                                        cell.SetCellType(CellType.Numeric);
                                        cell.SetCellValue(Convert.ToDouble(dt.Rows[j][cols[i]]));
                                    }
                                    break;

                                case ("System.Guid"):
                                    if (dt.Rows[j][cols[i]] != DBNull.Value)
                                    {
                                        cell.SetCellType(CellType.String);
                                        cell.SetCellValue(Convert.ToString(dt.Rows[j][cols[i]]));
                                    }
                                    break;

                                case ("System.Object"):
                                    if (dt.Rows[j][cols[i]] != DBNull.Value)
                                    {
                                        cell.SetCellType(CellType.Unknown);
                                        cell.SetCellValue(Convert.ToString(dt.Rows[j][cols[i]]));
                                    }
                                    break;
                            }
                        }
                        colIndex++;
                    }
                }

                if (false && File.Exists(fileName))
                {
                    using (FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        workbook.Write(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                    }
                }
                else
                {
                    workbook.Write(fsexcel);
                    //fsexcel.Flush();
                    fsexcel.Close();
                }
            }
            return exitCode;
        }

        /// <summary>
        /// 将DataTable中的数据导出为Excel，并通过Response对象将数据写入到客户端（Web工程方法），使用HttpContext对象获取当前页面的Page
        /// </summary>
        /// <param name="dt">数据</param>
        /// <param name="fileName">文件名</param>
        /// <param name="sheetName">生成的sheet名</param>
        /// <param name="exportColList">要导出的列</param>
        public static void DataTable2ExcelDownload(DataTable dt, string fileName, string sheetName, string exportColList)
        {
            //int exitCode = 0;
            string[] cols;
            if (fileName.EndsWith(".xls") && dt.Rows.Count >= 65535)
            {
                fileName += "x";
            }
            FileStream fsexcel = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            {
                dynamic workbook = null;
                if (fileName.EndsWith(".xls"))
                {

                    if (File.Exists(fileName))
                    {
                        workbook = new HSSFWorkbook();
                    }
                    else
                    {
                        workbook = new HSSFWorkbook();
                    }
                }
                else
                {
                    if (File.Exists(fileName))
                    {
                        workbook = new XSSFWorkbook();
                    }
                    else
                    {
                        workbook = new XSSFWorkbook();
                    }

                }
                ISheet sheet = null;

                HashSet<string> hsColList = new HashSet<string>();
                if (string.IsNullOrEmpty(exportColList))
                {
                    cols = new string[dt.Columns.Count];

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        cols[i] = dt.Columns[i].ColumnName;
                    }
                }
                else
                {
                    cols = exportColList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                }

                for (int i = 0; i < cols.Length; i++)
                {
                    hsColList.Add(cols[i]);
                }
                if (workbook.GetSheet(sheetName) != null)
                {
                    sheet = workbook.CreateSheet(sheetName + "_ex");
                }
                else
                {
                    sheet = workbook.CreateSheet(sheetName);
                }

                workbook.SetSheetOrder(sheet.SheetName, 0);
                workbook.SetActiveSheet(0);
                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i);
                }

                IRow headerRow = sheet.GetRow(0);
                for (int i = 0; i < cols.Length; i++)
                {
                    ICell cell = headerRow.CreateCell(i);
                    cell.SetCellValue(cols[i]);
                }

                int colIndex = 0;
                for (int i = 0; i < cols.Length; i++)
                {
                    if (dt.Columns.Contains(cols[i]))
                    {

                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            ICell cell = sheet.GetRow(j + 1).CreateCell(colIndex);
                            switch (dt.Columns[i].DataType.ToString())
                            {

                                case ("DBNull"):
                                    cell.SetCellType(CellType.Blank);
                                    break;
                                case ("System.Byte[]"):
                                    break;
                                case ("SByte"):
                                case ("UInt16"):
                                case ("UInt32"):
                                case ("UInt64"):
                                    if (dt.Rows[j][cols[i]] != DBNull.Value)
                                    {
                                        cell.SetCellType(CellType.Numeric);
                                        cell.SetCellValue(Convert.ToUInt64(dt.Rows[j][cols[i]]));
                                    }
                                    break;
                                case ("System.Int16"):
                                case ("System.Int32"):
                                case ("System.Int64"):
                                    if (dt.Rows[j][cols[i]] != DBNull.Value)
                                    {
                                        cell.SetCellType(CellType.Numeric);
                                        cell.SetCellValue(Convert.ToInt64(dt.Rows[j][cols[i]]));
                                    }
                                    break;
                                case ("Char"):
                                case ("System.String"):
                                    if (dt.Rows[j][cols[i]] != DBNull.Value)
                                    {
                                        cell.SetCellType(CellType.String);
                                        cell.SetCellValue(dt.Rows[j][cols[i]] as string);
                                    }
                                    break;
                                case ("System.Boolean"):
                                    if (dt.Rows[j][cols[i]] != DBNull.Value)
                                    {
                                        cell.SetCellType(CellType.Boolean);
                                        cell.SetCellValue(Convert.ToBoolean(dt.Rows[j][cols[i]]));
                                    }
                                    break;

                                case ("System.DateTime"):
                                    if (dt.Rows[j][cols[i]] != DBNull.Value)
                                    {
                                        cell.SetCellType(CellType.String);
                                        cell.SetCellValue(Convert.ToDateTime(dt.Rows[j][cols[i]]).ToString());
                                    }
                                    break;

                                case ("System.Double"):
                                    if (dt.Rows[j][cols[i]] != DBNull.Value)
                                    {
                                        cell.SetCellType(CellType.Numeric);
                                        cell.SetCellValue(Convert.ToDouble(dt.Rows[j][cols[i]]));
                                    }
                                    break;

                                case ("System.Decimal"):
                                    if (dt.Rows[j][cols[i]] != DBNull.Value)
                                    {
                                        cell.SetCellType(CellType.Numeric);
                                        cell.SetCellValue(Convert.ToDouble(dt.Rows[j][cols[i]]));
                                    }
                                    break;

                                case ("System.Guid"):
                                    if (dt.Rows[j][cols[i]] != DBNull.Value)
                                    {
                                        cell.SetCellType(CellType.String);
                                        cell.SetCellValue(Convert.ToString(dt.Rows[j][cols[i]]));
                                    }
                                    break;

                                case ("System.Object"):
                                    if (dt.Rows[j][cols[i]] != DBNull.Value)
                                    {
                                        cell.SetCellType(CellType.Unknown);
                                        cell.SetCellValue(Convert.ToString(dt.Rows[j][cols[i]]));
                                    }
                                    break;
                            }
                        }
                        colIndex++;
                    }
                }

                if (false && File.Exists(fileName))
                {
                    using (FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        workbook.Write(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                    }
                }
                else
                {

                    //workbook.Write(fsexcel);
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    workbook.Write(ms);
                    HttpContext.Current.Response.ClearContent();
                    HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", fileName));
                    HttpContext.Current.Response.ContentType = "application/excel";
                    HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                    HttpContext.Current.Response.BinaryWrite(ms.ToArray());
                    HttpContext.Current.Response.End();
                    //exitBytes = new byte[ms.Length];
                    //ms.Write(exitBytes,0, exitBytes.Length);
                    //ms.Close();
                    //ms.Dispose();
                    //fsexcel.Flush();
                    fsexcel.Close();
                }
            }

        }

        public static int DataTable2Excel4MU(DataTable dt, string fileName)
        {
            int errorcount = 0;
            FileStream fsexcel = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            {
                IWorkbook workbookMU = null;
                ISheet sheetMU = null;
                workbookMU = new HSSFWorkbook(fsexcel);

                sheetMU = workbookMU.GetSheetAt(0);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheetMU.GetRow(i + 5);

                    if (dt.Rows[i][0].ToString().Equals(row.GetCell(0).ToString()))
                    {
                        ICell ce = row.CreateCell(17);
                        ce.SetCellType(CellType.String);
                        ce.SetCellValue(Convert.ToDateTime(dt.Rows[i][1]).ToString());
                        ce = row.CreateCell(18);
                        ce.SetCellType(CellType.String);
                        ce.SetCellValue(Convert.ToDouble(i / 10.0));
                    }
                    else
                    {

                        errorcount++;
                    }
                }

                using (FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    workbookMU.Write(fileStream);
                    fileStream.Close();
                }

            }
            return errorcount;
        }

        public static DataTable ExcelToDataTable(Stream fsexcel, string fileName, string sheetName, int startRow, int startCol, int endRow,
            int endCol, bool firsRowIsHeader)
        {
            DataTable dt = new DataTable();
            //using (FileStream fsexcel = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            //{
            dynamic workbook = null;
            ISheet sheet = null;
            if (fileName.EndsWith(".xlsx"))
            {

                workbook = new XSSFWorkbook(fsexcel);
            }
            else
            {
                workbook = new HSSFWorkbook(fsexcel);
            }
            if (!string.IsNullOrEmpty(sheetName))
            {
                sheet = workbook.GetSheet(sheetName);
            }

            if (sheet == null)
            {
                sheet = workbook.GetSheetAt(0);
            }

            IRow firstRow = sheet.GetRow(startRow);

            if (endRow < 0)
            {

                endRow = sheet.LastRowNum;
            }
            if (endCol < 0)
            {

                endCol = firstRow.LastCellNum;
            }

            if (firsRowIsHeader)
            {
                for (int i = startCol; i < endCol; i++)
                {
                    if (firstRow.GetCell(i) == null)
                    {
                        endCol = i + 1;
                        break;
                    }
                    string colName = firstRow.GetCell(i).ToString().Trim();
                    if (dt.Columns.Contains(colName))
                    {

                        dt.Columns.Add(colName + "_");
                    }
                    else
                    {
                        dt.Columns.Add(colName);

                    }
                }
            }
            else
            {
                for (int i = startCol; i < endCol; i++)
                {
                    dt.Columns.Add(string.Format("Col{0}", i));
                }
            }

            if (firsRowIsHeader)
            {

                startRow++;
            }

            for (int i = startRow; i <= endRow; i++)
            {
                IRow row = sheet.GetRow(i);
                //没有数据的行默认是null　　　　　　　
                if (row == null) continue;

                DataRow dataRow = dt.NewRow();
                for (int j = startCol; j < endCol; j++)
                {
                    ICell cel = row.GetCell(j);
                    if (cel == null)
                    {
                        dataRow[j] = DBNull.Value;
                    }
                    else
                    {
                        dataRow[j] = row.GetCell(j).ToString();
                    }
                }

                dt.Rows.Add(dataRow);
                //}
                fsexcel.Close();
            }

            return dt;
        }

        public static DataTable ExcelToDataTable(string fileName, string sheetName, int startRow, int startCol, int endRow, int endCol, bool firsRowIsHeader)
        {
            DataTable dt = new DataTable();
            using (FileStream fsexcel = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                dynamic workbook = null;
                ISheet sheet = null;
                if (fileName.EndsWith(".xlsx"))
                {

                    workbook = new XSSFWorkbook(fsexcel);
                }
                else
                {
                    workbook = new HSSFWorkbook(fsexcel);
                }
                if (!string.IsNullOrEmpty(sheetName))
                {
                    sheet = workbook.GetSheet(sheetName);
                }

                if (sheet == null)
                {
                    sheet = workbook.GetSheetAt(0);
                }

                IRow firstRow = sheet.GetRow(startRow);

                if (endRow < 0)
                {

                    endRow = sheet.LastRowNum;
                }
                if (endCol < 0)
                {

                    endCol = firstRow.LastCellNum;
                }

                if (firsRowIsHeader)
                {
                    for (int i = startCol; i < endCol; i++)
                    {
                        if (firstRow.GetCell(i) == null)
                        {
                            endCol = i + 1;
                            break;
                        }
                        string colName = firstRow.GetCell(i).ToString().Trim();
                        if (dt.Columns.Contains(colName))
                        {

                            dt.Columns.Add(colName + "_");
                        }
                        else
                        {
                            dt.Columns.Add(colName);

                        }
                    }
                }
                else
                {
                    for (int i = startCol; i < endCol; i++)
                    {
                        dt.Columns.Add(string.Format("Col{0}", i));
                    }
                }

                if (firsRowIsHeader)
                {

                    startRow++;
                }

                for (int i = startRow; i <= endRow; i++)
                {
                    IRow row = sheet.GetRow(i);
                    //没有数据的行默认是null　　　　　　　
                    if (row == null) continue;

                    DataRow dataRow = dt.NewRow();
                    for (int j = startCol; j < endCol; j++)
                    {
                        ICell cel = row.GetCell(j);
                        if (cel == null)
                        {
                            dataRow[j] = DBNull.Value;
                        }
                        else
                        {
                            dataRow[j] = row.GetCell(j).ToString();
                        }
                    }

                    dt.Rows.Add(dataRow);
                }
                fsexcel.Close();
            }

            return dt;
        }
        public static DataTable ExcelToDataTable(FileStream fsexcel,string fileName, string sheetName, int startRow, int startCol, int endRow, int endCol, bool firsRowIsHeader)
        {
            DataTable dt = new DataTable();
            
            {
                dynamic workbook = null;
                ISheet sheet = null;
                if (fileName.EndsWith(".xlsx"))
                {
                    workbook = new XSSFWorkbook(fsexcel);
                }
                else
                {
                    workbook = new HSSFWorkbook(fsexcel);
                }
                if (!string.IsNullOrEmpty(sheetName))
                {
                    sheet = workbook.GetSheet(sheetName);
                }

                if (sheet == null)
                {
                    sheet = workbook.GetSheetAt(0);
                }

                IRow firstRow = sheet.GetRow(startRow);

                if (endRow < 0)
                {

                    endRow = sheet.LastRowNum;
                }
                if (endCol < 0)
                {

                    endCol = firstRow.LastCellNum;
                }

                if (firsRowIsHeader)
                {
                    for (int i = startCol; i < endCol; i++)
                    {
                        if (firstRow.GetCell(i) == null)
                        {
                            endCol = i + 1;
                            break;
                        }
                        string colName = firstRow.GetCell(i).ToString().Trim();
                        if (dt.Columns.Contains(colName))
                        {

                            dt.Columns.Add(colName + "_");
                        }
                        else
                        {
                            dt.Columns.Add(colName);

                        }
                    }
                }
                else
                {
                    for (int i = startCol; i < endCol; i++)
                    {
                        dt.Columns.Add(string.Format("Col{0}", i));
                    }
                }

                if (firsRowIsHeader)
                {

                    startRow++;
                }

                for (int i = startRow; i <= endRow; i++)
                {
                    IRow row = sheet.GetRow(i);
                    //没有数据的行默认是null　　　　　　　
                    if (row == null) continue;

                    DataRow dataRow = dt.NewRow();
                    for (int j = startCol; j < endCol; j++)
                    {
                        ICell cel = row.GetCell(j);
                        if (cel == null)
                        {
                            dataRow[j] = DBNull.Value;
                        }
                        else
                        {
                            dataRow[j] = row.GetCell(j).ToString();
                        }
                    }

                    dt.Rows.Add(dataRow);
                }
                fsexcel.Close();
            }

            return dt;
        }

        public static DataTable ExcelToDataTable4MU(string fileName)
        {

            // todo:暂定此处为从第4行开始，第4行为表头行
            int startRowNum = 4;
            DataTable dt = new DataTable("MUDataTable");
            using (FileStream fsexcel = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbookMU = null;
                ISheet sheetMU = null;
                workbookMU = new HSSFWorkbook(fsexcel);

                sheetMU = workbookMU.GetSheetAt(0);

                IRow headerRow = sheetMU.GetRow(startRowNum);
                int colnum = headerRow.LastCellNum;
                for (int i = 0; i < colnum; i++)
                {
                    dt.Columns.Add(headerRow.GetCell(i).ToString().Trim());
                }
                //最后一列的标号
                int rowCount = sheetMU.LastRowNum;

                for (; rowCount > startRowNum; rowCount--)
                {
                    IRow row = sheetMU.GetRow(rowCount);
                    if ("总订单数：".Equals(row.GetCell(0).ToString()))
                    {

                        rowCount--;
                        break;
                    }
                }

                for (int i = startRowNum + 1; i < rowCount; i++)
                {
                    IRow row = sheetMU.GetRow(i);
                    //没有数据的行默认是null
                    if (row == null) continue;

                    DataRow dataRow = dt.NewRow();
                    for (int j = 0; j < colnum; j++)
                    {
                        dataRow[j] = row.GetCell(j).ToString();
                    }

                    dt.Rows.Add(dataRow);
                }
                fsexcel.Close();
            }
            return dt;
        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        public DataTable ExcelToDataTable(string sheetName, bool isFirstRowColumn, int startRow, int endRow, int startCol, int endCol)
        {
            ISheet sheet = null;
            DataTable data = new DataTable();
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                //if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                //    workbook = new XSSFWorkbook(fs);
                //else if (fileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook(fs);

                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum;

                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }
                        //startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        //startRow = sheet.FirstRowNum;
                    }

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                dataRow[j] = row.GetCell(j).ToString();
                        }
                        data.Rows.Add(dataRow);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (fs != null)
                        fs.Close();
                }

                fs = null;
                disposed = true;
            }
        }
    }
}