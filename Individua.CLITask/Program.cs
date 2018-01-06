using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Individua.CLITask
{
    class Program
    {
        static Task Method1Async() {
            return Task.Delay(1);
        }

        static async Task<int> Method2Async()
        {
            await Task.Delay(100);
            return 1;
        }
        static void Main(string[] args)
        {

            Show();

            Console.ReadKey();
        }

        static async void Show() {
            Task a = Method1Async();
            //此处可继续执行其他代码
            await a;//等待任务a完成
            Task<int> b = Method2Async();
            //此处可继续执行其他代码
            int c = await b;//等待任务b完成，且可以拿到任务b的返回值
            Console.WriteLine(a.Id);
            Console.WriteLine(b.Id);
        }
             
    }
}
