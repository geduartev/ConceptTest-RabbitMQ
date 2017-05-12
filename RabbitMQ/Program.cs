using RabbitMQ.Acknowledgement;
using RabbitMQ.Exchanges;
using RabbitMQ.Routing;
using RabbitMQ.RPC;
using RabbitMQ.TopicExchanges;
using System;
using RabbitMQ.BasicSendAndReceive;

namespace RabbitMQ
{
    class Program
    {
        static void Main(string[] args)
        {
            string option = string.Empty;

            while (!option.Equals("0"))
            {
                Console.Clear();
                Console.WriteLine("RABBIT MQ");
                Console.WriteLine("\n");

                Console.WriteLine("Menú Principal");
                Console.WriteLine();

                Console.WriteLine("1. Envío básico.");
                Console.WriteLine("2. Recepción básica.");
                Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");

                Console.WriteLine("3. Envío de mensajes a dos o más colas.");
                Console.WriteLine("4. Recepción de mensajes desde dos o más colas de trabajo.");
                Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");

                Console.WriteLine("5. Envío de un mensaje a un intercambiador y no directamente a una cola (Transmite mensajes a todos los consumidores).");
                Console.WriteLine("6. Recepción de mensajes desde una o cualquier cola enlazada con el intercambiador especificado.");
                Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");

                Console.WriteLine("7. Envío de un mensaje con clave de enrutamiento. (Filtrar mensajes de acuerdo a su gravedad).");
                Console.WriteLine("8. Recepción de mensajes desde una cola enlazada con la clave de enrutamiento.");
                Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");

                Console.WriteLine("9. Envío de un mensaje con clave de enrutamiento. (Filtrar mensajes de acuerdo a su gravedad).");
                Console.WriteLine("101 - 102 - 103 - 104. Recepción de mensajes desde una colas enlazada con la clave de enrutamiento.");
                Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");

                Console.WriteLine("11. Enviando solicitud y esperar como respuesta la serie de fibonacci del número 30.");
                Console.WriteLine("12. Servidor esperando solicitudes para responder la serie de fibonacci solicitada.");
                Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");

                Console.WriteLine("0. Salir del Programa.");

                option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        BasicCommunication.Send();
                        break;
                    case "2":
                        BasicCommunication.Receive();
                        break;

                    case "3":
                        NewTask.Send(new string[] { "Primer mensaje que emula procesar durante 1 segundo." });
                        NewTask.Send(new string[] { "Segundo mensaje que emula procesar durante 2 segundos.." });
                        NewTask.Send(new string[] { "Tercer mensaje que emula procesar durante 1 segundo." });
                        break;
                    case "4":
                        Worker.Receive();
                        break;

                    case "5":
                        EmitLogFanout.Send(new string[] { "Enviando un mensaje al intercambiador, ahora no se envía directamente a la cola. " });
                        break;
                    case "6":
                        ReceiveLogsFanout.Receive();
                        break;

                    case "7":
                        EmitLogDirect.Send(new string[] { "info", "Mensaje de información." });
                        EmitLogDirect.Send(new string[] { "warning", "Mensaje de advertencia." });
                        EmitLogDirect.Send(new string[] { "error", "Mensaje de error." });
                        EmitLogDirect.Send(new string[] { "personalizado", "Mensaje de clasificación personalizado." });
                        break;
                    case "8":
                        ReceiveLogsDirect.Receive(new string[] { "warning", "error", "personalizado" });
                        break;

                    case "9":
                        EmitLogTopic.Send(new string[] { "kern.critical", "un mensaje error crítico del kernel." });
                        EmitLogTopic.Send(new string[] { "kern.*", "Mensaje general del kernel." });
                        EmitLogTopic.Send(new string[] { "*.critital", "Mensaje crítico sin importar de quien sea." });
                        break;
                    case "101":
                        ReceiveLogsTopic.Receive(new string[] { "#" });
                        break;
                    case "102":
                        ReceiveLogsTopic.Receive(new string[] { "kern.*" });
                        break;
                    case "103":
                        ReceiveLogsTopic.Receive(new string[] { "*.critical" });
                        break;
                    case "104":
                        ReceiveLogsTopic.Receive(new string[] { "kern.*", "*.critical" });
                        break;

                    case "11":
                        RPCClient rpcClient = new RPCClient();
                        Console.WriteLine(" [x] Solicitando fib(30)...");
                        var response = rpcClient.Call("30");
                        Console.WriteLine(" [.] Recibido {0}", response);
                        Console.ReadLine();
                        rpcClient.Close();
                        break;
                    case "12":
                        RPCServer rpcServer = new RPCServer();
                        rpcServer.Server();
                        break;
                    default:
                        break;
                }
            }

        }
    }
}
