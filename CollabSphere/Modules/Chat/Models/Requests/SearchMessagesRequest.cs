namespace CollabSphere.Modules.Chat.Models.Requests;

public class SearchMessagesRequest
{
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string Keyword { get; set; } = string.Empty;
}
