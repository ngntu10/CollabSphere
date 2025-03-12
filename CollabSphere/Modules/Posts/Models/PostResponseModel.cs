using System;

namespace CollabSphere.Modules.Posts.Models;

public class PostResponseModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string ThumbnailUrl { get; set; }
    public Guid? SubredditId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public Guid UpdatedBy { get; set; }
    public string UpdatedByUsername { get; set; }
}
