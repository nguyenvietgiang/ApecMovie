using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcEmailService;


namespace gRPC_CoreServer.Services
{
    public class EmailSenderService: GrpcEmailService.EmailSender.EmailSenderBase
    {
        public override Task<EmailResponse> SendEmail(EmailRequest request, ServerCallContext context)
        {
            string message = $"Email sent to {request.To} with subject '{request.Subject}' and body '{request.Body}'";
            return Task.FromResult(new EmailResponse { Message = message });
        }
    }
}
