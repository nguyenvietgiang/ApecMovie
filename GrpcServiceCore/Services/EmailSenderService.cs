using Grpc.Core;
using GrpcEmailService;

namespace GrpcServiceCore.Services
{
    public class EmailSenderService : EmailSender.EmailSenderBase
    {
        public override Task<EmailResponse> SendEmail(EmailRequest request, ServerCallContext context)
        {
            string message = $"Email sent to {request.To} with subject '{request.Subject}' and body '{request.Body}'";
            return Task.FromResult(new EmailResponse { Message = message });
        }
    }
}
