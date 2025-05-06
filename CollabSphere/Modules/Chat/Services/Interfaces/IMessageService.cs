using CollabSphere.Entities.Domain;

namespace CollabSphere.Modules.Chat.Services.Interfaces;

public interface IMessageService
{
    Task SaveMessageAsync(Message message);
    Task<List<Message>> GetMessagesBetweenUsersAsync(Guid senderId, Guid receiverId);
}
