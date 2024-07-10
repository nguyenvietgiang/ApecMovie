using Grpc.Core;
using GrpcEmailService;

namespace EmailServices.Api.Services
{
    public class EmailSenderService : EmailSender.EmailSenderBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailSenderService> _logger;
        public EmailSenderService(IEmailService emailService, ILogger<EmailSenderService> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        // ServerCallContext context: Bối cảnh của cuộc gọi gRPC.
        public override Task<EmailResponse> SendEmail(EmailRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received email request: To={To}, Subject={Subject}, Body={Body}", request.To, request.Subject, request.Body);

            _emailService.SendEmail(request.To, request.Subject, request.Body);

            _logger.LogInformation("Email sent successfully");

            return Task.FromResult(new EmailResponse { Message = "Email sent successfully" });
        }
    }
}
