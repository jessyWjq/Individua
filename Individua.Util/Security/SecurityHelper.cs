/*
 ** 功能 ：安全帮助工具类
 ** 作者 ：徐德意
 ** 版本 ：v1.0
 ** 时间 ：2017/7/16 9:33:09
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Individua.Util.Security
{
    public class SecurityHelper
    {
        /// <summary>
        /// 密码加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string PasswordEncrypt(string password)
        {
            //md5(md5(base64))加密方式
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            string base64Str = Convert.ToBase64String(bytes);
            string md51 = MD5Helper.GetStringMD5(base64Str);
            string md52 = MD5Helper.GetStringMD5(md51);
            return md52.ToLower();
        }
        /// <summary>
        /// 获取 密码 Cookie 加密 字符串
        /// </summary>
        /// <param name="md5Password"></param>
        /// <returns></returns>
        public static string GetPasswordCookieEncryptStr(string md5Password)
        {
            return DESHelper.Encrypt(md5Password);
        }
        /// <summary>
        /// 获取 密码 Cookie 解密 字符串
        /// </summary>
        /// <param name="eccryptKey"></param>
        /// <returns></returns>
        public static string GetPasswordCookieDecryptStr(string eccryptKey)
        {
            return DESHelper.Decrypt(eccryptKey);
        }
    }
}
