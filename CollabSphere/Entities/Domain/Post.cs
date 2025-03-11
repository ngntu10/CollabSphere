using CollabSphere.Common;

namespace CollabSphere.Entities.Domain;

public class Post : BaseEntity, IAuditedEntity
{
    public string? Title { get; set; }


    public string? Content { get; set; }

    public string ThumbnailUrl { get; set; }

    public Guid UserId { get; set; }

    public Guid? SubredditId { get; set; }

    public int UpvoteCount { get; set; }

    public int DownvoteCount { get; set; }

    public int ShareCount { get; set; }

    // Navigation properties
    public virtual User User { get; set; }

    public virtual Subreddit Subreddit { get; set; }

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<Vote> Votes { get; } = new List<Vote>();

    public virtual ICollection<Share> Shares { get; } = new List<Share>();

    public virtual ICollection<Report> Reports { get; } = new List<Report>();

    // IAuditedEntity implementation
    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }
}
