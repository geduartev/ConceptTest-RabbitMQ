using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace RabbitMQ.TopicExchanges
{
    class EmitLogTopic
    {
        public static void Send(string[] args)
        {
            Console.Clear();

            var connectionFactory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "topic_logs", type: "topic");

                var routingKey = (args.Length > 0) ? args[0] : "anonymous.info";

                var message = (args.Length > 0) ? string.Join(" ", args.Skip(1).ToArray()) : "¡Hola Mundo!";

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: "topic_logs",
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);

                Console.WriteLine(" [x] Enviado {0}:{1}", routingKey, message);
            }

            Console.WriteLine(" Presione [ENTER] para volver al menú.");
            Console.ReadLine();
        }
    }
}
