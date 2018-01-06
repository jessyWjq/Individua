using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Individua.Util.PhoneMessagePush
{
    /// <summary>
    /// 推送接口
    /// </summary>
    public interface IMessagePush
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <returns></returns>
        bool SendMessage(MessageEntity message, params string[] tags);
    }
}
