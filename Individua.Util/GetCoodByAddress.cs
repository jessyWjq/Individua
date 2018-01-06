/*
 ** 功能 ：根据地址获取坐标
 ** 作者 ：吴俊强
 ** 版本 ：v1.0
 ** 时间 ：2017/11/15 14:02:26
*/
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Individua.Util
{
    public static class GetCoodByAddress
    {
        #region 根据位置获取坐标信息()
        public static string getCoords(string patch)
        {
            string latlng = "";
            if (patch != "" && patch != null)
            {
                // 8b01b43ae1161eb55a4d8dd0652166e7
                // 4a7d93e52ebbdf79e3c255dc0c6364b2
                //
                string url = "http://restapi.amap.com/v3/geocode/geo?key=4a7d93e52ebbdf79e3c255dc0c6364b2&address=" + patch;
                try
                {
                    WebRequest wRequest = WebRequest.Create(url);
                    //wRequest.Timeout = 6000;
                    WebResponse wResponse = wRequest.GetResponse();
                    Stream stream = wResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                    string r = reader.ReadToEnd();   //url返回的值
                    if (r.IndexOf("geocodes") == -1)
                    {
                        latlng = ",";
                    }
                    else
                    {
                        JObject jo = (JObject)JsonConvert.DeserializeObject(r);
                        string zone = jo["geocodes"].ToString();
                        JArray Jarows = JArray.Parse(zone);
                        if (Jarows.Count <= 0)
                        {
                            latlng = ",";
                        }
                        for (int j = 0; j < Jarows.Count; j++)
                        {
                            JObject Jorow = (JObject)JsonConvert.DeserializeObject(Jarows[j].ToString());
                            latlng = Jorow["location"].ToString();
                        }
                    }
                }
                catch (Exception)
                {
                    latlng = "异常";
                }
            }
            return latlng.ToString();
        }



        public static string GetInfo(string patch)
        {
            string strBuff = "";
            string url = "http://restapi.amap.com/v3/geocode/geo?key=4a7d93e52ebbdf79e3c255dc0c6364b2&address=" + patch;
            Uri httpURL = new Uri(url);
            ///HttpWebRequest类继承于WebRequest，并没有自己的构造函数，需通过WebRequest的Creat方法 建立，并进行强制的类型转换   
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(httpURL);
            ///通过HttpWebRequest的GetResponse()方法建立HttpWebResponse,强制类型转换   
            HttpWebResponse httpResp = (HttpWebResponse)httpReq.GetResponse();
            ///GetResponseStream()方法获取HTTP响应的数据流,并尝试取得URL中所指定的网页内容   
            ///若成功取得网页的内容，则以System.IO.Stream形式返回，若失败则产生ProtoclViolationException错 误。在此正确的做法应将以下的代码放到一个try块中处理。这里简单处理   
            Stream respStream = httpResp.GetResponseStream();
            ///返回的内容是Stream形式的，所以可以利用StreamReader类获取GetResponseStream的内容，并以   
            //StreamReader类的Read方法依次读取网页源程序代码每一行的内容，直至行尾（读取的编码格式：UTF8）   
            StreamReader respStreamReader = new StreamReader(respStream, Encoding.UTF8);
            strBuff = respStreamReader.ReadToEnd();
            if (strBuff.IndexOf("geocodes") == -1)
            {
                strBuff = ",";
            }
            else
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(strBuff);
                string zone = jo["geocodes"].ToString();
                JArray Jarows = JArray.Parse(zone);
                if (Jarows.Count <= 0)
                {
                    strBuff = ",";
                }
                for (int j = 0; j < Jarows.Count; j++)
                {
                    JObject Jorow = (JObject)JsonConvert.DeserializeObject(Jarows[j].ToString());
                    strBuff = Jorow["location"].ToString();
                }
            }
            return strBuff;
        }
        #endregion
    }
}
