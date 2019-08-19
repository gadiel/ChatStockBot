using ChatBot.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBotBroker.Bots
{
    public class NotFoundCommandBot : IGenericBot
    {
        public string BotName => "NotFoundBot";

        public BotResponse ExecuteActions(string command)
        {
            return new BotResponse() { BotName = BotName, Message = "Command not found" };
        }

        public string BotCommandName => "notfound";

        public bool VerifyCommandName(string command)
        {
            return true;
        }
    }
}
