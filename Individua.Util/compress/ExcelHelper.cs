/*
 ** 功能 ：excel读写类
 ** 作者 ：吴俊强
 ** 原作者: 裴磊
 ** 版本 ：v1.0
 ** 时间 ：2017/11/30 14:43:49
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.Web.Script.Serialization;

namespace Individua.Util.compress
{
    public class ExcelHelper
    {
        public ExcelHelper() { }

        /// <summary>
        /// 从excel中读取dataset
        /// </summary>
        /// <param name="sheetname"></param>
        /// <param name="resultfile"></param>
        /// <param name="filetype"></param>
        /// <returns></returns>
        public DataSet ReadDataFormExcel(string sheetname, string resultfile, string filetype)
        {
            string connectstring = string.Empty;
            string sql_laoren = "select * from [" + sheetname + "$A:N]";
            if (filetype == ".xls")
            {
                connectstring = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + resultfile + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
            }
            else
            {
                connectstring = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + resultfile + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
            }
            OleDbConnection con = new OleDbConnection(connectstring);
            con.Open();
            OleDbDataAdapter adpt = new OleDbDataAdapter(sql_laoren, con);
            DataSet ds = new DataSet();
            adpt.Fill(ds);
            return ds;
        }
        /// <summary>
        /// json 导出到excel
        /// </summary>
        /// <param name="json">传入json数组（单级）</param>
        /// <param name="filepath">导出路径</param>
        /// <param name="title">到处数据表的列头名</param>
        /// <returns></returns>
        public bool Json2Excel(string json, string filepath, string title = "")
        {
            //json转array
            JavaScriptSerializer js = new JavaScriptSerializer();
            ArrayList lists = js.Deserialize<ArrayList>(json);
            //导出状态
            bool bSuccess = true;

            Microsoft.Office.Interop.Excel.Application appexcel = new Microsoft.Office.Interop.Excel.Application();
            System.Reflection.Missing miss = System.Reflection.Missing.Value;
            appexcel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workbookdata = null;
            Microsoft.Office.Interop.Excel.Worksheet worksheetdata = null;
            Microsoft.Office.Interop.Excel.Range rangedata;

            workbookdata = appexcel.Workbooks.Add();

            //设置对象不可见  
            appexcel.Visible = false;
            appexcel.DisplayAlerts = false;
            try
            {
                //新建工作簿
                worksheetdata = (Microsoft.Office.Interop.Excel.Worksheet)workbookdata.Worksheets.Add(miss, workbookdata.ActiveSheet);
                Dictionary<string, object> item_0 = (Dictionary<string, object>)lists[0];
                int keys_count = item_0.Keys.Count, items_count = lists.Count;
                int xx = 0;
                //如果有手工赋值的列表名字，那就用手工赋值的，如果没有，就用字段名
                if (title != "")
                {
                    string[] titles = title.Split(new char[1] { '|' });
                    for (int TI = 0; TI < titles.Length; TI++)
                    {
                        worksheetdata.Cells[1, TI + 1] = titles[TI];
                        xx++;
                    }
                }
                else
                {
                    foreach (string key in item_0.Keys)
                    {
                        if (key.Substring(key.IndexOf("_"), key.Length) != "TEMP" && key.Substring(key.IndexOf("_"), key.Length) != "BTN")
                        {
                            //为列头赋值
                            worksheetdata.Cells[1, xx + 1] = key;
                            xx++;
                        }

                    }
                }
                //因为第一行已经写了表头，所以所有数据都应该从a2开始  
                rangedata = worksheetdata.get_Range("a2", miss);
                Microsoft.Office.Interop.Excel.Range xlrang = null;
                //irowcount为实际行数，最大行  
                int irowcount = items_count;         //实际行数？？
                int iparstedrow = 0, icurrsize = 0;
                //ieachsize为每次写行的数值，可以自己设置  
                //int ieachsize = 10000;
                //icolumnaccount为实际列数，最大列数  
                int icolumnaccount = keys_count;
                //确定表格的写入范围
                string X = "A" + ((int)(iparstedrow + 2)).ToString();
                string col = "";
                if (icolumnaccount <= 26)
                {
                    col = ((char)('A' + icolumnaccount - 1)).ToString() + ((int)(items_count + 1)).ToString();
                }
                else
                {
                    col = ((char)('A' + (icolumnaccount / 26 - 1))).ToString() + ((char)('A' + (icolumnaccount % 26 - 1))).ToString() + ((int)(iparstedrow + icurrsize + 1)).ToString();
                }
                //x：左上角的单元格  col：右下角的单元格或整行
                xlrang = worksheetdata.get_Range(X, col);
                xlrang.NumberFormat = "@";
                object[,] insert_infos = new object[items_count, keys_count];
                int row_count = 0;
                foreach (Dictionary<string, object> item in lists)
                {
                    int col_count = 0;
                    foreach (string key in item.Keys)
                    {
                        if (key.IndexOf("_") >= 0)
                        {


                            switch (key.Substring(key.IndexOf("_") + 1, key.Length - key.IndexOf("_") - 1))
                            {
                                case "TEMP":
                                    break;
                                case "DATE":
                                    insert_infos[row_count, col_count] = item[key].ToString().Substring(0, 10);
                                    col_count++;
                                    break;
                                case "COMMAS":
                                    insert_infos[row_count, col_count] = String.Format("{0:N2}", item[key]);
                                    col_count++;
                                    break;
                                case "PERCENT":
                                    insert_infos[row_count, col_count] = (Convert.ToDouble(item[key]) * 100).ToString() + "%";
                                    col_count++;
                                    break;
                                case "BTN":
                                    break;
                                default:
                                    insert_infos[row_count, col_count] = item[key];
                                    col_count++;
                                    break;

                            }
                        }
                        else
                        {
                            insert_infos[row_count, col_count] = item[key];
                            col_count++;
                        }

                    }
                    row_count++;
                }
                // 调用range的value2属性，把内存中的值赋给excel  
                xlrang.Value2 = insert_infos;
                //((Microsoft.Office.Interop.Excel.Worksheet)workbookdata.Worksheets["Sheet1"]).Delete();
                //((Microsoft.Office.Interop.Excel.Worksheet)workbookdata.Worksheets["Sheet2"]).Delete();
                //((Microsoft.Office.Interop.Excel.Worksheet)workbookdata.Worksheets["Sheet3"]).Delete();
                //保存工作表  
                workbookdata.SaveAs(filepath, miss, miss, miss, miss, miss, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, miss, miss, miss);
                workbookdata.Close(false, miss, miss);
                appexcel.Workbooks.Close();
                appexcel.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbookdata);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(appexcel.Workbooks);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(appexcel);
                GC.Collect();
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message;
                bSuccess = false;
            }
            finally
            {
                if (appexcel != null)
                {
                    // ExcelImportHelper.KillSpecialExcel(appexcel);
                }
            }
            return bSuccess;
        }


        public bool DT2Excel(DataTable dt, string filepath, string title = "")
        {
            //导出状态
            bool bSuccess = true;
            //int dt_count = 1;
            //for (int auv = 0; auv < dt.Rows.Count; auv += 5000)
            //{
            //命名excel
            TimeSpan ts = DateTime.UtcNow - new DateTime(1949, 10, 1, 0, 0, 0, 0);
            string name = Convert.ToInt64(ts.TotalSeconds).ToString();
            filepath = filepath + "\\" + name + ".xls";
            //json转array
            //JavaScriptSerializer js = new JavaScriptSerializer();
            //ArrayList lists = js.Deserialize<ArrayList>(json);



            Microsoft.Office.Interop.Excel.Application appexcel = new Microsoft.Office.Interop.Excel.Application();
            //Microsoft.Office.Interop.Excel.Workbook bookDest = null;
            System.Reflection.Missing miss = System.Reflection.Missing.Value;
            appexcel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workbookdata = null;
            Microsoft.Office.Interop.Excel.Worksheet worksheetdata = null;
            Microsoft.Office.Interop.Excel.Range rangedata;
            workbookdata = appexcel.Workbooks.Add();

            //设置对象不可见  
            appexcel.Visible = false;
            appexcel.DisplayAlerts = false;
            try
            {
                //新建工作簿
                //if (File.Exists(filepath))
                //{
                //    bookDest= appexcel.Workbooks._Open(filepath);
                //    workbookdata = bookDest.Worksheets.Add(miss, workbookdata.ActiveSheet);
                //}
                //else
                //{
                worksheetdata = (Microsoft.Office.Interop.Excel.Worksheet)workbookdata.Worksheets.Add(miss, workbookdata.ActiveSheet);
                //}
                //Dictionary<string, object> item_0 = (Dictionary<string, object>)lists[0];
                int keys_count = dt.Columns.Count, items_count = dt.Rows.Count;
                int xx = 0;
                //如果有手工赋值的列表名字，那就用手工赋值的，如果没有，就用字段名
                if (title != "")
                {
                    string[] titles = title.Split(new char[1] { '|' });
                    for (int TI = 0; TI < titles.Length; TI++)
                    {
                        worksheetdata.Cells[1, TI + 1] = titles[TI];
                        xx++;
                    }
                }
                else
                {
                    //用dt的列名命名

                    foreach (DataColumn colu in dt.Columns)
                    {
                        string key = colu.ColumnName;
                        //if (key.Substring(key.IndexOf("_"), key.Length) != "TEMP" && key.Substring(key.IndexOf("_"), key.Length) != "BTN")
                        if (key.Substring(key.IndexOf("_"), key.Length) != "TEMP")

                        {
                            //为列头赋值
                            worksheetdata.Cells[1, xx + 1] = key;
                            xx++;
                        }

                    }
                }
                //因为第一行已经写了表头，所以所有数据都应该从a2开始  
                rangedata = worksheetdata.get_Range("a2", miss);
                Microsoft.Office.Interop.Excel.Range xlrang = null;
                //irowcount为实际行数，最大行  
                int irowcount = items_count;         //实际行数？？
                int iparstedrow = 0, icurrsize = 0;
                //ieachsize为每次写行的数值，可以自己设置  
                //int ieachsize = 10000;
                //icolumnaccount为实际列数，最大列数  
                int icolumnaccount = keys_count;
                //确定表格的写入范围
                string X = "A" + ((int)(iparstedrow + 2)).ToString();
                string col = "";
                if (icolumnaccount <= 26)
                {
                    col = ((char)('A' + icolumnaccount - 1)).ToString() + ((int)(items_count + 1)).ToString();
                }
                else
                {
                    col = ((char)('A' + (icolumnaccount / 26 - 1))).ToString() + ((char)('A' + (icolumnaccount % 26 - 1))).ToString() + ((int)(iparstedrow + icurrsize + 1)).ToString();
                }
                //x：左上角的单元格  col：右下角的单元格或整行
                xlrang = worksheetdata.get_Range(X, col);
                xlrang.NumberFormat = "@";
                object[,] insert_infos = new object[items_count, keys_count];
                int row_count = 0;
                // foreach (Dictionary<string, object> item in lists)
                foreach (DataRow row in dt.Rows)
                {

                    int col_count = 0;
                    foreach (DataColumn colu in dt.Columns)
                    {
                        string key = colu.ColumnName;
                        if (key.IndexOf("_") >= 0)
                        {
                            switch (key.Substring(key.IndexOf("_") + 1, key.Length - key.IndexOf("_") - 1))
                            {
                                case "TEMP":
                                    break;
                                case "DATE":
                                    insert_infos[row_count, col_count] = row[key].ToString().Substring(0, 10);
                                    col_count++;
                                    break;
                                case "COMMAS":
                                    insert_infos[row_count, col_count] = String.Format("{0:N2}", row[key]);
                                    col_count++;
                                    break;
                                case "PERCENT":
                                    insert_infos[row_count, col_count] = (Convert.ToDouble(row[key]) * 100).ToString() + "%";
                                    col_count++;
                                    break;
                                //case "BTN":
                                //    break;
                                default:
                                    insert_infos[row_count, col_count] = row[key];
                                    col_count++;
                                    break;

                            }
                        }
                        else
                        {
                            insert_infos[row_count, col_count] = row[key];
                            col_count++;
                        }

                    }
                    row_count++;
                }
                // 调用range的value2属性，把内存中的值赋给excel  
                xlrang.Value2 = insert_infos;
                //((Microsoft.Office.Interop.Excel.Worksheet)workbookdata.Worksheets["Sheet1"]).Delete();
                //((Microsoft.Office.Interop.Excel.Worksheet)workbookdata.Worksheets["Sheet2"]).Delete();
                //((Microsoft.Office.Interop.Excel.Worksheet)workbookdata.Worksheets["Sheet3"]).Delete();
                //保存工作表  
                workbookdata.SaveAs(filepath, miss, miss, miss, miss, miss, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, miss, miss, miss);
                workbookdata.Close(false, miss, miss);
                appexcel.Workbooks.Close();
                appexcel.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbookdata);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(appexcel.Workbooks);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(appexcel);
                GC.Collect();
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.Message;
                bSuccess = false;
            }
            finally
            {
                if (appexcel != null)
                {
                    // ExcelImportHelper.KillSpecialExcel(appexcel);
                }
            }
            //    dt_count++;
            //}

            return bSuccess;
        }



        /// <summary>
        /// 使用流导出
        /// </summary>
        /// <param name="dataGridview1"></param>
        /// <param name="name"></param>
        public bool DT2Excel_stream(DataTable dt, string filepath, string title = "")
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.Filter = "Execl files (*.xls)|*.xls";
            //saveFileDialog.FilterIndex = 0;
            //saveFileDialog.RestoreDirectory = true;
            //saveFileDialog.CreatePrompt = true;
            //saveFileDialog.Title = "导出Excel文件到";
            #region 开始了
            TimeSpan ts = DateTime.UtcNow - new DateTime(1949, 10, 1, 0, 0, 0, 0);
            string name = Convert.ToInt64(ts.TotalSeconds).ToString();
            filepath = filepath + "/" + name + ".xls";
            bool resu = true;

            string str = "";

            StreamWriter sw = new StreamWriter(filepath, false, System.Text.Encoding.GetEncoding("gb2312"));

            try
            {
                //写标题   

                if (title != "")
                {
                    string[] titles = title.Split(new char[1] { '|' });

                    for (int i = 0; i < titles.Length; i++)
                    {
                        str += titles[i];
                        str += "\t";

                    }

                }
                else
                {
                    //用dt的列名命名

                    foreach (DataColumn colu in dt.Columns)
                    {
                        string key = colu.ColumnName;
                        //if (key.Substring(key.IndexOf("_"), key.Length) != "TEMP" && key.Substring(key.IndexOf("_"), key.Length) != "BTN")
                        if (key.Substring(key.IndexOf("_"), key.Length) != "TEMP")
                        {
                            //为列头赋值
                            str += key;
                            str += "\t";
                        }

                    }
                }
                sw.WriteLine(str);


                //写内容    
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    string tempStr = "";
                    DataRow row = dt.Rows[j];
                    foreach (DataColumn colu in dt.Columns)
                    {

                        string key = colu.ColumnName;
                        if (key.IndexOf("_") >= 0)
                        {
                            switch (key.Substring(key.IndexOf("_") + 1, key.Length - key.IndexOf("_") - 1))
                            {
                                case "TEMP":
                                    break;
                                case "DATE":
                                    tempStr += row[key].ToString().Substring(0, 10) + "\t";
                                    break;
                                case "COMMAS":
                                    tempStr += String.Format("{0:N2}", row[key]) + "\t";
                                    break;
                                case "PERCENT":
                                    tempStr += (Convert.ToDouble(row[key]) * 100).ToString() + "%\t";
                                    break;
                                case "BTN":
                                    tempStr += "'" + row[key] + "\t";
                                    break;
                                //case "BTN":
                                //    break;
                                default:
                                    tempStr += row[key] + "\t";
                                    break;

                            }
                        }
                        else if (key.IndexOf("SBH") >= 0)
                        {
                            tempStr += "'" + row[key] + "\t";
                        }
                        else
                        {
                            tempStr += row[key] + "\t";
                        }
                    }
                    sw.WriteLine(tempStr);
                }
                sw.Close();
            }
            catch (Exception )
            {
                resu = false;
            }
            finally
            {
                sw.Close();

            }
            return resu;
            #endregion
        }

    }
}
