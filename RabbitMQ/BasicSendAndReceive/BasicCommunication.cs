using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.BasicSendAndReceive
{
    class BasicCommunication
    {
        public static void Send()
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
                        queue: "ColaParaSaludos",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    string message = "¡Hola Mundo!";

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: "ColaParaSaludos",
                        basicProperties: null,
                        body: body);

                    Console.WriteLine(" [x] Enviado {0}", message);
                }

                Console.WriteLine(" Presione [ENTER] para volver al menú.");
                Console.ReadLine();
            }
        }

        public static void Receive()
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
                        queue: "ColaParaSaludos",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Recibido {0}", message);
                    };

                    channel.BasicConsume(
                        queue: "ColaParaSaludos",
                        noAck: true,
                        consumer: consumer);

                    Console.WriteLine(" Presiones [ENTER] para volver al menú.");
                    Console.ReadLine();
                }
            }
        }
    }
}
