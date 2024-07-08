using DataAccess_Layer.Common;
using DataAccess_Layer.DTOs;
using DataAccess_Layer.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace DataAccess_Layer.Utilities
{
    public class MailService
    {
        private readonly MailSettings _mailSettings;

        public MailService(IOptions<MailSettings> options)
        {
            _mailSettings = options.Value;
        }
        public async Task<bool> SendEmailVerify(User user, EmailVerificationToken token)
        {
            try
            {
                MailRequest mailrequest = new MailRequest();
                mailrequest.ToEmail = user.Email;
                mailrequest.Subject = "Gmail confirmation from Miko Tech";
                string base64String = HtmlEmail.EncodeToBase64(user.Id.ToString(), token.TokenEmail);
                var generateLink = $"http://localhost:5173/verify/register?code={base64String}";
                mailrequest.Body = HtmlEmail.GetHtmlVerify(generateLink);
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_mailSettings.Email);
                email.To.Add(MailboxAddress.Parse(mailrequest.ToEmail));
                email.Subject = mailrequest.Subject;
                var builder = new BodyBuilder();
                builder.HtmlBody = mailrequest.Body;
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Email, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> SendEmailPassword(User user)
        {
            try
            {
                MailRequest mailrequest = new MailRequest();
                mailrequest.ToEmail = user.Email;
                mailrequest.Subject = "Gmail password from Miko Tech";
                mailrequest.Body = HtmlEmail.GetHtmlSendPassword(user.Password);
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_mailSettings.Email);
                email.To.Add(MailboxAddress.Parse(mailrequest.ToEmail));
                email.Subject = mailrequest.Subject;
                var builder = new BodyBuilder();
                builder.HtmlBody = mailrequest.Body;
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Email, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> SendPasswordResetEmail(OTPResetPassword otp)
        {
            try
            {
                MailRequest mailrequest = new MailRequest();
                mailrequest.ToEmail = otp.Email;
                mailrequest.Subject = "Gmail reset password from Miko Tech";
                mailrequest.Body = HtmlEmail.GetHtmlResetPassword(otp.OTP);
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_mailSettings.Email);
                email.To.Add(MailboxAddress.Parse(mailrequest.ToEmail));
                email.Subject = mailrequest.Subject;
                var builder = new BodyBuilder();
                builder.HtmlBody = mailrequest.Body;
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Email, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
