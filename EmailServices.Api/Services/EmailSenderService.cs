using Grpc.Core;
using Grpc.Net.Client;
using GrpcEmailService;
using System.Threading.Tasks;

namespace EmailServices.Api.Services
{
    public class EmailSenderService : EmailSender.EmailSenderBase
    {
        private readonly IEmailService _emailSender;
        private readonly EmailSender.EmailSenderClient _client;

        public EmailSenderService(IEmailService emailSender)
        {
            _emailSender = emailSender;
            // Khởi tạo gRPC client
            var channel = GrpcChannel.ForAddress("https://localhost:7089"); 
            _client = new EmailSender.EmailSenderClient(channel);
        }

        public override async Task<EmailResponse> SendEmail(EmailRequest request, ServerCallContext context)
        {
            // Gửi email
            _emailSender.SendEmail(request.To, request.Subject, request.Body);

            // Gửi email qua gRPC
            var grpcRequest = new GrpcEmailService.EmailRequest
            {
                To = request.To,
                Subject = request.Subject,
                Body = request.Body
            };

            // Gọi phương thức từ service 2 qua gRPC client
            var grpcResponse = await _client.SendEmailAsync(grpcRequest);
            // Trả về phản hồi
            return new EmailResponse
            {
                Message = $"Email đã được gửi đến {request.To}, cảm ơn bạn đã sử dụng dịch vụ của chúng tôi."
            };
        }
    }
}

