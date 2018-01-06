using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace Individua.Util.EmailPush
{
    public static class EmailPush
    {
        #region 邮件发送
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">要发送的邮箱</param>
        /// <param name="mailSubject">邮箱主题</param>
        /// <param name="mailContent">邮箱内容</param>
        /// <returns>返回发送邮箱的结果</returns>
        public static bool SendEmail(string mailTo, string mailSubject, string mailContent)
        {
            // 设置发送方的邮件信息,例如使用网易的smtp
            string smtpServer = "smtp.126.com"; //SMTP服务器
            string mailFrom = "xietongruanjian@126.com"; //登陆用户名
            string userPassword = "xtrj@6309900";//登陆密码

            // 邮件服务设置
            SmtpClient smtpClient = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,//指定电子邮件发送方式
                Host = smtpServer, //指定SMTP服务器
                Credentials = new System.Net.NetworkCredential(mailFrom, userPassword)//用户名和密码
            };

            // 发送邮件设置        
            // 发送人和收件人
            MailMessage mailMessage = new MailMessage(mailFrom, mailTo)
            {
                Subject = mailSubject,//主题
                Body = mailContent,//内容
                BodyEncoding = Encoding.UTF8,//正文编码
                IsBodyHtml = true,//设置为HTML格式
                Priority = MailPriority.Low//优先级
            }; 

            try
            {
                smtpClient.Send(mailMessage); // 发送邮件
                return true;
            }
            catch (SmtpException ex)
            {
                Console.WriteLine( ex.Message);
                return false;
            }
        }
        #endregion
    }
} 