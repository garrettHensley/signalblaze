using Microsoft.AspNetCore.SignalR;
using SignalBlaze.Services;
using System.Collections.Concurrent;

namespace SignalBlaze.Hubs
{
    public class ChatHub : Hub
    {
        // The "Source of Truth" for who is actually connected right now
        private static ConcurrentDictionary<string, string> OnlineUsers = new();

        private readonly IMessageService _messageService;

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task SendMessage(string user, string message)
        {
            _messageService.AddMessage(user, message);
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            // Add by ConnectionId. If they refresh, they get a new ID, 
            // and the old one eventually times out/removes.
            OnlineUsers.TryAdd(Context.ConnectionId, "Guest");

            var messages = _messageService.GetMessages();
            await Clients.Caller.SendAsync("ReceiveMessageHistory", messages);

            await UpdateUserCount();
            await Clients.All.SendAsync("ReceiveMessage", "System", "Someone joined the chat.");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Remove by ConnectionId so the count stays honest
            OnlineUsers.TryRemove(Context.ConnectionId, out _);

            await UpdateUserCount();
            await Clients.All.SendAsync("ReceiveMessage", "System", "Someone left the chat.");

            await base.OnDisconnectedAsync(exception);
        }

        private async Task UpdateUserCount()
        {
            // Broadcast the count of the dictionary keys
            await Clients.All.SendAsync("ReceiveUserCount", OnlineUsers.Count);
        }
    }
}
