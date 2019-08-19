using ChatBot.Models;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace ChatBotBroker.Bots
{
    public class StockBot : IGenericBot
    {
        public string BotName => "StockBot";
        public string BotCommandName => "stock";

        public BotResponse ExecuteActions(String command)
        {
            var argumentsMatch = obtainArgs(command);
            var stockSymbol = argumentsMatch.Groups[1].Value.ToUpper();

            var botResult = runBotActions(stockSymbol);
            if(botResult.CompareTo("N/D") == 0)
            {
                return new BotResponse() { BotName = BotName, Message = $"Stock Symbol: {stockSymbol} not found" };
            }

            return new BotResponse() { BotName = BotName, Message = $"{stockSymbol} quote is ${botResult} per share" };
        }

        private string runBotActions(string stock_code)
        {
            var stooqUrl = String.Format("https://stooq.com/q/l/?s={0}&f=sd2t2ohlcv&h&e=csv", stock_code);

            string csvTextFile;
            using (HttpClient httpClient = new HttpClient())
            using (var response = httpClient.GetStringAsync(stooqUrl))
            {
                csvTextFile = response.Result;
            }

            var lines = csvTextFile.Split('\n');
            var stockData = lines[1].Split(',');
            return stockData[6];
        }

        public bool VerifyCommandName(string command)
        {
            return obtainArgs(command).Success;
        }

        private Match obtainArgs(string command)
        {
            return new Regex(@"^/" + BotCommandName+ @"=([\w\.]+)").Match(command);
        }
    }
}
