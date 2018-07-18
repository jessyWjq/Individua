using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Individua.Util
{
    public static class SqlBulkCopyHelp
    {
        /// <summary>
        /// 批量插入数据(insert)
        /// </summary>
        /// <param name="dt">数据集</param>
        /// <param name="sqlConn">sqlConn链接</param>
        /// <param name="TableName">插入表名</param>
        public static void BulkToDB(DataTable dt, SqlConnection sqlConn, string tableName)
        {
            SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConn);
            bulkCopy.DestinationTableName = tableName;
            bulkCopy.BatchSize = dt.Rows.Count;
            try
            {
                sqlConn.Open();
                if (dt != null && dt.Rows.Count != 0)
                    bulkCopy.WriteToServer(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlConn.Close();
                if (bulkCopy != null)
                    bulkCopy.Close();
            }
        }

        /// <summary>
        /// 批量插入数据(insert) 
        /// </summary>
        /// <param name="dt">数据集</param>
        /// <param name="sqlConn">sqlConn链接</param>
        /// <param name="TableName">插入表名</param>
        /// <returns>并返回用时(毫秒)</returns>
        public static Tuple<string, int> BulkToDbRtnMs(DataTable dt, SqlConnection sqlConn, string tableName)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            BulkToDB(dt, sqlConn, tableName);
            sw.Stop();
            return new Tuple<string, int>(string.Format("Elapsed Time is {0} Milliseconds", sw.ElapsedMilliseconds), (int)sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// 初始化数据进行插入
        /// </summary>
        /// <returns>影响值</returns>
        public Tuple<string, int> initDate()
        {
            SqlConnection sqlConn = new SqlConnection(
                    ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            DataTable dt = GetTableSchema();
            for (int count = 1 * 100; count < (1 + 1) * 100; count++)
            {
                DataRow r = dt.NewRow();
                r[0] = count;
                r[1] = string.Format("User-{0}", count * 1);
                r[2] = string.Format("Pwd-{0}", count * 1);
                dt.Rows.Add(r);
            }
            Tuple<string, int> tuple = SqlBulkCopyHelp.BulkToDbRtnMs(dt, sqlConn, "BulkTestTable");
            return tuple;
        }

        /// <summary>
        /// 构建表结构
        /// </summary>
        /// <returns>Table</returns>
        public static DataTable GetTableSchema()
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[]{  
             new DataColumn("Id",typeof(int)),  
             new DataColumn("UserName",typeof(string)),  
             new DataColumn("Pwd",typeof(string))});
            return dt;
        }
    }
}
