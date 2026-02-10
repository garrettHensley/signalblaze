using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace SignalBlaze.Hubs
{
    public class ChatHub : Hub
    {
        // Thread-safe dictionary to store <ConnectionId, Username>
        private static readonly ConcurrentDictionary<string, string> OnlineUsers = new();

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            // We don't know their name yet, so we use a placeholder
            OnlineUsers.TryAdd(Context.ConnectionId, "Guest");
            await UpdateUserCount();
            await Clients.All.SendAsync("ReceiveMessage", "System", "Someone joined the chat.");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            OnlineUsers.TryRemove(Context.ConnectionId, out _);
            await UpdateUserCount();
            await Clients.All.SendAsync("ReceiveMessage", "System", "Someone left the chat.");
            await base.OnDisconnectedAsync(exception);
        }

        private async Task UpdateUserCount()
        {
            // Tell all clients how many people are online
            await Clients.All.SendAsync("ReceiveUserCount", OnlineUsers.Count);
        }
    }
}
