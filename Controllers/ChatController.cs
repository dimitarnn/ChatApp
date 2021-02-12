using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApp.Data;
using ChatApp.Hubs;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> _chat;
        private readonly AppDbContext _context;

        public ChatController(IHubContext<ChatHub> chat, AppDbContext context)
        {
            _chat = chat;
            _context = context;
        }

        [HttpPost("[action]/{connectionId}/{roomName}")]
        public async Task<IActionResult> JoinRoom(string connectionId, string roomName)
        {
            await _chat.Groups.AddToGroupAsync(connectionId, roomName);
            return Ok();
        }

        [HttpPost("[action]/{connectionId}/{roomName}")]
        public async Task<IActionResult> LeaveRoom(string connectionId, string roomName)
        {
            await _chat.Groups.RemoveFromGroupAsync(connectionId, roomName);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessage(int chatId, string message, string roomName)
        {
            var newMessage = new Message
            {
                ChatId = chatId,
                Text = message,
                Name = User.Identity.Name,
                TimeSpan = DateTime.Now
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            //await _chat.Clients.All.SendAsync("ReceiveMessage", newMessage);
            //var groups = _chat.Clients.Group(roomName);
            await _chat.Clients.Group(roomName).SendAsync("ReceiveMessage", newMessage);
            return Ok();
        }
    }
}
