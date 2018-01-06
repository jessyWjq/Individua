using Individua.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Individua.BLL
{
    public partial class XFGBBLL
    {
        private readonly XFGBDAL dalXfgb = new XFGBDAL();

        /// <summary>
        /// 查询训练科目信息
        /// </summary>
        public DataSet SelXlkmxx()
        {
            return dalXfgb.SelXlkmxx();
        }

        /// <summary>
        /// 查询训练科目类型下的具体科目
        /// </summary>
        public DataSet SelXlkmxxByxlguid(string xlguid)
        {
            return dalXfgb.SelXlkmxxByxlguid(xlguid);
        }
    }
}
