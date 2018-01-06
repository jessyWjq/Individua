using Individua.BLL;
using Individua.Util.Office;
using Individua.Util.PhoneMessagePush;
using Individua.Web.Models;
using System;
using System.Data;
using System.Web.Mvc;

namespace Individua.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly AndroidResult aResult = new AndroidResult();
        private readonly XFGBBLL bllXfgb = new XFGBBLL();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Xiazai()
        {
            DataTable item = bllXfgb.SelXlkmxx().Tables[0];
            return DataTableToExcelFile(item, "ssss");
        }

        /// <summary>
        /// DataTable保存为Excel文件
        /// </summary>
        /// <param name="dt">datatable</param>
        /// <param name="title">文件标题及文件名</param>
        /// <returns></returns>
        protected FileContentResult DataTableToExcelFile(DataTable dt, string title)
        {
           
            IDataTableToExcel excel = new NPOIDataTableToExcel(dt);

            byte[] bytes = excel.GetBuffer(title);

            return File(bytes, "application/x-xls", title + ".xls");
        }


        #region 极光推送
        /*---------------------------------------------------极光推送-------------------------------------------------------*/
        /// <summary>
        /// 极光推送
        /// </summary>
        /// <param name="guid">消息guid（报告）</param>
        /// <param name="title">标题</param>
        /// <param name="cout">内容</param>
        /// <param name="xxsj">时间</param>
        /// <param name="type">类型(消息，报告)</param>
        /// <param name="jsry">接收人</param>
        /// <returns></returns>
        public bool JiGuangPush(MessageEntity entity, string jsry)
        {
            IMessagePush push = PushFactory.CreatePush(PushType.JiGuang);

            //MessageEntity entity = new MessageEntity()
            //{
            //    Alert = cout,
            //    Title = title + "   " + fsr,
            //    Content = new MessageContentEntity(type, guid)
            //};
            string[] stringsss = jsry.Split(new Char[] { ',' });

            bool sendFlag = push.SendMessage(entity, stringsss);
            if (sendFlag)
            {
                Console.WriteLine("推送成功");
                return true;
            }
            else
            {
                Console.WriteLine("推送失败");
                return false;
            }
        }
        /*---------------------------------------------------极光推送-------------------------------------------------------*/
        #endregion


    }
}