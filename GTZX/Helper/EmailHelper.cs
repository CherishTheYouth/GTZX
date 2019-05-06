using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Helper
{
    /// <summary>
    /// 邮件发送辅助类
    /// </summary>
    public static class EmailHelper
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailFrom">发送者邮箱</param>
        /// <param name="password">发送者邮箱密码</param>
        /// <param name="mailTo">目标邮箱</param>
        /// <param name="subject">标题</param>
        /// <param name="body">内容</param>
        /// <param name="isBodyHtml">内容是否为html格式，默认为是</param>
        /// <param name="enableSsl">是否加密，默认为否</param>
        public static bool SendEmail(string mailFrom, string password, string mailTo, string subject, string body,
            bool isBodyHtml = true, bool enableSsl = false)
        {
            var smptClient = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = GetEmailServerAddress(mailFrom),
                Credentials = new NetworkCredential(mailFrom, password),
                EnableSsl = enableSsl
            };

            var mailMessage = new MailMessage(mailFrom, mailTo)
            {
                Subject = subject,
                SubjectEncoding = Encoding.UTF8,
                Body = body,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = isBodyHtml,
                Priority = MailPriority.High
            };
            try
            {
                smptClient.Send(mailMessage);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取邮箱主机地址
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <returns></returns>
        public static string GetEmailServerAddress(string email)
        {
            var server = string.Empty;
            switch (email.Substring(email.LastIndexOf("@", StringComparison.CurrentCultureIgnoreCase)))
            {
                case "@qq.com":
                    server = "smtp.qq.com";
                    break;
                case "@tgeosmart.com":
                    server = "smtp.exmail.qq.com";
                    break;
                case "@sina.com":
                    server = "smtp.sina.com.cn";
                    break;
                case "@sina.cn":
                    server = "smtp.sina.com";
                    break;
                case "@sohu.com":
                    server = "smtp.sohu.com";
                    break;
                case "@126.com":
                    server = "smtp.126.com";
                    break;
                case "@163.com":
                    server = "smtp.163.com";
                    break;
                case "@139.com":
                    server = "smtp.139.com";
                    break;
                case "@yahoo.com":
                    server = "smtp.mail.yahoo.com";
                    break;
                case "@yahoo.com.cn":
                    server = "smtp.mail.yahoo.com.cn";
                    break;
                case "@hotmail.com":
                    server = "smtp.live.com";
                    break;
                case "@gmail.com":
                    server = "smtp.gmail.com";
                    break;
                case "@263.net":
                    server = "smtp.263.net";
                    break;
                case "@263.net.cn":
                    server = "smtp.263.net.cn";
                    break;
                case "@x263.net":
                    server = "smtp.x263.net";
                    break;
                case "@21cn.com":
                    server = "smtp.21cn.com";
                    break;
                case "@foxmail.com":
                    server = "smtp.foxmail.com";
                    break;
                case "@china.com":
                    server = "smtp.china.com";
                    break;
                case "@tom.com":
                    server = "smtp.tom.com";
                    break;
                case "@etang.com":
                    server = "smtp.etang.com";
                    break;
            }
            return server;
        }
    }
}
