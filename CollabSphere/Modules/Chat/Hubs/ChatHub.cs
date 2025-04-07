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

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
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
    }

}
