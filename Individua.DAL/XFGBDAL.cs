using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Individua.Util;

namespace Individua.DAL
{
    public partial class XFGBDAL
    {

        /// <summary>
        /// 查询训练科目信息
        /// </summary>
        public DataSet SelXlkmxx()
        {
            string sqlCmd = @" select  xlguid,xllx from xf_gbxlkm where xlname='kmlx' ";
            SqlParameter[] param = {
            };
            DataSet msg = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sqlCmd, param);
            return msg;
        }

        /// <summary>
        /// 查询训练科目类型下的具体科目
        /// </summary>
        public DataSet SelXlkmxxByxlguid(string xlguid)
        {
            string sqlCmd = @" select xlguid,xlname,xldw from xf_gbxlkm  where xllx = @xlguid ";
            SqlParameter[] param = {
                    new SqlParameter("@xlguid",xlguid)
            };
            DataSet msg = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sqlCmd, param);
            return msg;
        }

    }
}
