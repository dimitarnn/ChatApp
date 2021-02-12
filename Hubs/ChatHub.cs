using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApp.Data;
using ChatApp.Hubs;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        public async Task Send(Message message)
        {
            await this.Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessage",
                new Message { Name = "Bot", Text = $"{Context.ConnectionId} has connected to group {groupName}." } );
        }
    }
}
