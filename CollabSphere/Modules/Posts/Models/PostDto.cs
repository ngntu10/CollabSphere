using System;

using CollabSphere.Common;
using CollabSphere.Entities.Domain;

namespace CollabSphere.Modules.Posts.Models;

public class PostDto : BaseEntity
{

    public string Title { get; set; }
    public string Content { get; set; }
    public string? ThumbnailUrl { get; set; }
    public Guid? SubredditId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public int UpvoteCount { get; set; }

    public int DownvoteCount { get; set; }

    public int ShareCount { get; set; }
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();

    public virtual ICollection<Share> Shares { get; set; } = new List<Share>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();


}
