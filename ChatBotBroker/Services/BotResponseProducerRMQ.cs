using ChatBot.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBotBroker.Services
{
    public class BotResponseProducerRMQ
    {
        public void SendToSignalR(BotResponse botResponse)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "botresponsetosignalr",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                var serializedBotResponse = JsonConvert.SerializeObject(botResponse);
                var responseBody = Encoding.UTF8.GetBytes(serializedBotResponse);

                channel.BasicPublish(exchange: "",
                                     routingKey: "botresponsetosignalr",
                                     basicProperties: null,
                                     body: responseBody);

                Console.WriteLine(" [x] Sent {0}", serializedBotResponse);
            }
        }
    }
}
