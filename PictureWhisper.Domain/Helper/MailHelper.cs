using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Helpers
{
    /// <summary>
    /// 邮件发送帮助类
    /// </summary>
    public class MailHelper
    {
        //邮件发送配置
        private readonly static string SMTPServer = "smtp.office365.com";
        private readonly static int Port = 587;
        private readonly static string UserName = "justrekisoftware@outlook.com";
        private readonly static string Pwd = "1314510qaz";
        private readonly static string FromName = "JustReki Software";
        private readonly static string FromEmail = "justrekisoftware@outlook.com";
        private readonly static string SubjectIdentifyCode = "修改密码";

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="toEmail">邮件接收者邮箱</param>
        /// <returns>返回验证码</returns>
        public async static Task<string> SendIdentifyCodeAsync(string toName, string toEmail)
        {
            //配置邮件信息
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(FromName, FromEmail));//发送者
            message.To.Add(new MailboxAddress(toName, toEmail));//接收者
            message.Subject = SubjectIdentifyCode;//主题
            var builder = new StringBuilder();
            var code = GenIdentifyCode(6);
            builder.AppendLine("您好，" + toName);
            builder.AppendLine("我们检测到您正在修改密码，若非您本人操作，请检查账号安全性或申请冻结账号。");
            builder.AppendLine();
            builder.AppendLine("验证码：  " + code);
            builder.AppendLine();
            builder.AppendLine("注：该验证码仅本次操作有效");
            message.Body = new TextPart("plain")//邮件内容
            {
                Text = builder.ToString()
            };
            //发送邮件
            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.ConnectAsync(SMTPServer, Port, SecureSocketOptions.StartTls);//连接邮箱服务器
                await client.AuthenticateAsync(UserName, Pwd);//登录
                await client.SendAsync(message);//发送邮件
                await client.DisconnectAsync(true);//断开连接
            }

            return code;
        }

        /// <summary>
        /// 生成随机验证码，默认为6位
        /// </summary>
        /// <param name="length">验证码长度</param>
        /// <returns>返回验证码</returns>
        private static string GenIdentifyCode(int length = 6)
        {
            StringBuilder builder = new StringBuilder(length);
            Random rand = new Random();
            for (int i = 0; i < length; i++)
            {
                builder.Append(rand.Next(0, 10));
            }

            return builder.ToString();
        }
    }
}
