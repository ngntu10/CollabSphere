using System;
using System.Collections.Generic;

namespace CollabSphere.Modules.Posts.Models;

public class CommentUserDto
{
    public virtual Guid Id { get; set; }
    public virtual string UserName { get; set; }
    public virtual string AvatarId { get; set; }
}

public class CommentDto
{
    public virtual Guid Id { get; set; }
    public virtual string Content { get; set; }
    public virtual Guid CreatedBy { get; set; }
    public virtual DateTime CreatedOn { get; set; }
    public virtual Guid PostId { get; set; }
    public virtual Guid? ParentCommentId { get; set; }
    public virtual CommentUserDto User { get; set; }
    public virtual int UpvoteCount { get; set; }
    public virtual int DownvoteCount { get; set; }
    public virtual ICollection<VoteDto> Votes { get; set; } = new List<VoteDto>();
    public virtual ICollection<CommentDto> ChildComments { get; set; } = new List<CommentDto>();
}
