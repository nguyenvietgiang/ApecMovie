using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.RegularExpressions;

namespace EmailServices.Api.Services
{
    public class ConsumerService : IHostedService
    {
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;
        private readonly IEmailService _emailService;
        public ConsumerService(IEmailService emailService)
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            _emailService = emailService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "sendmail",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Received message: {0}", message);

                // Biểu thức chính quy để tách thông tin
                var toAddressPattern = @"To:\s*(.+?),";
                var subjectPattern = @"Subject:\s*(.+?),";
                var bodyPattern = @"Body:\s*(.*)(?="")";

                var toAddressMatch = Regex.Match(message, toAddressPattern);
                var subjectMatch = Regex.Match(message, subjectPattern);
                var bodyMatch = Regex.Match(message, bodyPattern);

                if (toAddressMatch.Success && subjectMatch.Success && bodyMatch.Success)
                {
                    var toAddress = toAddressMatch.Groups[1].Value.Trim();
                    var subject = subjectMatch.Groups[1].Value.Trim();
                    var bodyContent = bodyMatch.Groups[1].Value;

                    // Loại bỏ dấu ngoặc kép ở cuối chuỗi body (nếu có)
                    if (bodyContent.EndsWith("\""))
                    {
                        bodyContent = bodyContent.TrimEnd('"');
                    }

                    // Gửi email
                    _emailService.SendEmail(toAddress, subject, bodyContent);
                }
                else
                {
                    Console.WriteLine("Invalid message format.");
                }
            };
            _channel.BasicConsume(queue: "sendmail",
                                  autoAck: true,
                                  consumer: consumer);

            Console.WriteLine("Consumer started.");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection?.Close();
            return Task.CompletedTask;
        }
    }
}
