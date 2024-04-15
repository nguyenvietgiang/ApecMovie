using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Connection
{
    public interface IRabbitmqConnection
    {
        IConnection Connection { get; }
    }
}
