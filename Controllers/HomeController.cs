using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChatApp.Models;
using ChatApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using ChatApp.Hubs;

namespace ChatApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IHubContext<ChatHub> _chat;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, IHubContext<ChatHub> chat)
        {
            _logger = logger;
            _context = context;
            _chat = chat;
        }

        [HttpGet]
        public IActionResult Chat(int id)
        {
            var chat = _context.Chats
                .Include(x => x.Messages)
                .FirstOrDefault(x => x.Id == id);
            return View(chat);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int chatId, string message)
        {
            var newMessage = new Message {
                ChatId = chatId,
                Text = message,
                Name = User.Identity.Name,
                TimeSpan = DateTime.Now
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            return RedirectToAction("Chat", new { id = chatId });
        }

        public IActionResult Index()
        {
            var chatId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var chats = _context.Chats
                .Include(x => x.Users)
                .Where(x => !x.Users.Any(y => y.UserId == chatId))
                .ToList();

            return View(chats);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(string name)
        {
            var chat = new Chat
            {
                Name = name,
                Type = ChatType.Room
            };

            chat.Users.Add(new ChatUser
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Role = UserRole.Admin
            });

            _context.Chats.Add(chat);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> JoinRoom(int id)
        {
            var chatUser = new ChatUser
            {
                ChatId = id,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Role = UserRole.Member
            };

            _context.ChatUsers.Add(chatUser);

            await _context.SaveChangesAsync();

            return RedirectToAction("Chat", "Home", new { id = id });
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
