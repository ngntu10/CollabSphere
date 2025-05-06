using System;

namespace CollabSphere.Modules.Comment.Models;

public class CreateCommentRequest
{
    public Guid PostId { get; set; }
    public string Content { get; set; }
    public Guid? ParentCommentId { get; set; }
}
