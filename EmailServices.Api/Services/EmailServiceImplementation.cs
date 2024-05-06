using Grpc.Core;
using GrpcEmailService;
using Grpc.Net.Client;

namespace EmailServices.Api.Services
{
    public class EmailServiceImpl : GrpcEmailService.EmailSender.EmailSenderBase
    {
        private readonly IEmailService _emailService;
        private readonly EmailSender.EmailSenderClient _client;

        public EmailServiceImpl(IEmailService emailService, GrpcEmailService.EmailSender.EmailSenderClient client)
        {
            _emailService = emailService;
            _client = client;
        }

        public override Task<EmailResponse> SendEmail(EmailRequest request, ServerCallContext context)
        {
            _client.SendEmail(request);

            _emailService.SendEmail(request.To, request.Subject, request.Body);

            return Task.FromResult(new EmailResponse { Message = "Email sent successfully" });
        }
    }

}
