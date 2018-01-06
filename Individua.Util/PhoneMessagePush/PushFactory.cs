/*
 ** 功能 ：推送工厂类
 ** 作者 ：吴俊强
 ** 版本 ：v1.0
 ** 时间 ：2017/8/2 20:37:31
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Individua.Util.PhoneMessagePush
{
    public static class PushFactory
    {
        public static IMessagePush CreatePush(PushType pushType)
        {
            IMessagePush iPush = null;
            switch (pushType)
            {
                //激光推送
                case PushType.JiGuang:
                    iPush = new JiguangMessagePush();
                    break;
            }
            return iPush;
        }
    }
}
