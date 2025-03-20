using System;

namespace CollabSphere.Modules.Posts.Models;

public class UpdatePostModel
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string ThumbnailUrl { get; set; }
}
