﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    /// <summary>
    /// Represents a chatting hub.
    /// </summary>
    public class ChatHub : Hub
    {
        /// <summary>
        /// Send message to all clients in the chatroom.
        /// </summary>
        /// <param name="user">User that sent the message.</param>
        /// <param name="message"></param>
        public Task SendMessage(string user, string message)
            => Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
