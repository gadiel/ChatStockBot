using System;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace ChatBotBroker.Bots
{
    public class StockBot : IGenericBot
    {
        public string BotName => "StockBot";
        public string BotCommandName => "stock";

        public string ExecuteActions(String command)
        {
            var argumentsMatch = obtainArgs(command);

            var botResult = runBotActions(argumentsMatch.Groups[1].Value);
            if(botResult.CompareTo("N/D") == 0)
            {
                return $"Stock Symbol: {argumentsMatch.Groups[1].Value.ToUpper()} not found";
            }
            return String.Format("{0} quote is ${1} per share", argumentsMatch.Groups[1].Value.ToUpper(), botResult);
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

    public interface IGenericBot
    {
        string BotCommandName { get; }

        bool VerifyCommandName(string command);
        string ExecuteActions(string command);
        string BotName { get; }
    }
}
