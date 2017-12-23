using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MEMO_JOB.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(subject, message, email);
        }
        public async Task<bool> Execute(string subject, string message, string email)
        {

            //SmtpClient client = new SmtpClient("mail.smtp2go.com");
            //client.UseDefaultCredentials = false;
            //client.Credentials = new NetworkCredential("putpro@gmail.com", "anhnokiaz");

            //MailMessage mailMessage = new MailMessage();
            //mailMessage.From = new MailAddress("Vinh@memo.ml");
            //mailMessage.To.Add(email);
            //mailMessage.Body = message;
            //mailMessage.IsBodyHtml = true;
            //mailMessage.Subject = subject;

            //return client.SendMailAsync(mailMessage);
            return false;
        }

        #region SendGrid
        //Send gird
        //public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor)
        //{
        //    Options = optionsAccessor.Value;
        //}

        //public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

        //public Task SendEmailAsync(string email, string subject, string message)
        //{
        //    return Execute(Options.SendGridKey, subject, message, email);
        //}

        //public Task Execute(string apiKey, string subject, string message, string email)
        //{
        //    var client = new SendGridClient(apiKey);
        //    var msg = new SendGridMessage()
        //    {
        //        From = new EmailAddress("putpro@gmail.com", "Vinh"),
        //        Subject = subject,
        //        PlainTextContent = message,
        //        HtmlContent = message
        //    };
        //    msg.AddTo(new EmailAddress(email));
        //    return client.SendEmailAsync(msg);
        //}
        #endregion
    }
}
