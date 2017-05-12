using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.RPC
{
    class RPCClient
    {
        #region Fields

        private static IConnection connection;
        private static IModel channel;
        private static string replyQueueName;
        private static QueueingBasicConsumer consumer;

        #endregion

        public RPCClient()
        {
            var connectionFactory = new ConnectionFactory() { HostName = "localhost" };
            connection = connectionFactory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(
                queue: replyQueueName,
                noAck: true,
                consumer: consumer);
        }

        public string Call(string message)
        {
            var correlationalId = Guid.NewGuid().ToString();
            var props = channel.CreateBasicProperties();
            props.ReplyTo = replyQueueName;
            props.CorrelationId = correlationalId;

            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(
                exchange: "",
                routingKey: "rpc_queue",
                basicProperties: props,
                body: messageBytes);

            // Esperando respuesta de la solicitud...
            while (true)
            {
                var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                // Si la solicitud tiene el mismo correlationalId la retorna.
                if (ea.BasicProperties.CorrelationId == correlationalId)
                {
                    return Encoding.UTF8.GetString(ea.Body);
                }
            }
        }

        public void Close()
        {
            connection.Close();
        }
    }
}
