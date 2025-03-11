using CollabSphere.Common;

using Microsoft.AspNetCore.Identity;

namespace CollabSphere.Entities.Domain;

public class User : IdentityUser<Guid>, IAuditedEntity
{
    public string AvatarUrl { get; set; }

    public int Reputation { get; set; }

    public DateTime? LastLoginDate { get; set; }

    // Navigation properties
    public virtual ICollection<Post> Posts { get; } = new List<Post>();

    public virtual ICollection<Comment> Comments { get; } = new List<Comment>();

    public virtual ICollection<Vote> Votes { get; } = new List<Vote>();

    public virtual ICollection<Share> Shares { get; } = new List<Share>();

    public virtual ICollection<Subscription> Subscriptions { get; } = new List<Subscription>();

    public virtual ICollection<Follow> Followers { get; } = new List<Follow>();

    public virtual ICollection<Follow> Following { get; } = new List<Follow>();

    public virtual ICollection<Message> SentMessages { get; } = new List<Message>();

    public virtual ICollection<Message> ReceivedMessages { get; } = new List<Message>();

    public virtual ICollection<Notification> Notifications { get; } = new List<Notification>();

    public virtual ICollection<Report> Reports { get; } = new List<Report>();

    public virtual ICollection<VideoCall> VideoCalls { get; } = new List<VideoCall>();

    // IAuditedEntity implementation
    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }
}

public class Role : IdentityRole<Guid> { }
