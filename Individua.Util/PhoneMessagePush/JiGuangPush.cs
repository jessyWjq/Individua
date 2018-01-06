using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Individua.Util.PhoneMessagePush
{
    public static class JiGuangPush
    {
        #region 极光推送
        /*--------------------------------极光推送-----------------------------*/
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
        public static bool Push(MessageEntity entity, string jsry)
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
        /*------------------------------极光推送----------------------------*/
        #endregion
    }
}
