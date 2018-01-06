using Individua.Util.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Individua.CLIRedis
{
    class Program
    {
        public static string SayHello()
        {
            return "Hello";
        }

        private delegate string Say();

        /// <summary>
        /// 根据id获取 如果存在 返回 否则 创建
        /// </summary>
        /// 默认存储五分钟
        /// <param name="id"></param>
        /// <returns></returns>
        public static UserInfo First(int id)
        {
            var user = RedisManager.Get<UserInfo>($"userInfo_{id}", () =>
            {
                return new UserInfo() { UserName = $"凄凄切切_{id}", UserPwd = $"11fff11_{id}" };
            }, 5);
            return user;
        }


        /// <summary>
        /// 
        /// </summary>
        public static List<T> GetTEntityList<T>(Func<int, T> func, params int[] ids)
        {
            List<T> tList = new List<T>();

            foreach (var item in ids ?? new int[0])
            {
                tList.Add(func.Invoke(item));
            }
            return tList;
        }

        static void Main(string[] args)
        {
            Func<string, string[]> extractMeth = delegate (string s)
            {
                char[] delimiters = new char[] { ' ' };
                return s.Split(delimiters);
            };

            string title = "The Scarlet Letter";
            // Use Func instance to call ExtractWords method and display result
            foreach (string word in extractMeth(title))
                Console.WriteLine(word + "\n");


            Func<String, int, bool> predicate = (str, index) => str.Length == index;

            //String[] words = { "orange", "apple", "Article", "elephant", "star", "and" };
            //IEnumerable<String> aWords = words.Where(predicate).Select(str => str);

            //foreach (String word in aWords)
            //    Console.WriteLine(word + "\n");

            var tex = TimeSpan.FromMinutes(120);
            for (int i = 0; i < 1; i++)
            {
                // 保存数据
                Func<UserInfo> func = () => new UserInfo()
                {
                    UserName = $"凄凄切切_{i}",
                    UserPwd = $"11fff11_{i}"
                };
                RedisManager.Set("userInfo_" + i, func, 20);
                Console.WriteLine(i);
            }
            Console.WriteLine(tex);
            // 添加 Value 到 Redis中 覆盖
            //RedisManager.Add("userInfo_1", "232", 10);
            // 
            Console.Write(RedisManager.Contains("redis_userInfo_0"));

            //RedisManager.RemoveContaions("userInfo");

            //int[] ids = { 1, 2, 3, 4, 33, 44, 55, 35, 33, 32, 101 };

            //var list = GetTEntityList<UserInfo>(First, ids);
            //List<UserInfo> lists = new List<UserInfo>();

            //foreach (var item in list)
            //{
            //    lists.Add(item);
            //    Console.WriteLine(item.UserName );
            //}
            //Console.WriteLine("listsJson:" + lists.ToJson());





            Console.ReadKey();



        }
    }
}
