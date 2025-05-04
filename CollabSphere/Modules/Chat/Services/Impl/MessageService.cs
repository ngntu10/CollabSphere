using CollabSphere.Database;
using CollabSphere.Entities.Domain;
using CollabSphere.Modules.Chat.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace CollabSphere.Modules.Chat.Services.Impl
{
    public class MessageService : IMessageService
    {
        private readonly DatabaseContext _context;

        public MessageService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task SaveMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Message>> GetMessagesBetweenUsersAsync(Guid senderId, Guid receiverId)
        {
            return await _context.Messages
                .Where(m =>
                    (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                    (m.SenderId == receiverId && m.ReceiverId == senderId))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
    }
}
