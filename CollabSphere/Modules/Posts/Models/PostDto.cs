using System;

using CollabSphere.Common;
using CollabSphere.Entities.Domain;

namespace CollabSphere.Modules.Posts.Models;

public class PostDto : BaseEntity
{
    public virtual string Title { get; set; }
    public virtual string Content { get; set; }
    public virtual string Category { get; set; }

    public virtual Guid CreatedBy { get; set; }
    public virtual DateTime CreatedOn { get; set; }
    public virtual int UpvoteCount { get; set; }

    public virtual int DownvoteCount { get; set; }

    public virtual int ShareCount { get; set; }
    public virtual ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();

    public virtual ICollection<VoteDto> Votes { get; set; } = new List<VoteDto>();

    public virtual ICollection<Share> Shares { get; set; } = new List<Share>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<PostImages> PostImages { get; set; } = new List<PostImages>();

    // Thêm thông tin người dùng đăng bài
    public virtual string Username { get; set; }
    public virtual string UserAvatar { get; set; }
}
