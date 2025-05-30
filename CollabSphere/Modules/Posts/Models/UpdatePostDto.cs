using System;

namespace CollabSphere.Modules.Posts.Models;

public class UpdatePostDto
{
    public string Title { get; set; }
    public string Content { get; set; }

    public List<PostImageDto> PostImages { get; set; } = new();
}
