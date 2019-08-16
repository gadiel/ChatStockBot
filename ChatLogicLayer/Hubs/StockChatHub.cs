using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ChatLogicLayer.Hubs
{
    [Authorize]
    public class StockChatHub : 

        public async Task Send(string message) {
            await Clients.All.SendAsync("Send", username, message);
        }
    }
}
