public class ChatRoomResponse
{
    public Guid PartnerId { get; set; }
    public string PartnerName { get; set; } = string.Empty;
    public string PartnerAvatar { get; set; } = string.Empty;
    public string LastMessage { get; set; } = string.Empty;
    public DateTime LastMessageTime { get; set; }
    public int UnreadCount { get; set; }
    public bool IsOnline { get; set; }
}
