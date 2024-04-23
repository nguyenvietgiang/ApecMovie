﻿using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace RabbitMQ.Event
{
    public class ConsumerService : IHostedService
    {
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;

        public ConsumerService()
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost"
            };
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