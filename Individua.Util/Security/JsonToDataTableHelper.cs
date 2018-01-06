/*
 ** 功能 ：将 Json 解析成 DateTable
 ** 作者 ：吴俊强
 ** 版本 ：v1.0
 ** 时间 ：2017/9/13 11:01:41
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Individua.Util.Security
{
    public static class JsonToDataTableHelper
    {
        /// <summary>   
        /// 将 Json 解析成 DateTable。  
        /// Json 数据格式如: 
        ///     [{column1:1,column2:2,column3:3},{column1:1,column2:2,column3:3}] /// </summary>   
        /// <param name="strJson">要解析的 Json 字符串</param>   
        /// <returns>返回 DateTable</returns>   
        public static DataTable JsonToDataTable(string strJson)
        {
            // 取出表名   
            Regex rg = new Regex(@"(?<={)[^:]+(?=:\[)", RegexOptions.IgnoreCase);
            string strName = rg.Match(strJson).Value;
            DataTable tb = null;

            //// 去除表名   
            strJson = strJson.Substring(strJson.IndexOf("[") + 1);
            strJson = strJson.Substring(0, strJson.IndexOf("]"));

            // 获取数据   
            rg = new Regex(@"(?<={)[^}]+(?=})");
            MatchCollection mc = rg.Matches(strJson);
            for (int i = 0; i < mc.Count; i++)
            {
                string strRow = mc[i].Value;
                string[] strRows = strRow.Split(',');
                // 创建表   
                if (tb == null)
                {
                    tb = new DataTable();
                    foreach (string str in strRows)
                    {
                        DataColumn dc = new DataColumn();
                        string[] strCell = str.Split(':');
                        dc.ColumnName = strCell[0].Replace("\"", "");
                        //dc.ColumnName = strCell[0];
                        tb.Columns.Add(dc);
                    }
                    tb.AcceptChanges();
                }
                // 增加内容   
                DataRow dr = tb.NewRow();
                for (int j = 0; j < strRows.Length; j++)
                {
                    //dr[j] = strRows[j].Split(':')[1].Replace("\"", "");
                    dr[j] = strRows[j].Split(':')[1].Replace("\"", "").Replace("：", ":").Replace("\"", "");
                }
                tb.Rows.Add(dr);
                tb.AcceptChanges();
            }
            return tb;
        }
    }
}
