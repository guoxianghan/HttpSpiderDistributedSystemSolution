//1.添加引用：

//Aspose.Cells.dll（我们就叫工具包吧，可以从网上下载。关于它的操作我在“Aspose.Cells操作说明 中文版 下载 Aspose C# 导出Excel 实例”一文中的说。这里你暂时也可不理会它。）
//即使没有安装office也能用噢，这是一个好强的大工具。
//2.编写Excel操作类

using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Cells;
using System.Data;
namespace CommonHelper
{
    public class AsposeExcel
    {
        static void AsposeToExcel()
        {
            Workbook workbook = new Workbook(); //工作簿 
            Worksheet sheet = workbook.Worksheets[0]; //工作表 
            Cells cells = sheet.Cells;//单元格 

            Style style = workbook.Styles[workbook.Styles.Add()];//新增样式
            #region 表头
            //标题 
            style.HorizontalAlignment = TextAlignmentType.Center;//文字居中  
            style.Font.Name = "宋体";//文字字体 
            style.Font.Size = 18;//文字大小  
            style.Font.IsBold = true;//粗体
            cells.Merge(0, 0, 1, 12);               //合并单元格 
            cells[0, 0].PutValue("");   //填写内容 
            cells[0, 0].SetStyle(style);            //给单元格关联样式  
            cells.SetRowHeight(0, 28);              //设置行高 

            //发布时间
            style.HorizontalAlignment = TextAlignmentType.Left;
            style.Font.Size = 11;
            style.Font.IsBold = false;
            cells.Merge(1, 0, 1, 7);
            cells[1, 0].PutValue(string.Format("发布起止时间：{0}至{1},datetime.now.adddays(-1).tostring(yyyy年mm月dd日),datetime.now.tostring(yyyy年mm月dd日))"));
            cells[1, 0].SetStyle(style);
            cells.SetRowHeight(1, 20);
            //统计时间
            style.HorizontalAlignment = TextAlignmentType.Right;
            style.Font.Size = 11;
            style.Font.IsBold = false;
            cells.Merge(1, 7, 1, 5);
            cells[1, 7].PutValue(string.Format("统计时间:{0}, datetime.now.tostring(yyyy年mm月dd日))"));
            cells[1, 7].SetStyle(style);
            cells.SetRowHeight(1, 20);
            #endregion
            #region 表格
            #region 表格标题行
            //序号
            style.HorizontalAlignment = TextAlignmentType.Center;
            cells[2, 0].PutValue("");
            cells[2, 0].SetStyle(style);
            cells.SetRowHeight(2, 20);
            cells.SetColumnWidthPixel(0, 38);
            //建议时间
            cells[2, 1].PutValue("");
            cells[2, 1].SetStyle(style);
            cells.SetColumnWidthPixel(1, 77);
            //建议部门
            cells[2, 2].PutValue("");
            cells[2, 2].SetStyle(style);
            cells.SetColumnWidthPixel(2, 107);
            //建 议 人
            cells[2, 3].PutValue("");
            cells[2, 3].SetStyle(style);
            cells.SetColumnWidthPixel(3, 69);
            //类   别
            cells[2, 4].PutValue("");
            cells[2, 4].SetStyle(style);
            cells.SetColumnWidthPixel(4, 71);
            //业务种类
            cells[2, 5].PutValue("");
            cells[2, 5].SetStyle(style);
            cells.SetColumnWidthPixel(5, 71);
            //标准名称
            cells[2, 6].PutValue("");
            cells[2, 6].SetStyle(style);
            cells.SetColumnWidthPixel(6, 114);
            //标准章、条编号
            cells[2, 7].PutValue("");
            cells[2, 7].SetStyle(style);
            cells.SetColumnWidthPixel(7, 104);
            //意见建议
            cells[2, 8].PutValue("");
            cells[2, 8].SetStyle(style);
            cells.SetColumnWidthPixel(8, 255);
            //处理部门
            cells[2, 9].PutValue("");
            cells[2, 9].SetStyle(style);
            cells.SetColumnWidthPixel(9, 72);
            //处理进度
            cells[2, 10].PutValue("");
            cells[2, 10].SetStyle(style);
            cells.SetColumnWidthPixel(10, 72);
            //备注
            cells[2, 11].PutValue("");
            cells[2, 11].SetStyle(style);
            cells.SetColumnWidthPixel(11, 255);
            #endregion
            #endregion

            System.IO.MemoryStream ms = workbook.SaveToStream();//生成数据流 
            byte[] bt = ms.ToArray();
            workbook.Save(@"e:\test.xls");//保存到硬盘 
        }

        /// <summary>
        /// 导出EXCEL
        /// </summary>
        /// <param name="dsExcel">要保存为EXCEL的数据集,可包含多个Table,每个Tabel对应生成一个Sheet页 </param>
        /// <param name="outFileName"> 文件名  </param>
        /// <param name="savePath">文件保存路径</param>
        public static void ExportExcel(DataSet dsExcel, string outFileName, string savePath)
        {
            //创建一个workbook和worksheet对象
            Worksheet wkSheet = null;
            Workbook wkBook = new Workbook();
            wkBook.Worksheets.Clear();
            //遍历DataSet
            for (int i = 0; i < dsExcel.Tables.Count; i++)
            {
                //获取DataTable
                DataTable table = dsExcel.Tables[i];
                //创建一个worksheet
                wkBook.Worksheets.Add(table.TableName);
                //获取worksheet
                wkSheet = wkBook.Worksheets[i];
                //遍历Table的行
                for (int rNum = 0; rNum < table.Rows.Count; rNum++)
                {
                    #region 遍历Table的列
                    for (int cNum = 0; cNum < table.Columns.Count; cNum++)
                    {
                        //给sheet写入标头,只需执行一次
                        if (rNum == 0)
                        {
                            string _columnName = table.Columns[cNum].ColumnName.ToString().Trim();
                            //如果DataTable列名包含Column，则表示此列名是由系统自动生成，导出时将列名还原为空值
                            if (_columnName.Contains("Column"))
                            {
                                _columnName = "";
                            }
                            wkSheet.Cells[0, cNum].PutValue(_columnName);
                        }
                        //给sheet写入数据
                        wkSheet.Cells[rNum + 1, cNum].PutValue(table.Rows[rNum][cNum].ToString().Trim());
                    }
                    #endregion
                }
            }
            //导出保存
            wkBook.Save(savePath + "\\" + outFileName, FileFormatType.Excel2003);
            //释放对象
            wkSheet = null;
            wkBook = null;
        }

        /// <summary>
        /// 导出EXCEL
        /// </summary>
        /// <param name="dsExcel">要保存为EXCEL的数据集,可包含多个Table,每个Tabel对应生成一个Sheet页 </param>
        /// <param name="outFileName"> 文件名  </param>
        /// <param name="savePath">文件保存路径</param>
        public static void ExportExcel(DataTable table, string savePath)
        {
            //创建一个workbook和worksheet对象
            Worksheet wkSheet = null;
            Workbook wkBook = new Workbook();
            wkBook.Worksheets.Clear(); 
            //创建一个worksheet
            wkBook.Worksheets.Add(table.TableName);
            //获取worksheet
            wkSheet = wkBook.Worksheets[0];
            //遍历Table的行
            for (int rNum = 0; rNum < table.Rows.Count; rNum++)
            {
                #region 遍历Table的列
                for (int cNum = 0; cNum < table.Columns.Count; cNum++)
                {
                    //给sheet写入标头,只需执行一次
                    if (rNum == 0)
                    {
                        string _columnName = table.Columns[cNum].ColumnName.ToString().Trim();
                        //如果DataTable列名包含Column，则表示此列名是由系统自动生成，导出时将列名还原为空值
                        if (_columnName.Contains("Column"))
                        {
                            _columnName = "";
                        }
                        wkSheet.Cells[0, cNum].PutValue(_columnName);
                    }
                    //给sheet写入数据
                    wkSheet.Cells[rNum + 1, cNum].PutValue(table.Rows[rNum][cNum].ToString().Trim());
                }
                #endregion
            }
            //导出保存
            wkBook.Save(savePath, FileFormatType.Excel2003);
            //释放对象
            wkSheet = null;
            wkBook = null;
        }
    }

}