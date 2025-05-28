namespace CollabSphere.Modules.Chat.Models.Requests;

public class ArchiveConversationRequest
{
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public bool IsArchived { get; set; }
}
