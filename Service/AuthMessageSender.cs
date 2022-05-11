using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace SendEmail.Service
{
    public class AuthMessageSender : IEmailSender
    {
        public EmailSettings _emailSettings { get; }

        public AuthMessageSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                Execute(email, subject, message).Wait();
                return Task.FromResult(0);
            }catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Execute(string email, string subject, string message)
        {
            try
            {
                string toEmail = email;

                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, "Denys Rodrigues")
                };

                mail.To.Add(new MailAddress(toEmail));

                mail.Subject = "Denys Rodrigues - " + subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient(
                    _emailSettings.PrimaryDomain, _emailSettings.PrimaryPort))
                {
                    smtp.Credentials = new NetworkCredential(
                        _emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
