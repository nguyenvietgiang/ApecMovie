using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Event
{
    public class RabbitMQConsumer : IDisposable
    {
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public event EventHandler<string> MessageReceived;

        public RabbitMQConsumer()
        {
            _queueName = "sendmail";

            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                OnMessageReceived(message); // Kích hoạt sự kiện MessageReceived
            };

            _channel.BasicConsume(queue: _queueName,
                autoAck: true,
                consumer: consumer);
        }

        protected virtual void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(this, message);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}


