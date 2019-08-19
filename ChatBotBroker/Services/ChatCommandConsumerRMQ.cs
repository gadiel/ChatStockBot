using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBotBroker.Services
{
    public class ChatCommandConsumerRMQ
    {
        private ConnectionFactory factory { get; set; }
        private IConnection connection { get; set; }
        private IModel channel { get; set; }

        private readonly BotService _botService;

        public ChatCommandConsumerRMQ(BotService botService)
        {
            factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            _botService = botService;
        }

        public void Register()
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

                _botService.HandleCommand(message);
            };
            channel.BasicConsume(queue: "botbroker",
                                 autoAck: true,
                                 consumer: consumer);
        }

        public void Deregister()
        {
            connection.Close();
        }
    }
}
