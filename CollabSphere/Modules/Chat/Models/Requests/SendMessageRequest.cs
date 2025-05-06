namespace CollabSphere.Modules.Chat.Models.Requests;

public class SendMessageRequest
{
    public string ReceiverId { get; set; }
    public string Content { get; set; }
    public string MessageType { get; set; } = ChatConstants.MESSAGE_TYPE_TEXT;
}
