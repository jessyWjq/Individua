using System;
using System.Collections.Generic;
using System.Text;

namespace Individua.coreCLI
{
    /// <summary>
    /// 字符串转换成二进制流
    /// </summary>
    public class StrTo2jz
    {
        //Char字符转成7位二进制  
        public static string StringToBinary(string ss)
        {
            byte[] u = Encoding.ASCII.GetBytes(ss);
            string binaryNum = string.Empty;
            string result = string.Empty;
            foreach (byte a in u)
            {
                binaryNum = Convert.ToString(a, 2);
                if (binaryNum.Length < 7)
                {
                    for (int i = 0; i < 7 - binaryNum.Length; i++)
                    {
                        binaryNum = '0' + binaryNum;
                    }
                }
                result += binaryNum;
            }
            return result;
        }

        public static void Show(string str)
        {
            Console.WriteLine(str + "\n");
            char[] np = str.ToCharArray();
            string username = "";
            string password = "";
            bool flag = false;
            string a = "";
            for (int i = 0; i < np.Length; i++)
            {
                a += np[i];
                if (a.Length % 7 == 0)
                {
                    if (Convert.ToInt32(a, 2) == 0)
                    {
                        flag = true;
                        a = string.Empty;
                        continue;
                    }
                    if (flag)
                        password += Convert.ToChar(Convert.ToInt32(a, 2));
                    else
                        username += Convert.ToChar(Convert.ToInt32(a, 2));
                    a = string.Empty;
                }
            }
            Console.WriteLine(password);
            Console.WriteLine(username);
        }
    }
}
