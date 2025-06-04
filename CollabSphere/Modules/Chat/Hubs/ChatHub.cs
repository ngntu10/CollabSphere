using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using CollabSphere.Database;
using CollabSphere.Entities.Domain;
using CollabSphere.Modules.Chat.Services.Interfaces;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CollabSphere.Modules.Chat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly DatabaseContext _context;
        private readonly ILogger<ChatHub> _logger;
        private static readonly ConcurrentDictionary<string, string> _activeUsers = new ConcurrentDictionary<string, string>();

        public ChatHub(IMessageService messageService, DatabaseContext context, ILogger<ChatHub> logger)
        {
            _messageService = messageService;
            _context = context;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            string userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out _))
            {
                _activeUsers.TryAdd(userId, userId);
                _logger.LogInformation($"User {userId} connected with ConnectionId {Context.ConnectionId}");
            }
            else
            {
                _logger.LogWarning($"Invalid userId on connection: {userId}");
            }

            var activeUserDetails = await _context.Users
                .Where(u => _activeUsers.Keys.Contains(u.Id.ToString()))
                .Select(u => new
                {
                    Id = u.Id.ToString(),
                    UserName = u.UserName,
                    AvatarId = u.AvatarId ?? string.Empty
                })
                .ToListAsync();

            await Clients.All.SendAsync("UpdateActiveUsers", activeUserDetails);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                _activeUsers.TryRemove(userId, out _);
                _logger.LogInformation($"User {userId} disconnected: {exception?.Message}");
            }

            var activeUserDetails = await _context.Users
                .Where(u => _activeUsers.Keys.Contains(u.Id.ToString()))
                .Select(u => new
                {
                    Id = u.Id.ToString(),
                    UserName = u.UserName,
                    AvatarId = u.AvatarId ?? string.Empty
                })
                .ToListAsync();

            await Clients.All.SendAsync("UpdateActiveUsers", activeUserDetails);

            await base.OnDisconnectedAsync(exception);
        }

        // public async Task JoinRoom(string roomId, string senderId, string receiverId)
        // {
        //     if (!Guid.TryParse(senderId, out _) || !Guid.TryParse(receiverId, out _))
        //     {
        //         _logger.LogWarning($"Invalid user IDs: senderId={senderId}, receiverId={receiverId}");
        //         throw new HubException("Invalid user ID.");
        //     }
        //
        //     await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        //     _logger.LogInformation($"User {senderId} joined room {roomId}");
        //
        //     var messages = await _messageService.GetMessagesBetweenUsersAsync(
        //         Guid.Parse(senderId),
        //         Guid.Parse(receiverId)
        //     );
        //
        //     // Lấy thông tin tất cả người gửi trong danh sách tin nhắn
        //     var userIds = messages.Select(m => m.SenderId.ToString()).Distinct().ToList();
        //     var users = await _context.Users
        //         .Where(u => userIds.Contains(u.Id.ToString()))
        //         .Select(u => new
        //         {
        //             Id = u.Id.ToString(),
        //             UserName = u.UserName,
        //             AvatarId = u.AvatarId ?? string.Empty
        //         })
        //         .ToDictionaryAsync(u => u.Id, u => u);
        //
        //     await Clients.Caller.SendAsync("LoadMessages", messages.Select(m => new
        //     {
        //         Id = m.Id.ToString(),
        //         SenderId = m.SenderId.ToString(),
        //         SenderName = users.ContainsKey(m.SenderId.ToString()) ? users[m.SenderId.ToString()].UserName : "Unknown",
        //         SenderAvatar = users.ContainsKey(m.SenderId.ToString()) ? users[m.SenderId.ToString()].AvatarId : string.Empty,
        //         Content = m.Content,
        //         SentAt = m.SentAt,
        //         ReadStatus = m.ReadStatus
        //     }).ToList());
        // }

        public async Task SendMessage(string roomId, string senderId, string receiverId, string content)
        {
            if (!Guid.TryParse(senderId, out var senderGuid) || !Guid.TryParse(receiverId, out var receiverGuid))
            {
                _logger.LogWarning($"Invalid user IDs: senderId={senderId}, receiverId={receiverId}");
                throw new HubException("Invalid user ID.");
            }

            var isBlocked = await _messageService.IsUserBlockedAsync(senderGuid, receiverGuid);
            if (isBlocked)
            {
                throw new HubException("You are blocked from sending messages to this user.");
            }

            var message = new Message
            {
                SenderId = senderGuid,
                ReceiverId = receiverGuid,
                Content = content,
                SentAt = DateTime.UtcNow,
                MessageType = "text",
                Subject = string.Empty,
                ReadStatus = false,
                CreatedBy = senderGuid,
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = senderGuid,
                UpdatedOn = DateTime.UtcNow
            };

            await _messageService.SaveMessageAsync(message);
            _logger.LogInformation($"Message sent from {senderId} to {receiverId} in room {roomId}");

            // Lấy thông tin người gửi
            var sender = await _context.Users
                .Where(u => u.Id.ToString() == senderId)
                .Select(u => new
                {
                    UserName = u.UserName,
                    AvatarId = u.AvatarId ?? string.Empty
                })
                .FirstOrDefaultAsync();

            await Clients.Group(roomId).SendAsync("ReceiveMessage", senderId, content, message.Id.ToString(), sender?.UserName ?? "Unknown", sender?.AvatarId ?? string.Empty);
        }

        public async Task UpdateMessageStatus(string messageId, bool status)
        {
            if (!Guid.TryParse(messageId, out var messageGuid))
            {
                throw new HubException("Invalid message ID.");
            }

            await _messageService.UpdateMessageStatusAsync(messageGuid, status);
            _logger.LogInformation($"Message {messageId} status updated to {status}");
        }

        public async Task DeleteMessage(string messageId, string userId)
        {
            if (!Guid.TryParse(messageId, out var messageGuid) || !Guid.TryParse(userId, out var userGuid))
            {
                throw new HubException("Invalid message or user ID.");
            }

            await _messageService.DeleteMessageAsync(messageGuid, userGuid);
            _logger.LogInformation($"Message {messageId} deleted by user {userId}");
        }

        public async Task ArchiveConversation(string senderId, string receiverId, bool isArchived)
        {
            if (!Guid.TryParse(senderId, out var senderGuid) || !Guid.TryParse(receiverId, out var receiverGuid))
            {
                throw new HubException("Invalid user ID.");
            }

            await _messageService.ArchiveConversationAsync(senderGuid, receiverGuid, isArchived);
            _logger.LogInformation($"Conversation between {senderId} and {receiverId} archived: {isArchived}");
        }

        public async Task SearchMessages(string senderId, string receiverId, string keyword)
        {
            if (!Guid.TryParse(senderId, out var senderGuid) || !Guid.TryParse(receiverId, out var receiverGuid))
            {
                throw new HubException("Invalid user ID.");
            }

            var messages = await _messageService.SearchMessagesAsync(senderGuid, receiverGuid, keyword);
            _logger.LogInformation($"Search messages between {senderId} and {receiverId} with keyword: {keyword}");

            // Lấy thông tin tất cả người gửi trong danh sách tin nhắn
            var userIds = messages.Select(m => m.SenderId.ToString()).Distinct().ToList();
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id.ToString()))
                .Select(u => new
                {
                    Id = u.Id.ToString(),
                    UserName = u.UserName,
                    AvatarId = u.AvatarId ?? string.Empty
                })
                .ToDictionaryAsync(u => u.Id, u => u);

            await Clients.Caller.SendAsync("LoadMessages", messages.Select(m => new
            {
                Id = m.Id.ToString(),
                SenderId = m.SenderId.ToString(),
                SenderName = users.ContainsKey(m.SenderId.ToString()) ? users[m.SenderId.ToString()].UserName : "Unknown",
                SenderAvatar = users.ContainsKey(m.SenderId.ToString()) ? users[m.SenderId.ToString()].AvatarId : string.Empty,
                Content = m.Content,
                SentAt = m.SentAt,
                ReadStatus = m.ReadStatus
            }).ToList());
        }

        public async Task BlockUser(string userId, string blockedUserId)
        {
            if (!Guid.TryParse(userId, out var userGuid) || !Guid.TryParse(blockedUserId, out var blockedGuid))
            {
                throw new HubException("Invalid user ID.");
            }

            await _messageService.BlockUserAsync(userGuid, blockedGuid);
            _logger.LogInformation($"User {userId} blocked {blockedUserId}");
        }

        public async Task<bool> IsUserBlocked(string userId, string blockedUserId)
        {
            if (!Guid.TryParse(userId, out var userGuid) || !Guid.TryParse(blockedUserId, out var blockedGuid))
            {
                throw new HubException("Invalid user ID.");
            }

            var isBlocked = await _messageService.IsUserBlockedAsync(userGuid, blockedGuid);
            _logger.LogInformation($"Check if {userId} blocked {blockedUserId}: {isBlocked}");
            return isBlocked;
        }

        public async Task<List<Message>> GetMessagesBetweenUsersAsync(Guid senderId, Guid receiverId, bool includeArchived = false)
        {
            var query = _context.Messages
                .Where(m =>
                    (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                    (m.SenderId == receiverId && m.ReceiverId == senderId));

            if (!includeArchived)
            {
                query = query.Where(m => !m.IsArchived);
            }

            return await query
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task JoinRoom(string roomId, string senderId, string receiverId)
        {
            if (!Guid.TryParse(senderId, out _) || !Guid.TryParse(receiverId, out _))
            {
                _logger.LogWarning($"Invalid user IDs: senderId={senderId}, receiverId={receiverId}");
                throw new HubException("Invalid user ID.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            _logger.LogInformation($"User {senderId} joined room {roomId}");

            var messages = await _messageService.GetMessagesBetweenUsersAsync(
                Guid.Parse(senderId),
                Guid.Parse(receiverId)

            );

            var userIds = messages.Select(m => m.SenderId.ToString()).Distinct().ToList();
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id.ToString()))
                .Select(u => new
                {
                    Id = u.Id.ToString(),
                    UserName = u.UserName,
                    AvatarId = u.AvatarId ?? string.Empty
                })
                .ToDictionaryAsync(u => u.Id, u => u);

            await Clients.Caller.SendAsync("LoadMessages", messages.Select(m => new
            {
                Id = m.Id.ToString(),
                SenderId = m.SenderId.ToString(),
                SenderName = users.ContainsKey(m.SenderId.ToString()) ? users[m.SenderId.ToString()].UserName : "Unknown",
                SenderAvatar = users.ContainsKey(m.SenderId.ToString()) ? users[m.SenderId.ToString()].AvatarId : string.Empty,
                Content = m.Content,
                SentAt = m.SentAt,
                ReadStatus = m.ReadStatus
            }).ToList());
        }
    }

}
