using ChatBotBroker.Bots;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBotBroker.Services
{
    public class BotService
    {
        private readonly BotBroker _botBroker;
        private readonly BotResponseProducerRMQ _botResponseProducerRMQ;

        public BotService(BotResponseProducerRMQ botResponseProducerRMQ)
        {
            _botBroker = new BotBroker();
            RegisterBotsToBroker();

            _botResponseProducerRMQ = botResponseProducerRMQ;
        }

        public void HandleCommand(string command)
        {
            var bot = _botBroker.GetBotForCommand(command);
            var botMessage = bot.ExecuteActions(command);

            _botResponseProducerRMQ.SendToSignalR(botMessage);
        }

        private void RegisterBotsToBroker()
        {
            _botBroker.Register(new StockBot());
        }
    }
}
