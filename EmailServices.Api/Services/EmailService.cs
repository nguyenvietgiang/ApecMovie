using MimeKit;
using MailKit.Net.Smtp;
using System.Text;

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
            var template = GetEmailTemplate("NomalTemplate.html");
            template = ReplacePlaceholders(template, subject, toAddress, bodyContent: bodyContent);

            SendEmailWithTemplate(toAddress, subject, template);
        }

        public void VerifyEmail(string toAddress, string subject, string ticketID, string token)
        {
            var template = GetEmailTemplate("VerifyTemplate.html");
            template = ReplacePlaceholders(template, subject, toAddress, ticketID, token);

            SendEmailWithTemplate(toAddress, subject, template);
        }

        private string GetEmailTemplate(string templateName)
        {
            var webRootPath = _env.WebRootPath;
            var templatePath = Path.Combine(webRootPath, "EmailTemplate", templateName);
            return System.IO.File.ReadAllText(templatePath);
        }

        private string ReplacePlaceholders(string template, string subject, string toAddress, string bodyContent = null, string ticketID = null, string token = null)
        {
            template = template
                .Replace("{{title}}", subject)
                .Replace("{{userName}}", toAddress)
                .Replace("{{logoUrl}}", "https://apecgroup.net/upload/news/Yc1i_logo-apec-group.jpg")
                .Replace("{{headerText}}", "Một sản phẩm của APEC Group")
                .Replace("{{senderName}}", "APEC MOVIE");

            if (!string.IsNullOrEmpty(bodyContent))
                template = template.Replace("{{bodyContent}}", bodyContent);

            if (!string.IsNullOrEmpty(ticketID))
                template = template.Replace("{{ticketID}}", ticketID);

            if (!string.IsNullOrEmpty(token))
                template = template.Replace("{{token}}", token);

            return template;
        }

        private void SendEmailWithTemplate(string toAddress, string subject, string template)
        {
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
