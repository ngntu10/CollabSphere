using System;

namespace CollabSphere.Modules.Posts.Models;

public class VoteDto
{
    public virtual Guid Id { get; set; }
    public virtual Guid PostId { get; set; }
    public virtual Guid UserId { get; set; }
    public virtual string VoteType { get; set; }
    public virtual CommentUserDto User { get; set; }
    public virtual DateTime CreatedOn { get; set; }
    public virtual Guid CreatedBy { get; set; }
    public virtual DateTime? UpdatedOn { get; set; }
    public virtual Guid UpdatedBy { get; set; }
}
