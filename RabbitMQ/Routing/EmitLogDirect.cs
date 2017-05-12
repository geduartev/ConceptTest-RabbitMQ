using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace RabbitMQ.Routing
{
    class EmitLogDirect
    {
        public static void Send(string[] args)
        {
            Console.Clear();

            var connectionFactory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "direct_logs", type: "direct");

                var severity = (args.Length > 0) ? args[0] : "info"; // info, warning, error

                var message = (args.Length > 0) ? string.Join(" ", args.Skip(1).ToArray()) : "¡Hola Mundo!";

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: "direct_logs",
                    routingKey: severity,
                    basicProperties: null,
                    body: body);

                Console.WriteLine(" [x] Enviado {0}", message);
            }

            Console.WriteLine(" Presione [ENTER] para volver al menú.");
            Console.ReadLine();
        }
    }
}
