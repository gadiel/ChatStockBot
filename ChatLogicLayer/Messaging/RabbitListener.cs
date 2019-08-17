using ChatLogicLayer.Hubs;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLogicLayer.Messaging
{
    public class RabbitListener
    {
        private ConnectionFactory factory { get; set; }
        private IConnection connection { get; set; }
        private IModel channel { get; set; }

        private readonly IHubContext<StockChatHub> _stockChatHub;

        public void Register()
        {
            channel.QueueDeclare(queue: "botresponsetosignalr", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var messageParts = message.Split('~');
                _stockChatHub.Clients.All.SendAsync("Send", messageParts[0], messageParts[1]);
                Console.WriteLine(" [x] Received {0}", message);
            };
            channel.BasicConsume(queue: "botresponsetosignalr", autoAck: true, consumer: consumer);
        }

        public void Deregister()
        {
            this.connection.Close();
        }

        public RabbitListener(IHubContext<StockChatHub> stockChatHub)
        {
            factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            _stockChatHub = stockChatHub;
        }
    }

}
