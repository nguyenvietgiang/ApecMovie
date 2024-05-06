using Grpc.Core;
using GrpcEmailService;

namespace EmailServices.Api.Services
{
    public class EmailServiceImpl : GrpcEmailService.EmailSender.EmailSenderBase
    {
        private readonly IEmailService _emailService;
        private readonly EmailSender.EmailSenderClient _client;
        private readonly ILogger<EmailServiceImpl> _logger;

        public EmailServiceImpl(IEmailService emailService, GrpcEmailService.EmailSender.EmailSenderClient client, ILogger<EmailServiceImpl> logger)
        {
            _emailService = emailService;
            _client = client;
            _logger = logger;
        }

        public override Task<EmailResponse> SendEmail(EmailRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received email request: To={To}, Subject={Subject}, Body={Body}", request.To, request.Subject, request.Body);

            _emailService.SendEmail(request.To, request.Subject, request.Body);

            _logger.LogInformation("Email sent successfully");

            return Task.FromResult(new EmailResponse { Message = "Email sent successfully" });
        }
    }

}
