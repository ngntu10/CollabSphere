using System;

namespace CollabSphere.Modules.Comment.Models;

public class CommentResponse
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public int Score { get; set; }
    public DateTime CreatedOn { get; set; }
    public List<CommentResponse> Replies { get; set; } = new();
}
