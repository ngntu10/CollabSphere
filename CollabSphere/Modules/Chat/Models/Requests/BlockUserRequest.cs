namespace CollabSphere.Modules.Chat.Models.Requests;

public class BlockUserRequest
{
    public Guid UserId { get; set; }
    public Guid BlockedUserId { get; set; }
}
