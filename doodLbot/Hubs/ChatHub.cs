using Microsoft.AspNetCore.SignalR;

using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        public Task SendMessage(string user, string message)
            => this.Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}