using System;

using CollabSphere.Common;
using CollabSphere.Entities.Domain;

namespace CollabSphere.Modules.Posts.Models;

public class PostDto : BaseEntity
{

    public string Title { get; set; }
    public string Content { get; set; }
    public string Category { get; set; }


    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public int UpvoteCount { get; set; }

    public int DownvoteCount { get; set; }

    public int ShareCount { get; set; }
    public virtual ICollection<CollabSphere.Entities.Domain.Comment> Comments { get; set; } = new List<CollabSphere.Entities.Domain.Comment>();

    public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();

    public virtual ICollection<Share> Shares { get; set; } = new List<Share>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<PostImages> PostImages { get; set; } = new List<PostImages>();

    // Thêm thông tin người dùng đăng bài
    public string Username { get; set; }
    public string UserAvatar { get; set; }

}
