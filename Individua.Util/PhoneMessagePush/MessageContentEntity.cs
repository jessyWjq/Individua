/*
 ** 功能 ：推送信息内容
 ** 作者 ：吴俊强
 ** 版本 ：v1.0
 ** 时间 ：2017/8/3 11:40:45
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Individua.Util.PhoneMessagePush
{
    /// <summary>
    /// 信息内容
    /// </summary>
    public class MessageContentEntity
    {
        /// <summary>
        /// 信息类型
        /// </summary>
        public MessageContentType Type { get; set; } = MessageContentType.Info;
        /// <summary>
        /// 信息GUID
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// 信息内容
        /// </summary>
        //public string Contents { get; set; }

        public MessageContentEntity()
        { }

        public MessageContentEntity(MessageContentType type, string guid)
        {
            this.Type = type;
            this.Guid = guid;
            //this.Contents = contents;
        }
    }
}
