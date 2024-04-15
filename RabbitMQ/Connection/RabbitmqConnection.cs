using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Connection
{
    public class RabbitmqConnection : IRabbitmqConnection, IDisposable
    {
        private IConnection? _connection;

        public IConnection Connection => _connection!;

        public RabbitmqConnection()
        {
            InitialazeConnection();
        }

        private void InitialazeConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            _connection = factory.CreateConnection();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }

}
