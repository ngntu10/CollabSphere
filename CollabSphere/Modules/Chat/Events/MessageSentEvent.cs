namespace CollabSphere.Modules.Chat.Events;

public class MessageSentEvent
{
    public string MessageId { get; set; }
    public string SenderId { get; set; }
    public string ReceiverId { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
}
