using ChatBot.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBotBroker.Services
{
    public class BotResponseProducerRMQ
    {
        private readonly RabbitMQSettings _rabbitMQSettings;

        public BotResponseProducerRMQ(IOptions<RabbitMQSettings> settings)
        {
            _rabbitMQSettings = settings.Value;
        }

        public void SendToSignalR(BotResponse botResponse)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: _rabbitMQSettings.BotResponseQueue.Name,
                    durable: _rabbitMQSettings.BotResponseQueue.Durable,
                    exclusive: _rabbitMQSettings.BotResponseQueue.Exclusive,
                    autoDelete: _rabbitMQSettings.BotResponseQueue.AutoDelete,
                    arguments: null);

                var serializedBotResponse = JsonConvert.SerializeObject(botResponse);
                var responseBody = Encoding.UTF8.GetBytes(serializedBotResponse);

                channel.BasicPublish(exchange: "",
                                     routingKey: _rabbitMQSettings.BotResponseQueue.Name,
                                     basicProperties: null,
                                     body: responseBody);

                Console.WriteLine(" [x] Sent {0}", serializedBotResponse);
            }
        }
    }
}
