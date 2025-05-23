using System;

namespace CollabSphere.Modules.Comment.Models;

public class CommentVoteUserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string AvatarId { get; set; }
}

public class CommentVoteDto
{
    public Guid Id { get; set; }
    public bool IsUpvote { get; set; }
    public CommentVoteUserDto User { get; set; }
}

public class CommentUserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string AvatarId { get; set; }
}

public class CommentResponse
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public CommentUserDto User { get; set; }
    public Guid PostId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public int UpvoteCount { get; set; }
    public int DownvoteCount { get; set; }
    public List<CommentVoteDto> Votes { get; set; } = new();
    public DateTime CreatedOn { get; set; }
    public List<CommentResponse> ChildComments { get; set; } = new();
}
