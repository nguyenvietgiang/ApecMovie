using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Connection;
using System.Text;
using System.Text.Json;


namespace RabbitMQ.Event
{
    public class RabbitmqProducer : IMessageProducer
    {
        private readonly IRabbitmqConnection _connection;

        public RabbitmqProducer(IRabbitmqConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public void SendMessage<T>(T message, string queueName)
        {
            if (_connection == null || _connection.Connection == null)
            {
                throw new InvalidOperationException("Không kết nối được vơi rabbitMQ.");
            }

            using (var channel = _connection.Connection.CreateModel())
            {
                try
                {
                    channel.QueueDeclare(queue: queueName, // Sử dụng tên hàng đợi được truyền vào
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    // Sử dụng định dạng JSON để serialize message
                    var json = JsonSerializer.Serialize(message);
                    var body = Encoding.UTF8.GetBytes(json);

                    channel.BasicPublish(exchange: "",
                                         routingKey: queueName, 
                                         basicProperties: null,
                                         body: body);
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi khi gửi message
                    Console.WriteLine($"Có lỗi khi gửi message: {ex.Message}");
                }
            }
        }


        public void ReceiveMessage<T>(Action<T> messageHandler, string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentException("Không thể lắng nghe.", nameof(queueName));
            }

            if (_connection.Connection == null)
            {
                throw new InvalidOperationException("Không kết nối được.");
            }

            using (var channel = _connection.Connection.CreateModel())
            {
                try
                {
                    channel.QueueDeclare(queue: queueName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(body));
                        messageHandler(message);
                    };

                    channel.BasicConsume(queue: queueName,
                                         autoAck: true,
                                         consumer: consumer);

                    Console.WriteLine($"Waiting for messages in queue '{queueName}'.");
                    Console.ReadLine(); // Để cho ứng dụng không kết thúc ngay sau khi nhận tin nhắn
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"có lỗi: {ex.Message}");
                }
            }
        }
    }
}
