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

        public async Task<List<ChatRoomResponse>> GetConversationsAsync(Guid userId, List<string> activeUsers)
        {
            var conversations = await (from m in _context.Messages
                                       where (m.SenderId == userId || m.ReceiverId == userId) && !m.IsArchived
                                       group m by m.SenderId == userId ? m.ReceiverId : m.SenderId into g
                                       join u in _context.Users on g.Key equals u.Id into users
                                       from u in users.DefaultIfEmpty()
                                       let lastMessage = g.OrderByDescending(m => m.SentAt).FirstOrDefault()
                                       select new ChatRoomResponse
                                       {
                                           PartnerId = g.Key,
                                           PartnerName = u != null ? u.UserName : "Unknown",
                                           PartnerAvatar = u != null ? u.AvatarId : string.Empty,
                                           LastMessage = lastMessage != null ? lastMessage.Content : string.Empty,
                                           LastMessageTime = lastMessage != null ? lastMessage.SentAt : DateTime.MinValue,
                                           UnreadCount = g.Count(m => m.ReceiverId == userId && m.ReadStatus == false),
                                           IsOnline = activeUsers != null && activeUsers.Contains(g.Key.ToString())
                                       })
                .ToListAsync();

            return conversations;
        }

        public async Task UpdateMessageStatusAsync(Guid messageId, bool status)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null)
            {
                message.ReadStatus = status;
                message.UpdatedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteMessageAsync(Guid messageId, Guid userId)
        {
            try
            {
                var message = await _context.Messages.FindAsync(messageId);
                if (message != null && (message.SenderId == userId || message.ReceiverId == userId))
                {
                    _context.Messages.Remove(message);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể xóa tin nhắn với ID {messageId}.", ex);
            }
        }

        public async Task ArchiveConversationAsync(Guid senderId, Guid receiverId, bool isArchived)
        {
            var messages = await _context.Messages
                .Where(m =>
                    (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                    (m.SenderId == receiverId && m.ReceiverId == senderId))
                .ToListAsync();

            foreach (var message in messages)
            {
                message.IsArchived = isArchived;
                message.UpdatedOn = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Message>> SearchMessagesAsync(Guid senderId, Guid receiverId, string keyword)
        {
            return await _context.Messages
                .Where(m =>
                    ((m.SenderId == senderId && m.ReceiverId == receiverId) ||
                     (m.SenderId == receiverId && m.ReceiverId == senderId)) &&
                    m.Content.Contains(keyword, StringComparison.OrdinalIgnoreCase) && !m.IsArchived)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task BlockUserAsync(Guid userId, Guid blockedUserId)
        {
            var block = new BlockedUser
            {
                UserId = userId,
                BlockedUserId = blockedUserId,
                BlockedOn = DateTime.UtcNow
            };
            _context.BlockedUsers.Add(block);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUserBlockedAsync(Guid userId, Guid blockedUserId)
        {
            return await _context.BlockedUsers
                .AnyAsync(b => b.UserId == userId && b.BlockedUserId == blockedUserId);
        }

    }
}
