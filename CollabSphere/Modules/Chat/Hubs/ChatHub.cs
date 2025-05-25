using System.Collections.Concurrent;
using System.Threading.Tasks;

using CollabSphere.Entities.Domain;
using CollabSphere.Modules.Chat.Services.Interfaces;

using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace CollabSphere.Modules.Chat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private static readonly ConcurrentDictionary<string, string> _activeUsers = new ConcurrentDictionary<string, string>();
        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public override async Task OnConnectedAsync()
        {
            string userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                _activeUsers.TryAdd(userId, userId);
            }

            await Clients.All.SendAsync("UpdateActiveUsers", _activeUsers.Keys.ToList());

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                _activeUsers.TryRemove(userId, out _);
            }

            // Gửi danh sách người dùng active cho tất cả client
            await Clients.All.SendAsync("UpdateActiveUsers", _activeUsers.Keys.ToList());

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoom(string roomId, string senderId, string receiverId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            var messages = await _messageService.GetMessagesBetweenUsersAsync(
                Guid.Parse(senderId),
                Guid.Parse(receiverId)
            );

            await Clients.Caller.SendAsync("LoadMessages", messages.Select(m => new
            {
                SenderId = m.SenderId.ToString(),
                Content = m.Content,
                SentAt = m.SentAt
            }).ToList());
        }

        public async Task SendMessage(string roomId, string senderId, string receiverId, string content)
        {
            var message = new Message
            {
                SenderId = Guid.Parse(senderId),
                ReceiverId = Guid.Parse(receiverId),
                Content = content,
                SentAt = DateTime.UtcNow,
                MessageType = "text",
                Subject = string.Empty,
                ReadStatus = false,
                CreatedBy = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = Guid.NewGuid(),
                UpdatedOn = DateTime.UtcNow
            };

            await _messageService.SaveMessageAsync(message);

            await Clients.Group(roomId).SendAsync("ReceiveMessage", senderId, content);
        }

        public async Task GetConversations(string userId)
        {
            var conversations = await _messageService.GetConversationsAsync(Guid.Parse(userId), _activeUsers.Keys.ToList());
            await Clients.Caller.SendAsync("ReceiveConversations", conversations);
        }
    }

}
