using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.SignalR;
using SignalBlaze.Hubs;

namespace SignalBlaze
{
    public class ChatCircuitHandler : CircuitHandler
    {
        private readonly ChatPresence _presence;
        private readonly IHubContext<ChatHub> _hub;

        public ChatCircuitHandler(ChatPresence presence, IHubContext<ChatHub> hub)
        {
            _presence = presence;
            _hub = hub;
        }

        public override async Task OnCircuitOpenedAsync(
                Circuit circuit,
                CancellationToken cancellationToken)
        {
            _presence.OnlineUsers[circuit.Id] = "Guest";

            await _hub.Clients.All.SendAsync(
                "ReceiveUserCount",
                _presence.OnlineUsers.Count,
                cancellationToken);
        }

        public override async Task OnCircuitClosedAsync(
            Circuit circuit,
            CancellationToken cancellationToken)
        {
            _presence.OnlineUsers.TryRemove(circuit.Id, out _);

            await _hub.Clients.All.SendAsync(
                "ReceiveUserCount",
                _presence.OnlineUsers.Count,
                cancellationToken);
        }
    }
}
