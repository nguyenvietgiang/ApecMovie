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

                // tách thông tin cần thiết từ Message
                var toAddressPattern = @"To:\s*(.*?),";
                var subjectPattern = @"Subject:\s*(.*?),";
                var bodyPattern = @"Body:\s*(.*)$";

                var toAddress = Regex.Match(message, toAddressPattern).Groups[1].Value.Trim();
                var subject = Regex.Match(message, subjectPattern).Groups[1].Value.Trim();
                var bodyContent = Regex.Match(message, bodyPattern).Groups[1].Value.Trim();

                // Gửi email
                _emailService.SendEmail(toAddress, subject, bodyContent);

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
