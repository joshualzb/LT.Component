using System;
using System.Net;
using System.Net.Mail;

namespace LT.Component.Utility
{
    /// <summary>
    /// EmailSender 通用类
    /// </summary>
    public static class EmailSender
    {
        /// <summary>
        /// 使用HTML格式发送邮件
        /// </summary>
        /// <param name="smtp">SMTP服务器，如 mail.myce.net.cn</param>
        /// <param name="username">SMTP认证用户名</param>
        /// <param name="password">SMTP认证密码</param>
        /// <param name="from">发送者邮件，如 industry@myce.net.cn</param>
        /// <param name="to">接收者，如多个接收则使用分号分隔</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容</param>
        public static void Send(string smtp, string username, string password, string sender, string subject, string body, bool isBodyHtml, string address, params string[] cc)
        {
            System.Web.Mail.MailMessage mailMessage = new System.Web.Mail.MailMessage();
            mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserver", smtp);
            mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserverport", "25");
            mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusing", 2);
            mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", 1);
            mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", username);
            mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", password);

            mailMessage.From = sender;
            mailMessage.To = address;

            if (cc != null)
            {
                foreach (var item in cc)
                {
                    mailMessage.Cc += "," + item;
                }

                if (mailMessage.Cc != null)
                {
                    mailMessage.Cc = mailMessage.Cc.TrimStart(',');
                }
            }

            mailMessage.Subject = subject;
            mailMessage.BodyFormat = System.Web.Mail.MailFormat.Html;
            mailMessage.Body = body;

            System.Web.Mail.SmtpMail.SmtpServer = smtp;
            System.Web.Mail.SmtpMail.Send(mailMessage);

            //demo
            //LT.Component.Utility.EmailSender.Send("smtp.163.com", "joshualzb@163.com", "joshualzb3858293", "joshualzb@163.com", "系统运行异常[中申运物流管理系统]", "test", false, "396167973@qq.com");
        }

        /// <summary>
        /// 异步发送邮件
        /// </summary>
        /// <param name="smtp">SMTP服务器，如 mail.myce.net.cn</param>
        /// <param name="username">SMTP认证用户名</param>
        /// <param name="password">SMTP认证密码</param>
        /// <param name="sender">发送者邮件，如 industry@myce.net.cn</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="isBodyHtml">是否是以HTML格式发送</param>
        /// <param name="address">发送给</param>
        public static void SendEmail(string smtp, string username, string password, string sender, string subject, string body, bool isBodyHtml, params string[] address)
        {
            MailAddress from = new MailAddress(sender);
            MailMessage message = new MailMessage();
            message.From = from;
            foreach (string sendTo in address)
            {
                message.Bcc.Add(sendTo);
            }
            message.Subject = subject;//设置邮件主题 
            message.IsBodyHtml = isBodyHtml;//设置邮件正文为html格式 
            message.Body = body;//设置邮件内容 
            SmtpClient client = new SmtpClient(smtp);
            client.Credentials = new NetworkCredential(username, password);
            client.SendCompleted += client_SendCompleted;
            client.SendAsync(message, null);
        }

        public static void client_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
        }
    }
}
