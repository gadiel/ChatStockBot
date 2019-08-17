using ChatBotBroker.Bots;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ChatBotBroker
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var _botService = new BotService();
            _botService.Register(new StockBot());

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "botbroker",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine(" [x] Received command {0}", message);
                };
                channel.BasicConsume(queue: "botbroker",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press ctrl+c to exit.");
                while (true)
                {
                    Console.ReadLine();
                }
            }
        }
    }
}
