using MimeKit;
using MailKit.Net.Smtp;
namespace EmailServices.Api.Services
{
    public class EmailService : IEmailService
    {
        public void SendEmail(string mail, string bodyString)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse("takadishen@gmail.com"));
            message.To.Add(MailboxAddress.Parse(mail));
            message.Subject = "Rạp chiếu phim ApecMovie";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
                <html>
                    <head>
                        <title>Thông báo từ APEC Movie</title>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                font-size: 14px;
                                line-height: 1.5;
                            }}
                            .container {{
                                max-width: 600px;
                                margin: 0 auto;
                                padding: 20px;
                                border: 1px solid #ccc;
                                border-radius: 5px;
                            }}
                            .header {{
                                text-align: center;
                                margin-bottom: 20px;
                            }}
                            .logo {{
                                max-width: 200px;
                                max-height: 200px;
                                display: block;
                                margin: 0 auto;
                            }}
                            .content {{
                                margin-bottom: 20px;
                            }}
                            .footer {{
                                text-align: center;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class=""container"">
                            <div class=""header"">
                                <img class=""logo"" src=""https://apecgroup.net/upload/news/Yc1i_logo-apec-group.jpg"" alt=""Logo"">
                                <h2>Rạp chiếu phim APEC Movie</h2>
                            </div>
                            <div class=""content"">
                                <p>Xin chào!</p>
                                <p>Dưới đây là nội dung email gửi từ APEC Movie.</p>
                                <p>{bodyString}</p>
                                <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>
                            </div>
                            <div class=""footer"">
                                <p>Trân trọng,</p>
                                <p>Nguyễn Việt Giang</p>
                            </div>
                        </div>
                    </body>
                </html>";

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
