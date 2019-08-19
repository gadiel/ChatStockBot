using ChatBot.Models;
using ChatLogicLayer.Hubs;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLogicLayer.Services
{
    public class BotResponseConsumerRMQ
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

                var botResponse = JsonConvert.DeserializeObject<BotResponse>(message);

                _stockChatHub.Clients.All.SendAsync("Send", botResponse.BotName, botResponse.Message, DateTime.Now);
                Console.WriteLine(" [x] Received {0}", message);
            };
            channel.BasicConsume(queue: "botresponsetosignalr", autoAck: true, consumer: consumer);
        }

        public void Deregister()
        {
            this.connection.Close();
        }

        public BotResponseConsumerRMQ(IHubContext<StockChatHub> stockChatHub)
        {
            factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            _stockChatHub = stockChatHub;
        }
    }

}
