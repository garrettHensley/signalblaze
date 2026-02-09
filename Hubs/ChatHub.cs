using Microsoft.AspNetCore.SignalR;

namespace SignalBlaze.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message) {
            // This sends the message to EVERYONE connected
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
