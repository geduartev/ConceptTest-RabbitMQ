using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ.Exchanges
{
    class EmitLogFanout
    {
        public static void Send(string[] args)
        {
            Console.Clear();

            var connectionFactory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "logs", type: "fanout");

                var message = GetMessage(args);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: "logs",
                    routingKey: "",
                    basicProperties: null,
                    body: body);

                Console.WriteLine(" [x] Enviado {0}", message);
            }

            Console.WriteLine(" Presione [ENTER] para volver al menú.");
            Console.ReadLine();
        }
        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join("", args) : "¡Hola mundo!");
        }
    }
}
