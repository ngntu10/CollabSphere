namespace CollabSphere.Modules.Chat.Models.Requests;

public class DeleteMessageRequest
{
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
}
