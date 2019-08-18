using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatLogicLayer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ChatLogicLayer.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public IndexModel(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public void OnGet()
        {

        }

        public JsonResult OnGetLast50Messages()
        {
            var chatmessages = _applicationDbContext.ChatMessages
                               .Include(chatmessage => chatmessage.ApplicationUser)
                               .Select(chatmessage => new
                               {
                                   user_nick = chatmessage.ApplicationUser.Nick,
                                   message = chatmessage.Message,
                                   date = chatmessage.CreationDate
                               })
                               .OrderByDescending(c => c.date)
                               .Take(50)
                               .ToList();
            chatmessages.Reverse();

            return new JsonResult(chatmessages);
        }
    }
}