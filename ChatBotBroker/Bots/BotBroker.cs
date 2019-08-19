using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBotBroker.Bots
{
    public class BotBroker
    {
        private List<IGenericBot> _bots { get; }

        public BotBroker()
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
}
