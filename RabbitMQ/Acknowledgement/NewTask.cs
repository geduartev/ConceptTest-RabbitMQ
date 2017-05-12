using System;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQ.Acknowledgement
{
    class NewTask
    {
        public static void Send(string[] args)
        {
            Console.Clear();

            var connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            using (var connection = connectionFactory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: "task_queue",
                        durable: true, // La cola no se perderá aunque Rabbit se cierre.
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var message = GetMessage(args);

                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true; // Habilita persistencia para garantizar que no se borra aunque Rabbit se cierre.

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: "task_queue",
                        basicProperties: properties,
                        body: body);

                    Console.WriteLine(" [x] Enviado {0}", message);
                }

                Console.WriteLine(" Presione [ENTER] para volver al menú.");
                Console.ReadLine();
            }
        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join("", args) : "¡Hola mundo!");
        }
    }
}
