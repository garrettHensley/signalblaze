using System.Collections.Concurrent;

namespace SignalBlaze.Services
{
    public interface IMessageService
    {
        // Define methods for message handling here
        void AddMessage(string user, string text);
        IEnumerable<Message> GetMessages();
        void PruneMessages();
    }

    public class MessageService : IMessageService
    {
        private readonly ConcurrentDictionary<Guid, Message> _store = new();

        private static int _maxMessages = 100; // Example limit for pruning
        private static int _maxMessageAgeMinutes = 1000; // Example age limit for pruning

        public void AddMessage(string user, string text)
        {
            // Implementation to add a message
            PruneMessages();
            var newMessage = new Message(Guid.NewGuid(), user, text, DateTime.UtcNow);
            _store[newMessage.Id] = newMessage;
        }

        public IEnumerable<Message> GetMessages()
        {
            // Implementation to retrieve messages
            var messages = _store.Values.OrderBy(m => m.DateCreated).ToList();
            return _store.Values;
        }

        public void PruneMessages()
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-_maxMessageAgeMinutes);

            // 1. Remove by Time (The "Expiration" logic)
            foreach (var (id, msg) in _store)
            {
                if (msg.DateCreated < cutoff)
                {
                    _store.TryRemove(id, out _);
                }
            }

            // 2. Remove by Count (The "Overflow" logic)
            // If we're still over the limit, remove the oldest ones
            if (_store.Count > _maxMessages)
            {
                var toRemove = _store.Values
                    .OrderBy(m => m.DateCreated)
                    .Take(_store.Count - _maxMessages);

                foreach (var msg in toRemove)
                {
                    _store.TryRemove(msg.Id, out _);
                }
            }
        }
    }
}
