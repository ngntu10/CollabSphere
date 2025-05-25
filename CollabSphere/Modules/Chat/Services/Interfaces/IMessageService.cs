using CollabSphere.Entities.Domain;

namespace CollabSphere.Modules.Chat.Services.Interfaces;

public interface IMessageService
{
    Task SaveMessageAsync(Message message);
    Task<List<Message>> GetMessagesBetweenUsersAsync(Guid senderId, Guid receiverId);
    Task<List<ChatRoomResponse>> GetConversationsAsync(Guid userId, List<string> activeUsers);
    Task UpdateMessageStatusAsync(Guid messageId, bool status);
    Task DeleteMessageAsync(Guid messageId, Guid userId);
    Task<List<Message>> SearchMessagesAsync(Guid senderId, Guid receiverId, string keyword);
    Task ArchiveConversationAsync(Guid senderId, Guid receiverId, bool isArchived);
}
