using CollabSphere.Common;

namespace CollabSphere.Entities.Domain
{
    public class Vote : BaseEntity, IAuditedEntity
    {
        public Guid UserId { get; set; }

        public Guid? PostId { get; set; }

        public Guid? CommentId { get; set; }

        public string VoteType { get; set; } // upvote/downvote

        // Navigation properties
        public virtual User User { get; set; }

        public virtual Post Post { get; set; }

        public virtual Comment Comment { get; set; }

        // IAuditedEntity implementation
        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
