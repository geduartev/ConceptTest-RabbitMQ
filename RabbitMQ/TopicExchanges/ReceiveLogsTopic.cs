using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.TopicExchanges
{
    class ReceiveLogsTopic
    {
        public static void Receive(string[] args)

        {
            Console.Clear();

            var connectionFactory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "topic_logs", type: "topic");

                var queueName = channel.QueueDeclare().QueueName; // Genera un nombre aleatorio para la cola (queue) enlazada al intercambiador (exchanger).

                if (args.Length < 1)
                {
                    Console.Error.WriteLine("Use: {0} [claves de enlace...]", Environment.GetCommandLineArgs()[0]);
                    Console.WriteLine(" Presione [ENTER] para volver al menú.");
                    Console.ReadLine();
                    Environment.ExitCode = 1;
                    return;
                }

                foreach (string bindingKey in args)
                {
                    channel.QueueBind(
                        queue: queueName,
                        exchange: "topic_logs",
                        routingKey: bindingKey);
                }

                Console.WriteLine(" [*] Esperando por los mensajes. Precione CTRL+C para salir.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    Console.WriteLine(" [x] Recibido {0}:{1}", routingKey, message);
                };

                channel.BasicConsume(
                    queue: queueName,
                    noAck: true,
                    consumer: consumer);

                Console.WriteLine(" Presione [ENTER] para volver al menú.");
                Console.ReadLine();
            }
        }
    }
}
