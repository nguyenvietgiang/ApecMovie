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
                                         durable: false,   // True để khi server bị sập sẽ không bị mất tin 
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    // Sử dụng định dạng JSON để serialize message
                    var json = JsonSerializer.Serialize(message);
                    var body = Encoding.UTF8.GetBytes(json);

                    // đang không sử dụng exchange mà đẩy thẳng vào queue
                    channel.BasicPublish(exchange: "",
                                         routingKey: queueName, 
                                         basicProperties: null,
                                         body: body);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Có lỗi khi gửi message: {ex.Message}");
                }
            }
        }

    }
}
