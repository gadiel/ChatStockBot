﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ChatLogicLayer.Data;
using Microsoft.AspNetCore.Http;
using ChatLogicLayer.Extensions;
using ChatLogicLayer.Data.Models;
using ChatBot.Models;
using Microsoft.Extensions.Options;

namespace ChatLogicLayer.Hubs
{
    [Authorize]
    public class StockChatHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly RabbitMQSettings _rabbitMQSettings;

        public StockChatHub(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ApplicationDbContext applicationDbContext, IOptions<RabbitMQSettings> settings)
        {
            _rabbitMQSettings = settings.Value;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _applicationDbContext = applicationDbContext;
        }

        public async Task Send(string message) {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            
            if (message.IsBotCommand())
            {
                message.SendToBotBroker(_rabbitMQSettings);
            }
            else
            {
                await _applicationDbContext.ChatMessages.AddAsync(new ChatMessage { ApplicationUser = user, Message = message, CreationDate = DateTime.Now });
                await _applicationDbContext.SaveChangesAsync();
                await Clients.All.SendAsync("Send", user.Nick, message, DateTime.Now);
            }
        }
    }
}
