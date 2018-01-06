using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Individua.Util.Office
{
    public class NPOIDataSetToExcel : IDataToExcel
    {
        private DataSet _ds = null;
        private NPOIDataSetToExcel() { }
        public NPOIDataSetToExcel(DataSet ds)
        {
            this._ds = ds;
        }

        public byte[] GetBuffer()
        {
            byte[] bytes = null;
            try
            {
                using (MemoryStream stream = GetMemoryStream())
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
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs.Position = 0;
                    byte[] bytes = GetBuffer();
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
            MemoryStream stream = null;
            try
            {
                IWorkbook workBook = new HSSFWorkbook();

                for (int g = 0; g < _ds.Tables.Count; g++)
                {
                    DataTable dt = _ds.Tables[g];

                    ISheet sheet = workBook.CreateSheet(dt.TableName);
                    //处理表格标题
                    IRow row = sheet.CreateRow(0);
                    row.CreateCell(0).SetCellValue(dt.TableName);
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dt.Columns.Count - 1));
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
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        row.CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
                        row.Height = 350;
                        sheet.AutoSizeColumn(i);
                    }

                    //处理数据内容
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        row = sheet.CreateRow(2 + i);
                        row.Height = 250;
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            row.CreateCell(j).SetCellValue(dt.Rows[i][j].ToString());
                            sheet.SetColumnWidth(j, 256 * 15);
                        }
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
