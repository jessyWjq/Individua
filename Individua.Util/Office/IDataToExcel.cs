using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Individua.Util.Office
{
    /// <summary>
    /// Data导出Excel
    /// </summary>
    public interface IDataToExcel
    {
        #region Buffer

        /// <summary>
        /// 获取转换后的缓冲区数据
        /// </summary>
        /// <returns></returns>
        byte[] GetBuffer();

        #region MemoryStream

        /// <summary>
        /// 获取MemoryStream
        /// </summary>
        /// <returns></returns>
        MemoryStream GetMemoryStream();

        #endregion

        #endregion

        #region SaveFile

        /// <summary>
        /// 保存Excel文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        void SaveFile(string fileName);

        #endregion
    }
}
