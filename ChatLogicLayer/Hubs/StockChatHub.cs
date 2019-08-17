using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ChatLogicLayer.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RabbitMQ.Client;
using System.Text;

namespace ChatLogicLayer.Hubs
{
    [Authorize]
    public class StockChatHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StockChatHub(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Send(string message) {
            ///var user = await _userManager.GetUserAsync(User);
            var username = _httpContextAccessor.HttpContext.User.Identity.Name;
            
            if (message.IsBotCommand())
            {
                String.Format("{0}~{1}", username, message).SendToBotBroker();
            }
            else
            {
                await Clients.All.SendAsync("Send", username, message);
            }
        }
    }

    public static class StringExtentions
    {
        public static bool IsBotCommand(this string message)
        {
            return message.StartsWith('/');
        }
        public static void SendToBotBroker(this string message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "botbroker",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "botbroker",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }
        }
    }
}
