using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Helpers
{
    public class MailHelper
    {
        private readonly static string SMTPServer = "smtp.office365.com";
        private readonly static int Port = 587;
        private readonly static string UserName = "justrekisoftware@outlook.com";
        private readonly static string Pwd = "1314510qaz";
        private readonly static string FromName = "JustReki Software";
        private readonly static string FromEmail = "justrekisoftware@outlook.com";
        private readonly static string SubjectIdentifyCode = "修改密码";


        public async static Task<string> SendIdentifyCodeAsync(string toName, string toEmail)
        {

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(FromName, FromEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = SubjectIdentifyCode;
            var builder = new StringBuilder();
            var code = GenIdentifyCode(6);
            builder.AppendLine("您好，" + toName);
            builder.AppendLine("我们检测到您正在修改密码，若非您本人操作，请检查账号安全性或申请冻结账号。");
            builder.AppendLine();
            builder.AppendLine("验证码：  " + code);
            builder.AppendLine();
            builder.AppendLine("注：该验证码仅本次操作有效");
            message.Body = new TextPart("plain")
            {
                Text = builder.ToString()
            };

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.ConnectAsync(SMTPServer, Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(UserName, Pwd);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

            return code;
        }

        private static string GenIdentifyCode(int length)
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
