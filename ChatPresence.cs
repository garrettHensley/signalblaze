using System.Collections.Concurrent;

namespace SignalBlaze
{
    public class ChatPresence
    {
        public ConcurrentDictionary<string, string> OnlineUsers { get; } = new();
    }
}
