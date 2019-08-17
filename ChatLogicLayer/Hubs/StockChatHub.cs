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
            await Clients.All.SendAsync("Send", username, message);
        }
    }
    
}
