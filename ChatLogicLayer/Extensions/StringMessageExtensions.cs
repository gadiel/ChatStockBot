using RabbitMQ.Client;
using System;
using System.Text;

namespace ChatLogicLayer.Extensions
{
    public static class StringMessageExtensions
    {
        public static bool IsBotCommand(this string message)
        {
            return message.StartsWith('/');
        }
        public static void SendToBotBroker(this string message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "botbroker",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "botbroker",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }
        }
    }
}
