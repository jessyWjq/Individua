using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Individua.Util.Office
{
    public class NPOIDataTableToExcel : IDataTableToExcel
    {
        private readonly string DEFAULT_SHEETNAME = "sheet1";
        private DataTable _dt = null;
        private NPOIDataTableToExcel() { }
        public NPOIDataTableToExcel(DataTable dt)
        {
            this._dt = dt;
        }

        public byte[] GetBuffer()
        {
            return GetBuffer("", DEFAULT_SHEETNAME);
        }

        public byte[] GetBuffer(string title)
        {
            return GetBuffer(title, DEFAULT_SHEETNAME);
        }

        public byte[] GetBuffer(string title, string sheetName)
        {
            byte[] bytes = null;
            try
            {
                using (MemoryStream stream = GetMemoryStream(title, sheetName))
                {
                    bytes = stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return bytes;
        }

        public void SaveFile(string fileName)
        {
            SaveFile(fileName, "", DEFAULT_SHEETNAME);
        }

        public void SaveFile(string fileName, string title)
        {
            SaveFile(fileName, title, DEFAULT_SHEETNAME);
        }

        public void SaveFile(string fileName, string title, string sheetName)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs.Position = 0;
                    byte[] bytes = GetBuffer(title, sheetName);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MemoryStream GetMemoryStream()
        {
            return GetMemoryStream("", DEFAULT_SHEETNAME);
        }

        public MemoryStream GetMemoryStream(string title)
        {
            return GetMemoryStream(title, DEFAULT_SHEETNAME);
        }

        public MemoryStream GetMemoryStream(string title, string sheetName)
        {

            MemoryStream stream = null;
            try
            {
                IWorkbook workBook = new HSSFWorkbook();

                ISheet sheet = workBook.CreateSheet(sheetName);
                //处理表格标题
                IRow row = sheet.CreateRow(0);
                row.CreateCell(0).SetCellValue(title);
                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, _dt.Columns.Count - 1));
                row.Height = 500;

                ICellStyle cellStyle = workBook.CreateCellStyle();
                IFont font = workBook.CreateFont();
                font.FontName = "微软雅黑";
                font.FontHeightInPoints = 17;
                cellStyle.SetFont(font);
                cellStyle.VerticalAlignment = VerticalAlignment.Center;
                cellStyle.Alignment = HorizontalAlignment.Center;
                row.Cells[0].CellStyle = cellStyle;

                //处理表格列头
                row = sheet.CreateRow(1);
                for (int i = 0; i < _dt.Columns.Count; i++)
                {
                    row.CreateCell(i).SetCellValue(_dt.Columns[i].ColumnName);
                    row.Height = 350;
                    sheet.AutoSizeColumn(i);
                }

                //处理数据内容
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    row = sheet.CreateRow(2 + i);
                    row.Height = 250;
                    for (int j = 0; j < _dt.Columns.Count; j++)
                    {
                        row.CreateCell(j).SetCellValue(_dt.Rows[i][j].ToString());
                        sheet.SetColumnWidth(j, 256 * 15);
                    }
                }
                stream = new MemoryStream();
                workBook.Write(stream);
                workBook.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return stream;
        }

    }
}
