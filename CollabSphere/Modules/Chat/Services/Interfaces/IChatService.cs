namespace CollabSphere.Modules.Chat.Services.Interfaces;

public interface IChatService
{
    // Task<MessageResponse> SendMessageAsync(SendMessageRequest request, string senderId);
    // Task<PaginatedResponse<MessageResponse>> GetChatHistoryAsync(string userId1, string userId2, PaginationParams pagination);
    // Task<List<ChatRoomResponse>> GetUserChatsAsync(string userId);
    Task MarkMessagesAsReadAsync(string chatId, string userId);
}
