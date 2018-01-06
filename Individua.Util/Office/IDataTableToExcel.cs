using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Individua.Util.Office
{
    public interface IDataTableToExcel : IDataToExcel
    {
        #region Buffer

        /// <summary>
        /// 获取转换后的缓冲区数据
        /// </summary>
        /// <param name="title">标题</param>
        /// <returns></returns>
        byte[] GetBuffer(string title);
        /// <summary>
        /// 获取转换后的缓冲区数据
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="sheetName">sheet名字</param>
        /// <returns></returns>
        byte[] GetBuffer(string title, string sheetName);

        #region MemoryStream

        /// <summary>
        /// 获取MemoryStream
        /// </summary>
        /// <param name="title">标题</param>
        /// <returns></returns>
        MemoryStream GetMemoryStream(string title);
        /// <summary>
        /// 获取MemoryStream
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="sheetName">sheet名字</param>
        /// <returns></returns>
        MemoryStream GetMemoryStream(string title, string sheetName);

        #endregion

        #endregion

        #region SaveFile

        /// <summary>
        /// 保存Excel文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="title">标题</param>
        void SaveFile(string fileName, string title);
        /// <summary>
        /// 保存Excel文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="title">标题</param>
        /// <param name="sheetName">sheet名</param>
        void SaveFile(string fileName, string title, string sheetName);

        #endregion
    }
}
