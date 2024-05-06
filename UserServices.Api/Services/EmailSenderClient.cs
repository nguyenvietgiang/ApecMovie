using Grpc.Net.Client;
using GrpcEmailService;

public class EmailSenderClient
{
    private readonly EmailSender.EmailSenderClient _client;

    public EmailSenderClient(GrpcChannel channel)
    {
        _client = new EmailSender.EmailSenderClient(channel);
    }

    public async Task<EmailResponse> SendEmailAsync(string to, string subject, string body)
    {
        var request = new EmailRequest
        {
            To = to,
            Subject = subject,
            Body = body
        };

        return await _client.SendEmailAsync(request);
    }
}