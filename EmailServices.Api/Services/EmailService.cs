using MimeKit;
using MailKit.Net.Smtp;
using GrpcEmailService;
namespace EmailServices.Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly IWebHostEnvironment _env;
       

        public EmailService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public void SendEmail(string toAddress, string subject, string bodyContent)
        {
            var webRootPath = _env.WebRootPath; 

            var templatePath = Path.Combine(webRootPath, "EmailTemplate", "NomalTemplate.html");

            var template = System.IO.File.ReadAllText(templatePath);
            template = template.Replace("{{title}}", subject)
                               .Replace("{{userName}}", toAddress)
                               .Replace("{{bodyContent}}", bodyContent)
                               .Replace("{{logoUrl}}", "https://apecgroup.net/upload/news/Yc1i_logo-apec-group.jpg")
                               .Replace("{{headerText}}", "Một sản phẩm của APEC Group")
                               .Replace("{{senderName}}", "APEC MOVIE");

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse("takadishen@gmail.com"));
            message.To.Add(MailboxAddress.Parse(toAddress));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = template;

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("takadishen@gmail.com", "zqyrplihnebvcswz");
                client.Send(message);
                client.Disconnect(true);
            }
        }


    }
}
