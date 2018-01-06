using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Individua.Util
{
    public static class ExportHelp
    {
        public static void ExportToExcel(string FileName, string dt)
        {
            HttpContext.Current.Response.Charset = "utf-8";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" 
                + HttpUtility.UrlEncode(FileName, System.Text.Encoding.UTF8).ToString());
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            //StringWriter tw = new StringWriter();

            //string style = @"<style> .textmode {mso-number-format:General} </style>";
            string style = @"<style> .textmode { mso-number-format:\@; } </style>";

            HttpContext.Current.Response.Write(style);
            HttpContext.Current.Response.Output.Write(dt);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }
    }
}
