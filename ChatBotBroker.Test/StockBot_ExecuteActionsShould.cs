using ChatBotBroker.Bots;
using System;
using Xunit;

namespace ChatBotBroker.Test
{
    public class StockBot_ExecuteActionsShould
    {

        private readonly IGenericBot _stockBot;

        public StockBot_ExecuteActionsShould()
        {
            _stockBot = new StockBot();
        }

        [Fact]
        public void StockBotNameShouldBe()
        {
            var result = _stockBot.BotCommandName;

            Assert.Matches("^stock", result);
        }

        [Theory]
        [InlineData("/stock=AAAA.AA")]
        [InlineData("/stock=aaaa1")]
        public void StockBotVerifyCommandName(string value)
        {
            var result = _stockBot.VerifyCommandName(value);

            Assert.True(result, $"{value} should be a correct CommandName");
        }

        [Theory]
        [InlineData("/stuck=AAAA.AA")]
        [InlineData("\\stock=AAAA.AA")]
        [InlineData("//stock=AAAA.AA")]
        public void StockBotVerifyCommandNameShouldBeFalse(string value)
        {
            var result = _stockBot.VerifyCommandName(value);

            Assert.False(result, $"{value} should be an incorrect CommandName");
        }

        [Theory]
        [InlineData("/stock=AFGH.US")]
        [InlineData("/stock=AGZ.US")]
        [InlineData("/stock=144.HK")]
        [InlineData("/stock=2768.JP")]
        public void StockBotExecuteCommandResultShouldBe(string value)
        {
            var result = _stockBot.ExecuteActions(value);
            Assert.Contains("quote is", result.Message);
        }

        [Theory]
        [InlineData("/stock=XXXXX.XX")]
        [InlineData("/stock=YYYZZZ")]
        [InlineData("/stock=NON.EX1STANT")]
        [InlineData("/stock=FICT.NON")]
        public void StockBotExecuteCommandResultShouldBeUnavailable(string value)
        {
            var result = _stockBot.ExecuteActions(value);
            Assert.Contains("not found", result.Message);
        }

    }
}
