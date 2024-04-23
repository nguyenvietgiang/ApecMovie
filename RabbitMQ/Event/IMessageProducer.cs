﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Event
{
    public interface IMessageProducer
    {
        void SendMessage<T>(T message, string queueName);
       // void ReceiveMessage<T>(Action<T> messageHandler, string queueName);
    }
}
