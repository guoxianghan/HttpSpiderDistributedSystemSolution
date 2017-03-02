using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;
using org.in2bits.MyXls;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
//using Interop.Excel;
//using NPOI.XSSF.UserModel;

namespace CommonHelper
{
    public abstract class ExcelHelper
    {
        //protected static string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="excelpath"></param>
        /// <returns></returns>
        public static System.Data.DataTable ExcelTable(string excelpath, DataRow datarow, out string err)
        {
            err = "";
            string connExcel = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + excelpath + ";Extended Properties=Excel 8.0;";
            System.Data.DataTable dt = datarow.Table;

            OleDbConnection oleDbConnection = new OleDbConnection(connExcel);
            oleDbConnection.Open();
            System.Data.DataTable dataTable = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            //获取sheet名，其中[0][1]...[N]: 按名称排列的表单元素
            string tableName = dataTable.Rows[0][2].ToString().Trim();
            tableName = "[" + tableName.Replace("'", "") + "]";

            string query = "SELECT [航空公司],[航程类型],[团散],[乘客类型],[政策类型],[出港城市],[到港城市],[出港城市排除],[到港城市排除],[舱位],[同行返点],[下级返点],[航班开始日期],[航班截止日期],[出票开始日期],[出票截止日期],[是否自动出票],[是否换PNR出票],[授权office号],[PNR是否需要授权],[共享航班是否适应],[适用航班号],[不适用航班号],[适用班期],[不适用班期],[工作开始时刻],[工作结束时刻],[备注] FROM " + tableName;
            //string query = "SELECT * FROM " + tableName;
            DataSet dataSet = new DataSet();
            OleDbDataAdapter oleAdapter = new OleDbDataAdapter(query, connExcel);
            oleAdapter.Fill(dataSet, "PolicyTemplate");

            #region 逐行读取数据
            foreach (DataRow dataRow in dataSet.Tables["PolicyTemplate"].Rows)
            {
                DataRow dr = dt.NewRow();
                try
                {
                    if (string.IsNullOrEmpty(dataRow["航空公司"].ToString().Trim()))
                        continue;
                    try
                    {
                        #region MyRegion
                        dr["AirCompany"] = dataRow["航空公司"].ToString().Trim().ToUpper();
                        dr["TripType"] = dataRow["航程类型"].ToString().Trim().ToUpper();
                        dr["TeamOrLittle"] = dataRow["团散"].ToString().Trim().ToUpper();
                        dr["PassengerType"] = dataRow["乘客类型"].ToString().Trim().ToUpper();
                        dr["PolicyType"] = dataRow["政策类型"].ToString().Trim().ToUpper();
                        dr["DPT"] = dataRow["出港城市"].ToString().Trim().ToUpper();
                        dr["ARR"] = dataRow["到港城市"].ToString().Trim().ToUpper();
                        dr["NoDPT"] = dataRow["出港城市排除"].ToString().Trim().ToUpper();
                        dr["NOARR"] = dataRow["到港城市排除"].ToString().Trim().ToUpper();
                        dr["Cabins"] = dataRow["舱位"].ToString().Trim().ToUpper();
                        dr["PeersPoint"] = dataRow["同行返点"].ToString().Trim().ToUpper();
                        dr["SubordinatePoint"] = dataRow["下级返点"].ToString().Trim().ToUpper();
                        dr["FlightBeginDate"] = dataRow["航班开始日期"].ToString().Trim().ToUpper();
                        dr["FlightEndDate"] = dataRow["航班截止日期"].ToString().Trim().ToUpper();
                        dr["TicketBeginDate"] = dataRow["出票开始日期"].ToString().Trim().ToUpper();
                        dr["TicketEndDate"] = dataRow["出票截止日期"].ToString().Trim().ToUpper();
                        if (dataRow["是否自动出票"].ToString().Trim() == "是")
                            dr["IsAutoSellTicket"] = true;
                        else dr["IsAutoSellTicket"] = false;
                        if (dataRow["是否换PNR出票"].ToString().Trim() == "是")
                            dr["IsChangePNR"] = true;
                        else dr["IsChangePNR"] = false;
                        dr["OfficeNO"] = dataRow["授权office号"].ToString().Trim();

                        if (dataRow["PNR是否需要授权"].ToString().Trim() == "是")
                            dr["IsAuthorizedPNR"] = true;
                        else dr["IsAuthorizedPNR"] = false;
                        if (dataRow["共享航班是否适应"].ToString().Trim() == "是")
                            dr["IsShareFlightNo"] = true;
                        else dr["IsShareFlightNo"] = false;

                        dr["FlightNo"] = dataRow["适用航班号"].ToString().Trim().ToUpper();
                        dr["NoFlightNo"] = dataRow["不适用航班号"].ToString().Trim().ToUpper();
                        dr["FlightDate"] = dataRow["适用班期"].ToString().Trim().ToUpper();
                        dr["NoFlightDate"] = dataRow["不适用班期"].ToString().Trim().ToUpper();
                        dr["WorkingBegin"] = dataRow["工作开始时刻"].ToString().Trim().ToUpper();
                        dr["WorkingEnd"] = dataRow["工作结束时刻"].ToString().Trim().ToUpper();
                        dr["Memo"] = dataRow["备注"].ToString().Trim().ToUpper();
                        #endregion

                    }
                    catch (Exception ex)
                    {
                        err = ex.Message;
                    }

                }
                catch (Exception ex)
                {
                    oleDbConnection.Close();
                    oleDbConnection.Dispose();
                    oleAdapter.Dispose();
                    err = ex.Message;
                    throw;
                }
                try
                {
                    dt.Rows.Add(dr);
                }
                catch (Exception ex)
                {
                    oleDbConnection.Close();
                    oleDbConnection.Dispose();
                    oleAdapter.Dispose();
                    err = ex.Message;
                    throw;
                }

            }
            #endregion
            oleDbConnection.Close();
            oleDbConnection.Dispose();
            oleAdapter.Dispose();
            return dataSet.Tables["PolicyTemplate"];
        }
        public static void createExcel(string savepath, string fileName, System.Data.DataTable dtData)
        {
            createExcel(savepath, fileName, dtData, null);
        }

        public static void createExcel(string savefullpath, System.Data.DataTable dtData)
        {
            createExcel(Path.GetDirectoryName(savefullpath),
                Path.GetFileName(savefullpath),
                dtData,
                null
                );
        }

        /// <summary>
        /// 根据传入的DataTable对象，自动生成相应的Excel并输出
        /// </summary>
        /// <param name="savepath">文件保存的路径（不包含文件名）</param>
        /// <param name="fileName">Excel保存的文件名</param>
        /// <param name="dtData">需要导出为Excel文件的数据，DataTable格式（由于是导出为2003格式，因此导出数据的行列数收到2003格式的限制，
        /// 也就是不能超过65534行，254列。 
        /// 方法会自动识别DataTable的Column.Caption属性并作为默认的导出Excel后每列数据的标题，如果Caption属性为空，则使用Name属性。
        /// </param>
        /// <param name="title">导出Excel为表格样式，如果参数非空，则会将内容作为表格的总标题在第一行显示；否则第一行不显示。</param>
        public static void createExcel(string savepath, string fileName, System.Data.DataTable dtData, string title)
        {

            // 生成一个ExcelDoc的对象
            XlsDocument xls = new XlsDocument();
            // 设定文件名
            xls.FileName = fileName;
            // 设定Sheet名
            string sheetName = "sheet1";
            // 添加Sheet
            org.in2bits.MyXls.Worksheet sheet = xls.Workbook.Worksheets.Add(sheetName);
            // 创建单元格的集合
            Cells cells = sheet.Cells;
            // 创建扩展样式的单元格样式控制
            XF cellXF = xls.NewXF();
            cellXF.Pattern = 1;
            // 数据开始写入的起始行，如果有标题，那么起始行从2开始。
            int startRowNum = 1;

            if (!string.IsNullOrEmpty(title))
            {
                startRowNum = 2;
            }

            // 遍历当前数据源的列，为表格生成表头
            for (int c = 1; c <= dtData.Columns.Count; c++)
            {
                // 用扩展样式的单元格样式创建单元格
                string defaultColName = dtData.Columns[c - 1].Caption;
                // 如果列的Caption属性为空，则使用ColumnName属性作为导出的列
                if (string.IsNullOrEmpty(dtData.Columns[c - 1].Caption))
                {
                    defaultColName = dtData.Columns[c - 1].ColumnName;
                }
                Cell cellTemp = cells.Add(startRowNum, c, dtData.Columns[c - 1].ColumnName, cellXF);

                // 设定表头单元格的样式
                //cellTemp.HorizontalAlignment = HorizontalAlignments.Centered;
                //cellTemp.Font.Bold = true;
                //cellTemp.Font.FontFamily = FontFamilies.Default;
                //cellTemp.LeftLineStyle = (ushort)LineStyle.Thin;
                //cellTemp.RightLineStyle = (ushort)LineStyle.Thin;
                //cellTemp.BottomLineStyle = (ushort)LineStyle.Double;
                //cellTemp.TopLineStyle = (ushort)LineStyle.Thin;
                cellTemp.UseBackground = true;
                cellTemp.PatternColor = Colors.Grey;
            }

            // 遍历数据源的所有数据，生成数据区
            for (int r = startRowNum; r < dtData.Rows.Count + startRowNum && r <= 65534; r++)
            {
                for (int c = 0; c < dtData.Columns.Count; c++)
                {

                    Cell cell;

                    cell = cells.Add(r + 1, c + 1, dtData.Rows[r - startRowNum][c] == System.DBNull.Value ? "" : dtData.Rows[r - startRowNum][c]);

                    //cell.BottomLineColor = Colors.Black;
                    //cell.BottomLineStyle = (ushort)LineStyle.Thin;
                    //cell.LeftLineColor = Colors.Black;
                    //cell.LeftLineStyle = (ushort)LineStyle.Thin;
                    //cell.RightLineColor = Colors.Black;
                    //cell.RightLineStyle = (ushort)LineStyle.Thin;
                    //cell.FormulaHidden = true;
                    cell.HorizontalAlignment = HorizontalAlignments.Left;
                    // 为第一行数据的上部创建不同样式的线条
                    //if (r != startRowNum)
                    //{
                    //    cell.TopLineColor = Colors.Black;
                    //    cell.TopLineStyle = (ushort)LineStyle.Thin;
                    //}
                }

            }
            // 合并单元格，设定表格的标题
            if (startRowNum == 2)
            {
                Cell ce = sheet.Cells.Add(1, 1, title);
                ce.Font.Height = 400;
                ce.HorizontalAlignment = HorizontalAlignments.Centered;
                MergeArea ma = new MergeArea(1, 1, 1, dtData.Columns.Count);
                sheet.AddMergeArea(ma);
            }
            // 设定列宽
            ColumnInfo colInfo = new ColumnInfo(xls, sheet);//生成列格式对象
            colInfo.ColumnIndexStart = 1;//起始列为第二列
            colInfo.ColumnIndexEnd = (ushort)dtData.Columns.Count;//终止列为第六列
            colInfo.Width = 15 * 256;//列的宽度计量单位为 1/256 字符宽
            sheet.AddColumnInfo(colInfo);//把格式附加到sheet页上（注：AddColumnInfo方法有点小问题，不给把colInfo对象多次赋值给sheet页）

            xls.Save(savepath, true);
        }
        //Datatable导出Excel
        public static void GridToExcelByNPOI(System.Data.DataTable dt, string strExcelFileName)
        {
            HSSFWorkbook workbook = null;
            try
            {
                workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Sheet1");

                ICellStyle HeadercellStyle = workbook.CreateCellStyle();
                HeadercellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                HeadercellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                //字体
                NPOI.SS.UserModel.IFont headerfont = workbook.CreateFont();
                headerfont.Boldweight = (short)FontBoldWeight.Bold;
                HeadercellStyle.SetFont(headerfont);


                //用column name 作为列名
                int icolIndex = 0;
                IRow headerRow = sheet.CreateRow(0);
                foreach (DataColumn item in dt.Columns)
                {
                    ICell cell;
                    try
                    {
                        switch (item.DataType.Name)
                        {
                            case "Decimal":
                            case "Int32":
                            case "long":
                            case "Int64":
                            case "Int16":
                                cell = headerRow.CreateCell(icolIndex, CellType.Numeric);
                                break;
                            default:
                                cell = headerRow.CreateCell(icolIndex, CellType.String);
                                //cell.SetCellType(CellType.String);
                                break;
                        }
                        cell.SetCellValue(item.ColumnName);
                        cell.CellStyle = HeadercellStyle;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    icolIndex++;
                }

                ICellStyle cellStyle = workbook.CreateCellStyle();

                //为避免日期格式被Excel自动替换，所以设定 format 为 『@』 表示一率当成text來看
                //cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;


                NPOI.SS.UserModel.IFont cellfont = workbook.CreateFont();
                cellfont.Boldweight = (short)FontBoldWeight.Normal;
                cellStyle.SetFont(cellfont);

                //建立内容行
                int iRowIndex = 1;
                int iCellIndex = 0;
                foreach (DataRow Rowitem in dt.Rows)
                {
                    IRow DataRow = sheet.CreateRow(iRowIndex);
                    foreach (DataColumn Colitem in dt.Columns)
                    {
                        ICell cell = DataRow.CreateCell(iCellIndex);
                        cell.CellStyle = cellStyle;
                        try
                        {
                            switch (Colitem.DataType.Name)
                            {
                                case "String":
                                    cell.SetCellType(CellType.String);
                                    cell.SetCellValue(Rowitem[Colitem].ToString());
                                    break;
                                case "Decimal":
                                case "Int32":
                                case "long":
                                case "Int64":
                                case "Int16":
                                    double v = 0;
                                    double.TryParse(Rowitem[Colitem].ToString(), out v);
                                    cell.SetCellType(CellType.Numeric);
                                    cell.SetCellValue(v);
                                    //cell.CellFormula = "0+"+Rowitem[Colitem].ToString();
                                    break;
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        iCellIndex++;
                    }
                    iCellIndex = 0;
                    iRowIndex++;
                }

                //自适应列宽度
                for (int i = 0; i < icolIndex; i++)
                {
                    sheet.AutoSizeColumn(i);
                }

                //写Excel
                FileStream file = new FileStream(strExcelFileName, FileMode.OpenOrCreate);
                workbook.Write(file);
                file.Flush();
                file.Close();

                //MessageBox.Show(m_Common_ResourceManager.GetString("Export_to_excel_successfully"), m_Common_ResourceManager.GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //ILog log = LogManager.GetLogger("Exception Log");
                //log.Error(ex.Message + Environment.NewLine + ex.StackTrace);
                ////记录AuditTrail
                //CCFS.Framework.BLL.AuditTrailBLL.LogAuditTrail(ex);

                //MessageBox.Show(m_Common_ResourceManager.GetString("Export_to_excel_failed"), m_Common_ResourceManager.GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally { workbook = null; }

        }

        public static System.Data.DataTable ImportExcelFile(string filePath, out string err)
        {
            err = "";
            IWorkbook hssfworkbook;
            #region//初始化信息
            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new XSSFWorkbook(file);
                }
            }
            catch (Exception e)
            {
                err = e.Message;
                return null;
            }
            #endregion

            ISheet sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            System.Data.DataTable dt = new System.Data.DataTable();

            //一行最后一个方格的编号 即总的列数
            for (int j = 0; j < (sheet.GetRow(0).LastCellNum); j++)
            {
                dt.Columns.Add(Convert.ToChar(((int)'A') + j).ToString());
            }

            while (rows.MoveNext())
            {
                IRow row = (XSSFRow)rows.Current;
                DataRow dr = dt.NewRow();

                for (int i = 0; i < row.LastCellNum; i++)
                {
                    ICell cell = row.GetCell(i);


                    if (cell == null)
                    {
                        dr[i] = null;
                    }
                    else
                    {
                        dr[i] = cell.ToString();
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public static void PolicyTemplateTable(System.Data.DataTable dt, DataRow datarow, out string err)
        {
            err = "";
            System.Data.DataTable dt_tmp = datarow.Table;
            foreach (DataRow dataRow in dt.Rows)
            {
                DataRow dr = dt_tmp.NewRow();
                if (string.IsNullOrEmpty(dataRow["航空公司"].ToString().Trim()))
                    continue;
                try
                {
                    #region MyRegion
                    dr["AirCompany"] = dataRow["航空公司"].ToString().Trim().ToUpper();
                    dr["TripType"] = dataRow["航程类型"].ToString().Trim().ToUpper();
                    dr["TeamOrLittle"] = dataRow["团散"].ToString().Trim().ToUpper();
                    dr["PassengerType"] = dataRow["乘客类型"].ToString().Trim().ToUpper();
                    dr["PolicyType"] = dataRow["政策类型"].ToString().Trim().ToUpper();
                    dr["DPT"] = dataRow["出港城市"].ToString().Trim().ToUpper();
                    dr["ARR"] = dataRow["到港城市"].ToString().Trim().ToUpper();
                    dr["NoDPT"] = dataRow["出港城市排除"].ToString().Trim().ToUpper();
                    dr["NOARR"] = dataRow["到港城市排除"].ToString().Trim().ToUpper();
                    dr["Cabins"] = dataRow["舱位"].ToString().Trim().ToUpper();
                    dr["PeersPoint"] = dataRow["同行返点"].ToString().Trim().ToUpper();
                    dr["SubordinatePoint"] = dataRow["下级返点"].ToString().Trim().ToUpper();
                    dr["FlightBeginDate"] = dataRow["航班开始日期"].ToString().Trim().ToUpper();
                    dr["FlightEndDate"] = dataRow["航班截止日期"].ToString().Trim().ToUpper();
                    dr["TicketBeginDate"] = dataRow["出票开始日期"].ToString().Trim().ToUpper();
                    dr["TicketEndDate"] = dataRow["出票截止日期"].ToString().Trim().ToUpper();
                    if (dataRow["是否自动出票"].ToString().Trim() == "是")
                        dr["IsAutoSellTicket"] = true;
                    else dr["IsAutoSellTicket"] = false;
                    if (dataRow["是否换PNR出票"].ToString().Trim() == "是")
                        dr["IsChangePNR"] = true;
                    else dr["IsChangePNR"] = false;
                    dr["OfficeNO"] = dataRow["授权office号"].ToString().Trim();

                    if (dataRow["PNR是否需要授权"].ToString().Trim() == "是")
                        dr["IsAuthorizedPNR"] = true;
                    else dr["IsAuthorizedPNR"] = false;
                    if (dataRow["共享航班是否适应"].ToString().Trim() == "是")
                        dr["IsShareFlightNo"] = true;
                    else dr["IsShareFlightNo"] = false;

                    dr["FlightNo"] = dataRow["适用航班号"].ToString().Trim().ToUpper();
                    dr["NoFlightNo"] = dataRow["不适用航班号"].ToString().Trim().ToUpper();
                    dr["FlightDate"] = dataRow["适用班期"].ToString().Trim().ToUpper();
                    dr["NoFlightDate"] = dataRow["不适用班期"].ToString().Trim().ToUpper();
                    dr["WorkingBegin"] = dataRow["工作开始时刻"].ToString().Trim().ToUpper();
                    dr["WorkingEnd"] = dataRow["工作结束时刻"].ToString().Trim().ToUpper();
                    dr["Memo"] = dataRow["备注"].ToString().Trim().ToUpper();
                    dt_tmp.Rows.Add(dr);
                    #endregion

                }
                catch (Exception ex)
                {
                    err = ex.Message;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="excelpath"></param>
        /// <returns></returns>
        public static System.Data.DataTable ExcelTableAdditional(string excelpath, DataRow datarow, out string err)
        {
            err = "";
            string connExcel = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + excelpath + ";Extended Properties=Excel 8.0;";
            System.Data.DataTable dt = datarow.Table;

            OleDbConnection oleDbConnection = new OleDbConnection(connExcel);
            oleDbConnection.Open();
            System.Data.DataTable dataTable = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            //获取sheet名，其中[0][1]...[N]: 按名称排列的表单元素
            string tableName = dataTable.Rows[0][2].ToString().Trim();
            tableName = "[" + tableName.Replace("'", "") + "]";

            string query = "SELECT [航空公司],[航程类型],[团散],[乘客类型],[政策类型],[出港城市],[到港城市],[出港城市排除],[到港城市排除],[舱位],[同行返点],[下级返点],[航班开始日期],[航班截止日期],[出票开始日期],[出票截止日期],[是否自动出票],[是否换PNR出票],[授权office号],[PNR是否需要授权],[共享航班是否适应],[适用航班号],[不适用航班号],[适用班期],[不适用班期],[工作开始时刻],[工作结束时刻],[备注],[贴点],[赔点],[最小值],[固定点],[超点] FROM " + tableName;
            //string query = "SELECT * FROM " + tableName;
            DataSet dataSet = new DataSet();
            OleDbDataAdapter oleAdapter = new OleDbDataAdapter(query, connExcel);
            try
            {
                oleAdapter.Fill(dataSet, "PolicyTemplate");
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return null;
            }

            #region 逐行读取数据
            foreach (DataRow dataRow in dataSet.Tables["PolicyTemplate"].Rows)
            {
                DataRow dr = dt.NewRow();
                try
                {
                    if (string.IsNullOrEmpty(dataRow["航空公司"].ToString().Trim()))
                        continue;
                    try
                    {
                        #region MyRegion
                        dr["AirCompany"] = dataRow["航空公司"].ToString().Trim().ToUpper();
                        dr["TripType"] = dataRow["航程类型"].ToString().Trim().ToUpper();
                        dr["TeamOrLittle"] = dataRow["团散"].ToString().Trim().ToUpper();
                        dr["PassengerType"] = dataRow["乘客类型"].ToString().Trim().ToUpper();
                        dr["PolicyType"] = dataRow["政策类型"].ToString().Trim().ToUpper();
                        dr["DPT"] = dataRow["出港城市"].ToString().Trim().ToUpper();
                        dr["ARR"] = dataRow["到港城市"].ToString().Trim().ToUpper();
                        dr["NoDPT"] = dataRow["出港城市排除"].ToString().Trim().ToUpper();
                        dr["NOARR"] = dataRow["到港城市排除"].ToString().Trim().ToUpper();
                        dr["Cabins"] = dataRow["舱位"].ToString().Trim().ToUpper();
                        dr["PeersPoint"] = dataRow["同行返点"].ToString().Trim().ToUpper();
                        dr["SubordinatePoint"] = dataRow["下级返点"].ToString().Trim().ToUpper();
                        dr["FlightBeginDate"] = dataRow["航班开始日期"].ToString().Trim().ToUpper();
                        dr["FlightEndDate"] = dataRow["航班截止日期"].ToString().Trim().ToUpper();
                        dr["TicketBeginDate"] = dataRow["出票开始日期"].ToString().Trim().ToUpper();
                        dr["TicketEndDate"] = dataRow["出票截止日期"].ToString().Trim().ToUpper();
                        if (dataRow["是否自动出票"].ToString().Trim() == "是")
                            dr["IsAutoSellTicket"] = true;
                        else dr["IsAutoSellTicket"] = false;
                        if (dataRow["是否换PNR出票"].ToString().Trim() == "是")
                            dr["IsChangePNR"] = true;
                        else dr["IsChangePNR"] = false;
                        dr["OfficeNO"] = dataRow["授权office号"].ToString().Trim();

                        if (dataRow["PNR是否需要授权"].ToString().Trim() == "是")
                            dr["IsAuthorizedPNR"] = true;
                        else dr["IsAuthorizedPNR"] = false;
                        if (dataRow["共享航班是否适应"].ToString().Trim() == "是")
                            dr["IsShareFlightNo"] = true;
                        else dr["IsShareFlightNo"] = false;

                        dr["FlightNo"] = dataRow["适用航班号"].ToString().Trim().ToUpper();
                        dr["NoFlightNo"] = dataRow["不适用航班号"].ToString().Trim().ToUpper();
                        dr["FlightDate"] = dataRow["适用班期"].ToString().Trim().ToUpper();
                        dr["NoFlightDate"] = dataRow["不适用班期"].ToString().Trim().ToUpper();
                        dr["WorkingBegin"] = dataRow["工作开始时刻"].ToString().Trim().ToUpper();
                        dr["WorkingEnd"] = dataRow["工作结束时刻"].ToString().Trim().ToUpper();
                        dr["Memo"] = dataRow["备注"].ToString().Trim().ToUpper(); double t = 0.0d;
                        double p = 0.0d;
                        double min = 0.0d;
                        double g = 0.0d;
                        double app = 0.0d;
                        double o = 0.0d;
                        double.TryParse(dataRow["贴点"].ToString().Trim(), out app);
                        double.TryParse(dataRow["同行返点"].ToString().Trim(), out t);
                        double.TryParse(dataRow["赔点"].ToString().Trim(), out p);
                        double.TryParse(dataRow["最小值"].ToString().Trim(), out min);
                        double.TryParse(dataRow["固定点"].ToString().Trim(), out g);
                        double.TryParse(dataRow["超点"].ToString().Trim(), out o);
                        dr["MaxValue"] = t + p;
                        dr["MinValue"] = min;
                        dr["GuDingPoint"] = g;
                        dr["AppendPoint"] = app;
                        dr["OutNumber"] = o;
                        dr["Rebate"] = p;
                        #endregion

                    }
                    catch (Exception ex)
                    {
                        err = ex.Message;
                    }

                }
                catch (Exception ex)
                {
                    oleDbConnection.Close();
                    oleDbConnection.Dispose();
                    oleAdapter.Dispose();
                    err = ex.Message;
                    throw;
                }
                try
                {
                    dt.Rows.Add(dr);
                }
                catch (Exception ex)
                {
                    oleDbConnection.Close();
                    oleDbConnection.Dispose();
                    oleAdapter.Dispose();
                    err = ex.Message;
                    throw;
                }

            }
            #endregion
            oleDbConnection.Close();
            oleDbConnection.Dispose();
            oleAdapter.Dispose();
            return dataSet.Tables["PolicyTemplate"];
        }
        public static void PolicyAdditionalTable(System.Data.DataTable dt, DataRow datarow, out string err)
        {
            err = "";
            System.Data.DataTable dt_tmp = datarow.Table;
            foreach (DataRow dataRow in dt.Rows)
            {
                DataRow dr = dt_tmp.NewRow();
                if (string.IsNullOrEmpty(dataRow["航空公司"].ToString().Trim()))
                    continue;
                try
                {
                    #region MyRegion
                    dr["AirCompany"] = dataRow["航空公司"].ToString().Trim().ToUpper();
                    dr["TripType"] = dataRow["航程类型"].ToString().Trim().ToUpper();
                    dr["TeamOrLittle"] = dataRow["团散"].ToString().Trim().ToUpper();
                    dr["PassengerType"] = dataRow["乘客类型"].ToString().Trim().ToUpper();
                    dr["PolicyType"] = dataRow["政策类型"].ToString().Trim().ToUpper();
                    dr["DPT"] = dataRow["出港城市"].ToString().Trim().ToUpper();
                    dr["ARR"] = dataRow["到港城市"].ToString().Trim().ToUpper();
                    dr["NoDPT"] = dataRow["出港城市排除"].ToString().Trim().ToUpper();
                    dr["NOARR"] = dataRow["到港城市排除"].ToString().Trim().ToUpper();
                    dr["Cabins"] = dataRow["舱位"].ToString().Trim().ToUpper();
                    dr["PeersPoint"] = dataRow["同行返点"].ToString().Trim().ToUpper();
                    dr["SubordinatePoint"] = dataRow["下级返点"].ToString().Trim().ToUpper();
                    dr["FlightBeginDate"] = dataRow["航班开始日期"].ToString().Trim().ToUpper();
                    dr["FlightEndDate"] = dataRow["航班截止日期"].ToString().Trim().ToUpper();
                    dr["TicketBeginDate"] = dataRow["出票开始日期"].ToString().Trim().ToUpper();
                    dr["TicketEndDate"] = dataRow["出票截止日期"].ToString().Trim().ToUpper();
                    if (dataRow["是否自动出票"].ToString().Trim() == "是")
                        dr["IsAutoSellTicket"] = true;
                    else dr["IsAutoSellTicket"] = false;
                    if (dataRow["是否换PNR出票"].ToString().Trim() == "是")
                        dr["IsChangePNR"] = true;
                    else dr["IsChangePNR"] = false;
                    dr["OfficeNO"] = dataRow["授权office号"].ToString().Trim();

                    if (dataRow["PNR是否需要授权"].ToString().Trim() == "是")
                        dr["IsAuthorizedPNR"] = true;
                    else dr["IsAuthorizedPNR"] = false;
                    if (dataRow["共享航班是否适应"].ToString().Trim() == "是")
                        dr["IsShareFlightNo"] = true;
                    else dr["IsShareFlightNo"] = false;

                    dr["FlightNo"] = dataRow["适用航班号"].ToString().Trim().ToUpper();
                    dr["NoFlightNo"] = dataRow["不适用航班号"].ToString().Trim().ToUpper();
                    dr["FlightDate"] = dataRow["适用班期"].ToString().Trim().ToUpper();
                    dr["NoFlightDate"] = dataRow["不适用班期"].ToString().Trim().ToUpper();
                    dr["WorkingBegin"] = dataRow["工作开始时刻"].ToString().Trim().ToUpper();
                    dr["WorkingEnd"] = dataRow["工作结束时刻"].ToString().Trim().ToUpper();
                    dr["Memo"] = dataRow["备注"].ToString().Trim().ToUpper();

                    dr["PeersPoint"] = dataRow["同行返点"].ToString().Trim();
                    dr["Rebate"] = dataRow["赔点"].ToString().Trim();
                    double t = 0.0d;
                    double p = 0.0d;
                    double min = 0.0d;
                    double g = 0.0d;
                    double app = 0.0d;
                    double o = 0.0d;
                    //double.TryParse(dataRow["贴点"].ToString().Trim(),out app);
                    double.TryParse(dataRow["同行返点"].ToString().Trim(),out t);
                    double.TryParse(dataRow["赔点"].ToString().Trim(), out p);
                    double.TryParse(dataRow["最小值"].ToString().Trim(), out min);
                    double.TryParse(dataRow["固定点"].ToString().Trim(),out g);
                    double.TryParse(dataRow["超点"].ToString().Trim(),out o);
                    dr["MaxValue"] = t + p;
                    dr["MinValue"] = min;
                    dr["GuDingPoint"] = g;
                    dr["AppendPoint"] = app;
                    dr["OutNumber"]=o;
                    dt_tmp.Rows.Add(dr);
                    #endregion

                }
                catch (Exception ex)
                {
                    err = ex.Message;
                }
            }
        }
        #region Excel2003
        /// <summary>
        /// 将Excel文件中的数据读出到DataTable中(xls)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static System.Data.DataTable ExcelToTableForXLS(string file, out string err)
        {
            err = "";
            System.Data.DataTable dt = new System.Data.DataTable();
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                HSSFWorkbook hssfworkbook = new HSSFWorkbook(fs);
                ISheet sheet = hssfworkbook.GetSheetAt(0);

                //表头
                IRow header = sheet.GetRow(sheet.FirstRowNum);
                List<int> columns = new List<int>();
                for (int i = 0; i < header.LastCellNum; i++)
                {
                    object obj = GetValueTypeForXLS(header.GetCell(i) as HSSFCell);
                    if (obj == null || obj.ToString() == string.Empty)
                    {
                        dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                        //continue;
                    }
                    else
                        dt.Columns.Add(new DataColumn(obj.ToString()));
                    columns.Add(i);
                }
                //数据
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                {
                    DataRow dr = dt.NewRow();
                    bool hasValue = false;
                    foreach (int j in columns)
                    {

                        try
                        {
                            dr[j] = GetValueTypeForXLS(sheet.GetRow(i).GetCell(j) as HSSFCell);
                        }
                        catch (Exception ex)
                        {
                            err = ex.Message;
                            continue;
                        }
                        ICell cell = sheet.GetRow(i).GetCell(j);

                        if (cell != null && CellType.Numeric == cell.CellType)
                            if (HSSFDateUtil.IsCellDateFormatted(cell))//日期类型
                            {
                                dr[j] = cell.DateCellValue;
                            }
                            else//其他数字类型
                            {
                                dr[j] = cell.NumericCellValue;
                            }

                        if (dr[j] != null && dr[j].ToString() != string.Empty)
                        {
                            hasValue = true;
                        }
                    }
                    if (hasValue)
                    {
                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// 将DataTable数据导出到Excel文件中(xls)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="file"></param>
        public static void TableToExcelForXLS(System.Data.DataTable dt, string file)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            ISheet sheet = hssfworkbook.CreateSheet("Test");

            //表头
            IRow row = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(dt.Columns[i].ColumnName);
            }

            //数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row1 = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());
                }
            }

            //转为字节数组
            MemoryStream stream = new MemoryStream();
            hssfworkbook.Write(stream);
            var buf = stream.ToArray();

            //保存为Excel文件
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }
        }

        /// <summary>
        /// 获取单元格类型(xls)
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static object GetValueTypeForXLS(HSSFCell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:
                    return null;
                case CellType.Boolean: //BOOLEAN:
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:
                    return cell.NumericCellValue;
                case CellType.String: //STRING:
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:
                default:
                    return "=" + cell.CellFormula;
            }
        }
        #endregion
        #region Excel2007
        /// <summary>
        /// 将Excel文件中的数据读出到DataTable中(xlsx)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static System.Data.DataTable ExcelToTableForXLSX(string file)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                XSSFWorkbook xssfworkbook = new XSSFWorkbook(fs);
                ISheet sheet = xssfworkbook.GetSheetAt(0);

                //表头
                IRow header = sheet.GetRow(sheet.FirstRowNum);
                List<int> columns = new List<int>();
                for (int i = 0; i < header.LastCellNum; i++)
                {
                    object obj = GetValueTypeForXLSX(header.GetCell(i) as XSSFCell);
                    if (obj == null || obj.ToString() == string.Empty)
                    {
                        dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                        //continue;
                    }
                    else
                        dt.Columns.Add(new DataColumn(obj.ToString()));
                    columns.Add(i);
                }
                //数据
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                {
                    DataRow dr = dt.NewRow();
                    bool hasValue = false;
                    foreach (int j in columns)
                    {
                        dr[j] = GetValueTypeForXLSX(sheet.GetRow(i).GetCell(j) as XSSFCell);
                        if (dr[j] != null && dr[j].ToString() != string.Empty)
                        {
                            hasValue = true;
                        }
                    }
                    if (hasValue)
                    {
                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// 将DataTable数据导出到Excel文件中(xlsx)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="file"></param>
        public static void TableToExcelForXLSX(System.Data.DataTable dt, string file)
        {
            XSSFWorkbook xssfworkbook = new XSSFWorkbook();
            ISheet sheet = xssfworkbook.CreateSheet("Test");

            //表头
            IRow row = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(dt.Columns[i].ColumnName);
            }

            //数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row1 = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ICell cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());
                }
            }

            //转为字节数组
            MemoryStream stream = new MemoryStream();
            xssfworkbook.Write(stream);
            var buf = stream.ToArray();

            //保存为Excel文件
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }
        }

        /// <summary>
        /// 获取单元格类型(xlsx)
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static object GetValueTypeForXLSX(XSSFCell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:
                    return null;
                case CellType.Boolean: //BOOLEAN:
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:
                    return cell.NumericCellValue;
                case CellType.String: //STRING:
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:
                default:
                    return "=" + cell.CellFormula;
            }
        }
        #endregion
        public static System.Data.DataTable GetDataTable(string filepath, out string err)
        {
            err = "";
            var dt = new System.Data.DataTable("xls");
            try
            {
                if (filepath.Last() == 's')
                {
                    dt = ExcelToTableForXLS(filepath, out err);
                }
                else
                {
                    dt = ExcelToTableForXLSX(filepath);
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return dt;
            }
            return dt;
        }

        public static void InteropExportExcel(System.Data.DataTable dt, string path, out string err)
        {
            err = "";
            //C:\Program Files\Microsoft Office\OFFICE11
            //WorkbookClass _WorkbookClass = new WorkbookClass();
          
            if (dt != null)
            {
                Microsoft.Office.Interop.Excel.Application xlApp = null;
                try
                {
                    xlApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
                }
                catch (Exception ex)
                {
                    err = ex.Message;
                    return;
                }

                if (xlApp != null)
                {
                    try
                    {
                        Microsoft.Office.Interop.Excel.Workbook xlBook = xlApp.Workbooks.Add(true);
                        object oMissing = System.Reflection.Missing.Value;
                        Microsoft.Office.Interop.Excel.Worksheet xlSheet = null;

                        xlSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets[1];
                        if (!string.IsNullOrEmpty(dt.TableName))
                            xlSheet.Name = dt.TableName;

                        int rowIndex = 1;
                        int colIndex = 1;
                        int colCount = dt.Columns.Count;
                        int rowCount = dt.Rows.Count;

                        //列名的处理
                        for (int i = 0; i < colCount; i++)
                        {
                            xlSheet.Cells[rowIndex, colIndex] = dt.Columns[i].ColumnName;
                            colIndex++;
                        }
                        //列名加粗显示
                        xlSheet.get_Range(xlSheet.Cells[rowIndex, 1], xlSheet.Cells[rowIndex, colCount]).Font.Bold = true;
                        xlSheet.get_Range(xlSheet.Cells[rowIndex, 1], xlSheet.Cells[rowCount + 1, colCount]).Font.Name = "Arial";
                        xlSheet.get_Range(xlSheet.Cells[rowIndex, 1], xlSheet.Cells[rowCount + 1, colCount]).Font.Size = "10";
                        rowIndex++;

                        for (int i = 0; i < rowCount; i++)
                        {
                            colIndex = 1;
                            for (int j = 0; j < colCount; j++)
                            {
                                xlSheet.Cells[rowIndex, colIndex] = dt.Rows[i][j].ToString();
                                colIndex++;
                            }
                            rowIndex++;
                        }
                        xlSheet.Cells.EntireColumn.AutoFit();

                        xlApp.DisplayAlerts = false;
                        path = Path.GetFullPath(path);
                        xlBook.SaveCopyAs(path);
                        xlBook.Close(false, null, null);
                        xlApp.Workbooks.Close();
                        Marshal.ReleaseComObject(xlSheet);
                        Marshal.ReleaseComObject(xlBook);
                        xlBook = null;
                    }
                    catch (Exception ex)
                    {
                        err = ex.Message;
                        return;
                    }
                    finally
                    {
                        xlApp.Quit();
                        Marshal.ReleaseComObject(xlApp);
                        int generation = System.GC.GetGeneration(xlApp);
                        xlApp = null;
                        System.GC.Collect(generation);
                    }
                }
            }
        }

        /// <summary>
        /// <summary>
        /// DataTable导出到Excel
        /// </summary>
        /// <param name="fileName">默认的文件名</param>
        /// <param name="dataTable">数据源,一个DataTable数据表</param>
        /// <param name="titleRowCount">标题占据的行数，为0则表示无标题</param>
        /// </summary> 
        public static void InteropExportExcel(System.Data.DataTable dataTable,string fileName,  out string err, int titleRowCount = 0)
        {
            err = ""; 
            //bool fileSaved = false;
            
            Microsoft.Office.Interop.Excel.Application xlApp;
            try
            {
                xlApp = new Microsoft.Office.Interop.Excel.Application();
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return;
            }
            finally
            {
            }

            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];//取得sheet1
            //写Title
            if (titleRowCount != 0)
                MergeCells(worksheet, 1, 1, titleRowCount, dataTable.Columns.Count, dataTable.TableName);

            //写入列标题
            for (int i = 0; i <= dataTable.Columns.Count - 1; i++)
            {
                worksheet.Cells[titleRowCount + 1, i + 1] = dataTable.Columns[i].ColumnName;

            }
            //写入数值
            for (int r = 0; r <= dataTable.Rows.Count - 1; r++)
            {
                for (int i = 0; i <= dataTable.Columns.Count - 1; i++)
                {
                    worksheet.Cells[r + titleRowCount + 2, i + 1] = dataTable.Rows[r][i].ToString();
                }
                System.Windows.Forms.Application.DoEvents();
            }

            worksheet.Columns.EntireColumn.AutoFit();//列宽自适应 
            if (fileName != "")
            {
                try
                {
                    workbook.Saved = true;
                    workbook.SaveCopyAs(fileName); 
                }
                catch (Exception ex)
                {
                    err = ex.Message;
                    return;                     
                }
            } 
            xlApp.Quit();
            GC.Collect();//强行销毁              
        }
        /// <summary>   
        /// 合并单元格，并赋值，对指定WorkSheet操作   
        /// </summary>   
        /// <param name="sheetIndex">WorkSheet索引</param>   
        /// <param name="beginRowIndex">开始行索引</param>   
        /// <param name="beginColumnIndex">开始列索引</param>   
        /// <param name="endRowIndex">结束行索引</param>   
        /// <param name="endColumnIndex">结束列索引</param>   
        /// <param name="text">合并后Range的值</param>   
        public static void MergeCells(Microsoft.Office.Interop.Excel.Worksheet workSheet, int beginRowIndex, int beginColumnIndex, int endRowIndex, int endColumnIndex, string text)
        {
            Microsoft.Office.Interop.Excel.Range range = workSheet.get_Range(workSheet.Cells[beginRowIndex, beginColumnIndex], workSheet.Cells[endRowIndex, endColumnIndex]);

            range.ClearContents();  //先把Range内容清除，合并才不会出错   
            range.MergeCells = true;
            range.Value2 = text;
            range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
        } 
    }

    enum MonthsParse
    {

        NONE = 00,
        JAN = 01,
        FEB = 02,
        MAR = 03,
        APR = 04,
        MAY = 05,
        JUN = 06,
        JUL = 07,
        AUG = 08,
        SEP = 09,
        OCT = 10,
        NOV = 11,
        DEC = 12,

    }
}


