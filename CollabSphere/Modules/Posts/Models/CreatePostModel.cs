using System;

namespace CollabSphere.Modules.Post.Models;

public class CreatePostModel

{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }
    public string Content { get; set; }
    public string ThumbnailUrl { get; set; }
    public Guid UserId { get; set; }
    public Guid SubredditId { get; set; }
    public int UpvoteCount { get; set; } = 0;
    public int DownvoteCount { get; set; } = 0;
    public int ShareCount { get; set; } = 0;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}
