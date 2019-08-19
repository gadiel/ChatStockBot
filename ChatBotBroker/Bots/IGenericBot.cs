using ChatBot.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBotBroker.Bots
{
    public interface IGenericBot
    {
        string BotCommandName { get; }
        bool VerifyCommandName(string command);
        BotResponse ExecuteActions(string command);
        string BotName { get; }
    }
}
