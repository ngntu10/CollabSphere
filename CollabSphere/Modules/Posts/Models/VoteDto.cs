namespace CollabSphere.Modules.Posts.Models;

public class VoteDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string VoteType { get; set; }
    public DateTime CreatedOn { get; set; }
}
