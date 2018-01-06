/*
 ** 功能 ：推送消息内容
 ** 作者 ：吴俊强
 ** 版本 ：v1.0
 ** 时间 ：2017/8/2 19:48:01
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Individua.Util.PhoneMessagePush
{
    public class MessageEntity
    {
        /// <summary>
        /// 推送标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 推送弹出来的内容
        /// </summary>
        public string Alert { get; set; }
        /// <summary>
        /// 推送内容
        /// </summary>
        public MessageContentEntity Content { get; set; }
    }
}
