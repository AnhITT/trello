using DataAccess_Layer.DTOs;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace DataAccess_Layer.Utilities
{
    public abstract class BaseMailService
    {
        protected readonly MailSettings _mailSettings;

        protected BaseMailService(IOptions<MailSettings> options)
        {
            _mailSettings = options.Value;
        }

        protected MimeMessage CreateEmail(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Email);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();

            return email;
        }

        protected async Task<bool> SendEmail(MimeMessage email)
        {
            try
            {
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
