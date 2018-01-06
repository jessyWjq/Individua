using Individua.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Web;
using System.Web.Mvc;
using Individua.Util.ImageHelps;
using System.Drawing.Imaging;
using Individua.Comm;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;
using System.Net;
using Individua.Util.Extensions;
using Individua.Util;
using System.Data;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Individua.Web.Controllers
{
    public class AcountController : Controller
    {
        private AndroidResult aResult = new AndroidResult();

        // GET: Acount
        public ActionResult Index()
        {
            return View();
        }

        // GET: Acount/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Acount/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Acount/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Acount/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Acount/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// 包含图片文件上传的请求
        /// </summary>
        /// <param name="collection">包含应用程序的窗体值提供程序。</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Uplode(FormCollection collection)
        {
            string name = collection.Keys[0];
            Console.WriteLine(name);
            System.Diagnostics.Debug.Write(name);

            string rensavepath = string.Empty;
            try
            {
                HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                if (files.Count > 0)
                {
                    string wjj = DateTime.Now.ToLocalTime().ToString("yyyyMMdd");
                    string path = Server.MapPath("/");
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFile file = files[i];//获取名称对应的文件进行操作    
                        if (file != null && file.ContentLength > 0)
                        {
                            string hzm = Path.GetExtension(file.FileName.ToString()).ToLower();
                            if (hzm == ".png" || hzm == ".gif" || hzm == ".jpg" || hzm == ".bmp")
                            {
                                string dir = Path.Combine(Path.GetDirectoryName(path), "files/" + wjj);   // 保存本地路径
                                string fileName = DateTime.Now.ToLocalTime().ToString("yyyyMMddhhmmssffff") + hzm;//file.FileName;
                                if (!Directory.Exists(dir))
                                {
                                    //不存在则创建该目录
                                    Directory.CreateDirectory(dir);
                                }
                                using (FileStream fs = new FileStream($"{dir}/{fileName}", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                                {
                                    fs.Position = 0;
                                    byte[] bytes = new byte[file.ContentLength];
                                    file.InputStream.Read(bytes, 0, file.ContentLength);
                                    fs.Write(bytes, 0, bytes.Length);
                                    fs.Flush();
                                }
                                string newpath = DateTime.Now.ToLocalTime().ToString("yyyyMMddhhmmssffff1") + hzm;
                                string savepath = wjj + "/" + newpath;   // 返回 保存数据库路径
                                string imgUrl = $"{ dir}/{newpath}";
                                Image Image = Image.FromFile($"{ dir}/{ fileName}");
                                int widthImage = Image.Width;
                                int heightImage = Image.Height;
                                Image.Dispose();
                                ImageHelps.MakeThumbnail($"{ dir}/{ fileName}", $"{ imgUrl}", widthImage, heightImage);
                                ImageHelps.DeleteImgFile($"{ dir}/{ fileName}");
                                // 拼接多个地址
                                if (i == files.Count - 1)
                                {
                                    rensavepath += savepath;
                                }
                                else
                                {
                                    rensavepath += savepath + ",";
                                }
                            }
                            else
                            {
                                rensavepath = "图片格式不正确";
                                break;
                            }
                        }
                        else
                        {
                            rensavepath = "文件不能为空";
                            break;
                        }
                    }
                }
                else
                {
                    rensavepath = "没有接收到文件";
                }
            }
            catch (Exception ex)
            {
                rensavepath = ex.Message;
            }
            return Content(rensavepath);
        }

        /// <summary>
        /// 单独文件保存处理
        /// </summary>
        /// <param name="file">客户端上传的单独文件</param>
        /// <returns></returns>
        protected string SaveFile(HttpPostedFileBase file)
        {
            string baseFilePath = Config.FileUploadPath;
            string fileUrl = string.Empty;

            if (file != null)
            {
                string tempUrl = DateTime.Now.ToString("yyyyMM");
                string dir = Path.Combine(Path.GetDirectoryName(baseFilePath), tempUrl);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string fileName = file.FileName;
                string suffix = fileName.Substring(fileName.IndexOf('.'));
                fileUrl = $"/{tempUrl}/{fileName}";//数据库保存

                using (FileStream fs = new FileStream($"{dir}/{fileName}",
                    FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs.Position = 0;
                    byte[] bytes = new byte[file.ContentLength];
                    file.InputStream.Read(bytes, 0, file.ContentLength);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                }
            }
            return fileUrl;
        }

        /// <summary>
        /// 获取地址路径信息
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> TaskT()
        {
            HttpClient client = new HttpClient();
            var src = client.GetStringAsync("https://www.visualstudio.com/");
            await src;
            string _Task_id = "Task_id:" + src.Id + ",_IsCompleted:" + src.IsCompleted;
            //var str = await client.GetStringAsync("https://www.visualstudio.com/");
            src.Dispose();
            return Content(src.Result.Length.ToString() + "_TaskID:" + _Task_id);
        }


        /// <summary>
        /// 异步 工厂化处理
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<ActionResult> Article(string name)
        {
            return Task.Factory.StartNew(() =>
            {
                string path = ControllerContext.HttpContext.Server.MapPath(string.Format(@"\articles\{0}.html", name));
                using (StreamReader reader = new StreamReader(path))
                {
                    AsyncManager.Parameters["content"] = reader.ReadToEnd();
                }
            }).ContinueWith<ActionResult>(task =>
            {
                string content = (string)AsyncManager.Parameters["content"];
                return Content(content);
            });
        }

        /// <summary>
        /// 异步获取相应文件内得正文部分
        /// </summary>
        /// <param name="name"></param>
        public void ArticleAsync(string name)
        {
            AsyncManager.OutstandingOperations.Increment();
            Task.Factory.StartNew(() =>
            {
                string path = ControllerContext.HttpContext.Server.MapPath(string.Format(@"\articles\{0}.html", name));
                using (StreamReader reader = new StreamReader(path))
                {
                    AsyncManager.Parameters["content"] = reader.ReadToEnd();
                }
                AsyncManager.OutstandingOperations.Decrement();
            });
        }

        // 获取网页信息打印
        [AsyncTimeout(100)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "Error")]
        public async Task<ActionResult> IndexCancleAsync()
        {
            var cancellationToken = new CancellationToken(false);
            var cnblogsTask = GetStringAsync("http://www.cnblogs.com", cancellationToken);
            var myblogTask = GetStringAsync("http://www.cnblogs.com/wintersun", cancellationToken);
            await Task.WhenAll(cnblogsTask, myblogTask);
            Operations translations = new Operations()
            {
                FirstOperation = cnblogsTask.Result,
                SecondOperation = myblogTask.Result
            };
            string json = translations.ToJson().Replace("\\r\\n", "");
            json = json.Replace("\\r\\n\\t", "").Replace("\\t", "");
            return Content(json);
        }

        public async Task<ActionResult> Dowlode()
        {
            IList<Uri> uris = new List<Uri>
            {
                new Uri("http://17a640351l.iask.in/Style/image/index.jpg"),
                new Uri("http://192.168.1.69:4561/attachment/201709230949534489.jpg"),
            };
            int count = await SumPageSizesAsync(uris);
            return Content(count.ToString());
        }

        /// <summary>
        /// 下载文件 by url集合
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        private bool[] DownloadFileArray(params string[] urls)
        {
            bool[] bools = new bool[urls.Length];
            using (WebClient client = new WebClient())
            {
                for (int i = 0; i < urls.Length; i++)
                {
                    try
                    {
                        client.DownloadFileAsync(new Uri(urls[i]), $"/filename{i}.txt");
                        bools[i] = true;
                    }
                    catch { }
                }
            }
            return bools;
        }

        /// <summary>
        /// 异步下载文件 by url集合
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        private async Task<bool[]> DownloadFileArrayAsync(params string[] urls)
        {
            return await Task.FromResult(DownloadFileArray(urls));
        }


        // 未完成
        /// <summary>
        /// 根据下载路径下载文件
        /// </summary>
        /// <param name="uris">下载路基集合</param>
        /// <returns></returns>
        private async Task<int> SumPageSizesAsync(IList<Uri> uris)
        {
            int total = 0;
            string sss = "";
            foreach (var uri in uris)
            {
                sss = string.Format("Found {0} bytes...", total);
                using (WebClient client = new WebClient())
                {
                    //var data =await client.DownloadFileAsync(uri, total.ToString());
                    await client.DownloadDataTaskAsync(uri);
                }
                await new WebClient().DownloadFileTaskAsync(uri, total.ToString());
                var data = await new WebClient().DownloadDataTaskAsync(uri);
                total += data.Length;
            }
            sss = string.Format("Found {0} bytes total", total);
            //return await Task.FromResult();
            return total;
        }

        private static async Task<string> GetStringAsync(string uri
             , CancellationToken cancelToken = default(CancellationToken))
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(uri, cancelToken);
                return (await response.Content.ReadAsStringAsync());
            }
        }

        public ActionResult IsAdmin()
        {
            bool flag = IsUserAdmin.IsUserAdministrator();
            return Content(flag.ToString());
        }

        #region 异步demo
        public async Task<ActionResult> Action(string uname, string pwd)
        {
            return await Task.Run(() =>
            {
                DataTable dt = new DataTable();
                return dt;
            }).ContinueWith<ActionResult>(task =>
            {
                return Content(task.Result.ToJson());
            });
        }
        #endregion

        #region 导出
        public void SelRKDByDHoutExcel(string rkdh, string title, string alter, string spzt)
        {
            int nuum = 0;
            string jgname = Session["jigou"].ToString();//Session：登录机构
            DataTable dtData = new DataTable();
            if (dtData.Rows.Count > 0)
            {
                string shtnl = "";
                // shtnl = "<meta http-equiv='content-type' content='application/ms-excel; charset=UTF-8'/>";
                shtnl = "<table border='1' cellspacing='1' cellpadding='1'>";
                shtnl = shtnl + "<tr align='center'>";
                shtnl = shtnl + "<td  colspan='11'><h2>" + title + "</h2></td>";
                shtnl = shtnl + "</tr>";
                shtnl = shtnl + "<tr align='center'>";
                shtnl = shtnl + "<td  colspan='11'>" + alter + "</td>";
                shtnl = shtnl + "</tr>";
                shtnl = shtnl + "<thead>";
                shtnl = shtnl + "<th>序号</th>";
                shtnl = shtnl + "<th>器材名称</th>";
                shtnl = shtnl + "<th>器材型号</th>";
                shtnl = shtnl + "<th>生产厂家</th>";
                shtnl = shtnl + "<th>数量</th>";
                shtnl = shtnl + "<th>单价</th>";
                shtnl = shtnl + "<th>来源</th>";
                shtnl = shtnl + "<th>报废提醒</th>";
                shtnl = shtnl + "<th>维保提醒</th>";
                shtnl = shtnl + "<th>审核状态</th>";
                shtnl = shtnl + "<th>备注</th>";
                shtnl = shtnl + "</thead><tbody>";
                for (int i = 0; i < dtData.Rows.Count; i++)
                {
                    nuum = nuum + 1;
                    shtnl = shtnl + "<tr>";
                    shtnl = shtnl + "<td>" + nuum.ToString() + "</td>";
                    shtnl = shtnl + "<td>" + dtData.Rows[i]["rkmc"] + "</td>";
                    shtnl = shtnl + "<td>" + dtData.Rows[i]["rjxh"] + "</td>";
                    shtnl = shtnl + "<td>" + dtData.Rows[i]["rkcj"] + "</td>";
                    shtnl = shtnl + "<td>" + dtData.Rows[i]["rknum"] + "</td>";
                    shtnl = shtnl + "<td>" + dtData.Rows[i]["rkdj"] + "</td>";
                    shtnl = shtnl + "<td>" + dtData.Rows[i]["rkly"] + "</td>";
                    shtnl = shtnl + "<td>" + dtData.Rows[i]["rkbftx"] + "</td>";
                    shtnl = shtnl + "<td>" + dtData.Rows[i]["rkwbjl"] + "</td>";
                    shtnl = shtnl + "<td>" + spzt + "</td>";
                    shtnl = shtnl + "<td>" + dtData.Rows[i]["rknote"] + "</td>";
                    shtnl = shtnl + "</tr>";
                }
                shtnl = shtnl + "<tr align='center'>";
                shtnl = shtnl + "<td  colspan='11'>" + "<br/>" + "审批人：" + "_____________" + "领用人：" + "_____________" + "经手人：" + "_____________" + "日期：" + "_____________" + "<br/>" + "<br/>" + "</td>";
                shtnl = shtnl + "</tr>";
                shtnl = shtnl + "</tbody></table>";
                string bm = DateTime.Now.ToString("yyyyMMddhhmmss");
                string filename = jgname + "入库单清单" + bm + ".xls";
                ExportHelp.ExportToExcel(filename, shtnl);
            }
        }

        #endregion        

        #region 图片压缩方法二
        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalImagePath"></param>
        /// <param name="thumbnailPath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height)
        {
            //获取原始图片 
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);
            //缩略图画布宽高 
            int towidth = width;
            int toheight = height;
            //原始图片写入画布坐标和宽高(用来设置裁减溢出部分) 
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            //原始图片画布,设置写入缩略图画布坐标和宽高(用来原始图片整体宽高缩放) 
            int bg_x = 0;
            int bg_y = 0;
            int bg_w = towidth;
            int bg_h = toheight;
            //倍数变量 
            double multiple = 0;
            //获取宽长的或是高长与缩略图的倍数 
            if (originalImage.Width >= originalImage.Height) multiple = (double)originalImage.Width / (double)width;
            else multiple = (double)originalImage.Height / (double)height;
            //上传的图片的宽和高小等于缩略图 
            if (ow <= width && oh <= height)
            {
                //缩略图按原始宽高 
                bg_w = originalImage.Width;
                bg_h = originalImage.Height;
                //空白部分用背景色填充 
                bg_x = Convert.ToInt32(((double)towidth - (double)ow) / 2);
                bg_y = Convert.ToInt32(((double)toheight - (double)oh) / 2);
            }
            //上传的图片的宽和高大于缩略图 
            else
            {
                //宽高按比例缩放 
                bg_w = Convert.ToInt32((double)originalImage.Width / multiple);
                bg_h = Convert.ToInt32((double)originalImage.Height / multiple);
                //空白部分用背景色填充 
                bg_y = Convert.ToInt32(((double)height - (double)bg_h) / 2);
                bg_x = Convert.ToInt32(((double)width - (double)bg_w) / 2);
            }
            //新建一个bmp图片,并设置缩略图大小. 
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
            //新建一个画板 
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            //设置高质量,低速度呈现平滑程度 
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空画布并设置背景色 
            g.Clear(System.Drawing.ColorTranslator.FromHtml("#F2F2F2"));
            //在指定位置并且按指定大小绘制原图片的指定部分 
            //第一个System.Drawing.Rectangle是原图片的画布坐标和宽高,第二个是原图片写在画布上的坐标和宽高,最后一个参数是指定数值单位为像素 
            g.DrawImage(originalImage, new System.Drawing.Rectangle(bg_x, bg_y, bg_w, bg_h), new System.Drawing.Rectangle(x, y, ow, oh), System.Drawing.GraphicsUnit.Pixel);
            try
            {
                //获取图片类型 
                string fileExtension = System.IO.Path.GetExtension(originalImagePath).ToLower();
                //按原图片类型保存缩略图片,不按原格式图片会出现模糊,锯齿等问题. 
                switch (fileExtension)
                {
                    case ".gif": bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Gif); break;
                    case ".jpg": bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg); break;
                    case ".bmp": bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Bmp); break;
                    case ".png": bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Png); break;
                }
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }

        }
        /// <summary>  
        /// 获取图片编码类型信息  
        /// </summary>  
        /// <param name="coderType">编码类型</param>  
        /// <returns>ImageCodecInfo</returns>  
        private ImageCodecInfo GetImageCoderInfo(string coderType)
        {
            ImageCodecInfo[] iciS = ImageCodecInfo.GetImageEncoders();

            ImageCodecInfo retIci = null;

            foreach (ImageCodecInfo ici in iciS)
            {
                if (ici.MimeType.Equals(coderType))
                    retIci = ici;
            }

            return retIci;
        }
        #endregion

        #region 删除文件

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileUrl">绝对路径</param>
        public static void DeleteImgFile(string fileUrl)
        {
            if (System.IO.File.Exists(fileUrl))
            {
                System.IO.File.Delete(fileUrl);
            }
        }

        #endregion

        #region Ip
        public string GetIP()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            string IP = request.UserHostAddress.ToString();
            string ip = "本机ip:" + GetLocalIpAddress();
            ip += "\n外网ip：" + GetExtenalIpAddress();
            ip += "\n服务器ip:" + GetClientIpConvertServer();
            ip += "\n主机地址:" + IP;
            return ip;

        }

        /// <summary>
        /// 获取本机内网 ip
        /// </summary>
        /// <returns></returns>
        private string GetLocalIpAddress()
        {
            //获取所有网卡信息
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                //判断是否为以太网卡
                //Wireless80211         无线网卡    Ppp     宽带连接
                //Ethernet              以太网卡   
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    //获取以太网卡网络接口信息
                    IPInterfaceProperties ip = adapter.GetIPProperties();
                    //获取单播地址集
                    UnicastIPAddressInformationCollection ipCollection = ip.UnicastAddresses;
                    foreach (UnicastIPAddressInformation ipadd in ipCollection)
                    {
                        //InterNetwork    IPV4地址      InterNetworkV6        IPV6地址
                        //Max            MAX 位址
                        if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                        //判断是否为ipv4
                        {
                            string strLocalIP = ipadd.Address.ToString();//获取ip
                            return strLocalIP;//获取ip
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>  
        /// 获取外网ip地址  
        /// </summary>  
        private static string GetExtenalIpAddress()
        {
            String url = "http://hijoyusers.joymeng.com:8100/test/getNameByOtherIp";
            string IP = "未获取到外网ip";
            try
            {
                //从网址中获取本机ip数据    
                WebClient client = new WebClient
                {
                    Encoding = System.Text.Encoding.Default
                };
                string str = client.DownloadString(url);
                client.Dispose();
                if (!str.Equals("")) IP = str;
                else IP = GetExtenalIpAddress();
            }
            catch (Exception) { }
            return IP;
        }

        /// <summary>
        /// 获取客户端IP地址  转为服务器
        /// </summary>
        private string GetClientIpConvertServer()
        {
            string hostName = Dns.GetHostName();//本机名   
            //IPAddress[] addressList = Dns.GetHostByName(hostName).AddressList;//会警告GetHostByName()已过期，   //我运行时且只返回了一个IPv4的地址   
            IPAddress[] addressList = Dns.GetHostAddresses(hostName);//会返回所有地址，包括IPv4和IPv6   
            foreach (IPAddress ip in addressList)
            {
                hostName = ip.ToString();
            }
            IPHostEntry fromHE = Dns.GetHostEntry(Dns.GetHostName());
            IPEndPoint ipEndPointFrom = new IPEndPoint(fromHE.AddressList[2], 80);
            EndPoint EndPointFrom = (ipEndPointFrom);
            return hostName;
        }
        #endregion


        #region 20180105 生成验证码图片
        public ActionResult SecurityCode()
        {
            string oldcode = TempData["SecurityCode"] as string;
            string code = VerificationHelper.CreateRandomCode();
            TempData["SecurityCode"] = code;
            return File(VerificationHelper.CreateValidateGraphic(code), "image/Jpeg");
        }
        #endregion





    }
}






