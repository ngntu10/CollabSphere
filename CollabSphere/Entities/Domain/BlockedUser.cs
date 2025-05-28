namespace CollabSphere.Entities.Domain;

public class BlockedUser
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BlockedUserId { get; set; }
    public DateTime BlockedOn { get; set; }
}
