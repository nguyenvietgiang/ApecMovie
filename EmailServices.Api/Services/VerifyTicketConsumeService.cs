using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.RegularExpressions;

namespace EmailServices.Api.Services
{
        public class VerifyTicketConsumeService : IHostedService
        {
            private readonly ConnectionFactory _factory;
            private IConnection _connection;
            private IModel _channel;
            private readonly IEmailService _emailService;
            public VerifyTicketConsumeService(IEmailService emailService)
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

                _channel.QueueDeclare(queue: "verifyticket",
                                      durable: false,
                                      exclusive: false,
                                      autoDelete: false,
                                      arguments: null);

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Received ticket: {0}", message);

                    // Biểu thức chính quy để tách thông tin
                    var toAddressPattern = @"To:\s*(.+?),\s*";
                    var subjectPattern = @"Subject:\s*(.+?),\s*";
                    var ticketPattern = @"TickedID:\s*(.+?),\s*";
                    var tokenPattern = @"Token:\s*(.+)";

                    var toAddressMatch = Regex.Match(message, toAddressPattern);
                    var subjectMatch = Regex.Match(message, subjectPattern);
                    var ticketMatch = Regex.Match(message, ticketPattern);
                    var tokenMatch = Regex.Match(message, tokenPattern);

                    if (toAddressMatch.Success && subjectMatch.Success && ticketMatch.Success && tokenMatch.Success)
                    {
                        var toAddress = toAddressMatch.Groups[1].Value.Trim();
                        var subject = subjectMatch.Groups[1].Value.Trim();
                        var ticket = ticketMatch.Groups[1].Value.Trim();
                        var token = tokenMatch.Groups[1].Value;

                        // Gửi email
                        _emailService.VerifyEmail(toAddress, subject, ticket, token);
                    }
                    else
                    {
                        Console.WriteLine("Invalid message format.");
                    }
                };
                _channel.BasicConsume(queue: "verifyticket",
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
