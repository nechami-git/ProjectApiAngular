using server.BLL.Intarfaces;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks; // חובה בשביל Task

namespace server.BLL
{
    public class EmailBLL : IEmailBLL
    {
        private readonly IConfiguration _config;

        public EmailBLL(IConfiguration config)
        {
            _config = config;
        }
        public async Task<bool> SendEmail(string to, string subject, string body)
        {
            try
            {
                var mail = _config["EmailSettings:Mail"]; // תוודאי שהשם תואם ל-appsettings
                var password = _config["EmailSettings:Password"];
                var host = "smtp.gmail.com";
                var port = 587;

                var message = new MailMessage();
                message.From = new MailAddress(mail, "Chinese Sale System");
                message.To.Add(new MailAddress(to));
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using (var client = new SmtpClient(host, port))
                {
                    client.Credentials = new NetworkCredential(mail, password);
                    client.EnableSsl = true;

                    await client.SendMailAsync(message);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // אם נכשל - מדפיסים לקונסול ומחזירים שקר
                Console.WriteLine("Email Error: " + ex.Message);
                return false;
            }
        }
    }
}