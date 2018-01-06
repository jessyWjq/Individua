using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Individua.CLI
{
    public class JsonTest
    {
        static void JTest()
        {
            string json = @"
            [
                {
                    'gguid': '3432564645768678678',
                    'xlkm': 65,
                    'shbh': '/Date(1375169588573+0800)/',
                    'hb': 30765,
                    'xl': 4869926,
                    'xy': 'v2764104',
                    'shzt': 'v2764104',
                    'cj': 'v2764104',
                    'xlcs': 'v2764104',
                    'xltime': '2017-11-11 22:23:24'
                },
                {
                    'gguid': '2232w43535345654756',
                    'xlkm': 65,
                    'shbh': '/Date(1375169588573+0800)/',
                    'hb': 30765,
                    'xl': 4869926,
                    'xy': 'v2764104',
                    'shzt': 'v2764104',
                    'cj': 'v2764104',
                    'xlcs': 'v2764104',
                    'xltime': '2017-11-11 23:23:23'
                }
            ]";
            List<GbxltnInfo> jobInfoList = JsonConvert.DeserializeObject<List<GbxltnInfo>>(json);
            foreach (GbxltnInfo jobInfo in jobInfoList)
            {
                Console.WriteLine("UserName:" + jobInfo.Xhb + "UserID:" + jobInfo.Gguid);
            }
        }
    }
}
