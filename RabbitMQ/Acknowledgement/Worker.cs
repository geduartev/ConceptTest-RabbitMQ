using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Acknowledgement
{
    class Worker
    {
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
                        queue: "task_queue",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    // No darle más de un mensaje a cada worker a la vez.
                    // No enviar un nuevo mensaje a un worker hasta que haya procesado y reconocido el anterior.
                    // En su lugar lo enviará al siguiente trabajador que todavía no está ocupado.
                    // Si todos los workers están ocupados, su cola puede llenarse.
                    // Usted querrá mantener un ojo en eso, y tal vez agregar más trabajadores, o tener alguna otra estrategia.
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    Console.WriteLine(" [*] Esperando por mensajes...");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Recibido {0}", message);

                        int dots = message.Split('.').Length - 1;
                        Thread.Sleep(dots * 1000); // Emula una tarea ocupada durante una cantidad de tiempo.

                        // Informar sobre los mensajes entrega
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false); // Esta línea no debe ir si en BasicConsume noAck: true
                    };

                    channel.BasicConsume(
                        queue: "task_queue",
                        noAck: false, // "False" cuando se quiere corroborar que el mensaje se recibió correctamente para ser eliminado de la cola.
                        consumer: consumer);


                    Console.WriteLine(" Presiones [ENTER] para volver al menú.");
                    Console.ReadLine();
                }
            }
        }
    }
}
