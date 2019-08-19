using ChatBot.Models;
using ChatBotBroker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

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

            var configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

            collection.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));

            _serviceProvider = collection.BuildServiceProvider();
        }
    }
}
