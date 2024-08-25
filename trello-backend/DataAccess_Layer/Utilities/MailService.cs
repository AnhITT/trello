using DataAccess_Layer.Common;
using DataAccess_Layer.DTOs;
using DataAccess_Layer.Models;
using Microsoft.Extensions.Options;
using MimeKit;

namespace DataAccess_Layer.Utilities
{
    public class MailService : BaseMailService
    {
        public MailService(IOptions<MailSettings> options) : base(options)
        {
        }

        public async Task<bool> SendEmailVerify(User user, EmailVerificationToken token)
        {
            string base64String = HtmlEmail.EncodeToBase64(user.Id.ToString(), token.TokenEmail);
            var generateLink = $"http://localhost:5173/verify/register?code={base64String}";
            var mailRequest = new MailRequest
            {
                ToEmail = user.Email,
                Subject = "Gmail confirmation from Trello",
                Body = HtmlEmail.GetHtmlVerify(generateLink)
            };

            var email = CreateEmail(mailRequest);
            return await SendEmail(email);
        }

        public async Task<bool> SendEmailPassword(User user, string password)
        {
            var mailRequest = new MailRequest
            {
                ToEmail = user.Email,
                Subject = "Gmail password from Miko Tech",
                Body = HtmlEmail.GetHtmlSendPassword(password)
            };

            var email = CreateEmail(mailRequest);
            return await SendEmail(email);
        }

        public async Task<bool> SendPasswordResetEmail(OTPResetPassword otp)
        {
            var mailRequest = new MailRequest
            {
                ToEmail = otp.Email,
                Subject = "Gmail reset password from Trello",
                Body = HtmlEmail.GetHtmlResetPassword(otp.OTP)
            };

            var email = CreateEmail(mailRequest);
            return await SendEmail(email);
        }
    }
}
