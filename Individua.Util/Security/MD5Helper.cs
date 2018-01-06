/*
 ** 功能 ：MD5加密帮助类
 ** 作者 ：徐德意
 ** 版本 ：v1.0
 ** 时间 ：2017/7/16 9:33:09
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Individua.Util.Security
{
    public class MD5Helper
    {
        /// <summary>  
        /// 获取文件的MD5码  
        /// </summary>  
        /// <param name="fileName">传入的文件名（含路径及后缀名）</param>  
        /// <returns></returns>  
        public static string GetFileMD5(string fileName)
        {
            FileStream file = new FileStream(fileName, System.IO.FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 获取字符串的MD5码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetStringMD5(string str)
        {
            byte[] data = Encoding.Default.GetBytes(str);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] outBytes = md5.ComputeHash(data);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < outBytes.Length; i++)
            {
                sb.Append(outBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 获取字符串的MD5码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetStringMD5x16(string str)
        {
            byte[] data = Encoding.Default.GetBytes(str);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] outBytes = md5.ComputeHash(data);

            string md5x16 = BitConverter.ToString(outBytes, 4, 8);
            md5x16 = md5x16.Replace("-", "");
            md5x16 = md5x16.ToLower();
            return md5x16;
        }
        /// <summary>
        /// 获取Bytes的MD5
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetBytesMD5(byte[] bytes)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] outBytes = md5.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < outBytes.Length; i++)
            {
                sb.Append(outBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
