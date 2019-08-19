using ChatBot.Models;
using ChatLogicLayer.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
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
        private readonly RabbitMQSettings _rabbitMQSettings;

        public void Register()
        {
            channel.QueueDeclare(
                queue: _rabbitMQSettings.BotResponseQueue.Name,
                durable: _rabbitMQSettings.BotResponseQueue.Durable,
                exclusive: _rabbitMQSettings.BotResponseQueue.Exclusive,
                autoDelete: _rabbitMQSettings.BotResponseQueue.AutoDelete,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);

                var botResponse = JsonConvert.DeserializeObject<BotResponse>(message);

                _stockChatHub.Clients.All.SendAsync("Send", botResponse.BotName, botResponse.Message, DateTime.Now);
                Console.WriteLine(" [x] Received {0}", message);
            };
            channel.BasicConsume(queue: _rabbitMQSettings.BotResponseQueue.Name, autoAck: true, consumer: consumer);
        }

        public void Deregister()
        {
            this.connection.Close();
        }

        public BotResponseConsumerRMQ(IHubContext<StockChatHub> stockChatHub, IOptions<RabbitMQSettings> settings)
        {
            _rabbitMQSettings = settings.Value;
            _stockChatHub = stockChatHub;

            factory = new ConnectionFactory() {
                HostName = _rabbitMQSettings.connection.HostName,
                UserName = _rabbitMQSettings.connection.Username,
                Password = _rabbitMQSettings.connection.Password
            };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }
    }

}
