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
        private readonly ApplicationDbContext _applicationDbContext;

        public StockChatHub(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _applicationDbContext = applicationDbContext;
        }

        public async Task Send(string message) {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            
            if (message.IsBotCommand())
            {
                message.SendToBotBroker();
            }
            else
            {
                await _applicationDbContext.ChatMessages.AddAsync(new ChatMessage { ApplicationUser = user, Message = message, CreationDate = DateTime.Now });
                await _applicationDbContext.SaveChangesAsync();
                await Clients.All.SendAsync("Send", user.Nick, message, DateTime.Now);
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
