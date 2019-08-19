using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBotBroker.Bots
{
    public class BotService
    {
        private List<IGenericBot> _bots { get; }

        public BotService()
        {
            _bots = new List<IGenericBot>();
        }
        
        public void Register(IGenericBot bot)
        {
            _bots.Add(bot);
        }

        public IGenericBot GetBotForCommand(string command)
        {
            foreach (var bot in _bots)
            {
                if (bot.VerifyCommandName(command))
                {
                    return bot;
                }
            }

            return new NotFoundCommandBot();
        }

    }

    public class NotFoundCommandBot : IGenericBot
    {
        public string BotName => "NotFoundBot";

        public string ExecuteActions(string command)
        {
            return "Command not found";
        }

        public string BotCommandName => "notfound";

        public bool VerifyCommandName(string command)
        {
            return true;
        }
    }
}
