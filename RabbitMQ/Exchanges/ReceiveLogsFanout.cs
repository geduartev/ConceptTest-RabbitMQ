using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Exchanges
{
    class ReceiveLogsFanout
    {
        public static void Receive()

        {
            Console.Clear();

            var connectionFactory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "logs", type: "fanout");

                var queueName = channel.QueueDeclare().QueueName; // Genera un nombre aleatorio para la cola (queue) enlazada al intercambiador (exchanger).

                channel.QueueBind(
                    queue: queueName,
                    exchange: "logs",
                    routingKey: "");

                Console.WriteLine(" [*] Esperando por los logs.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Recibido {0}", message);
                };

                channel.BasicConsume(
                    queue: queueName,
                    noAck: true,
                    consumer: consumer);

                Console.WriteLine(" Presiones [ENTER] para volver al menú.");
                Console.ReadLine();
            }
        }
    }
}
