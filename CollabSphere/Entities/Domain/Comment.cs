using CollabSphere.Common;

namespace CollabSphere.Entities.Domain
{
    public class Comment : BaseEntity, IAuditedEntity
    {
        public string Content { get; set; }

        public Guid UserId { get; set; }

        public Guid PostId { get; set; }

        public Guid? ParentCommentId { get; set; }

        public int Score { get; set; }

        // Navigation properties
        public virtual User User { get; set; }

        public virtual Post Post { get; set; }

        public virtual Comment ParentComment { get; set; }

        public virtual ICollection<Comment> ChildComments { get; } = new List<Comment>();

        public virtual ICollection<Vote> Votes { get; } = new List<Vote>();

        public virtual ICollection<Report> Reports { get; } = new List<Report>();

        // IAuditedEntity implementation
        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
