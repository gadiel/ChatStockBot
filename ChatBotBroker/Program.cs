using ChatBotBroker.Bots;
using ChatBotBroker.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ChatBotBroker
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
        
        static void Main(string[] args)
        {
            RegisterServices();

            _serviceProvider.GetService<ChatCommandConsumerRMQ>()
                            .Register();

            Console.WriteLine(" Press ctrl+c to exit.");
            while (true)
            {
                Console.ReadLine();
            }
        }

        private static void RegisterServices()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<ChatCommandConsumerRMQ>();
            collection.AddTransient<BotResponseProducerRMQ>();
            collection.AddSingleton<BotService>();

            _serviceProvider = collection.BuildServiceProvider();
        }
    }
}
