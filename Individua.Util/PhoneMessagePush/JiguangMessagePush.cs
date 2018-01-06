/*
 ** 功能 ：极光推送
 ** 作者 ：吴俊强
 ** 版本 ：v1.0
 ** 时间 ：2017/8/2 19:45:43
*/
using cn.jpush.api;
using cn.jpush.api.common;
using cn.jpush.api.common.resp;
using cn.jpush.api.device;
using cn.jpush.api.push.mode;
using Individua.Util.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Individua.Util.PhoneMessagePush
{
    public class JiguangMessagePush : IMessagePush
    {
        private static String app_key = "a8fcdf410a06f79bd5b3947f";
        private static String master_secret = "7272ba30452ee347725b1995";

        public static int DELAY_TIME = 1;
        //激光对象
        private static JPushClient client = null;

        public JiguangMessagePush()
        {
            client = new JPushClient(app_key, master_secret);
        }
        
        public bool SendMessage(MessageEntity message, params string[] tags)
        {
            bool resultFlag = false;
            PushPayload payload = PushObject_Android_Tag_AlertWithTitle(message, tags);

            try
            {
                //推送
                var result = client.SendPush(payload);
                resultFlag = true;
            }
            catch (APIRequestException e)
            {
                Console.WriteLine("Error response from JPush server. Should review and fix it. ");
                Console.WriteLine("----------");
                Console.WriteLine("HTTP Status: " + e.Status);
                Console.WriteLine("----------");
                Console.WriteLine("Error Code: " + e.ErrorCode);
                Console.WriteLine("----------");
                Console.WriteLine("Error Message: " + e.ErrorMessage);
            }
            catch (APIConnectionException e)
            {
                Console.WriteLine("----------");
                Console.WriteLine(e.Message);
            }

            return resultFlag;
        }

        /// <summary>
        /// 构建推送对象：平台是 Android，目标是 tag 为 "tag1" 的设备
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static PushPayload PushObject_Android_Tag_AlertWithTitle(MessageEntity message, params string[] tags)
        {
            //构建Message对象

            PushPayload pushPayload = new PushPayload()
            {
                platform = Platform.android(),
                audience = Audience.s_tag(tags),
                notification = Notification.android(message.Alert, message.Title),
                message = Message.content(message.Content.ToJson()),
            };

            return pushPayload;
        }
    }
}
