using CollabSphere.Common;

namespace CollabSphere.Entities.Domain
{
    public class Report : BaseEntity, IAuditedEntity
    {
        public Guid ReporterId { get; set; }

        public Guid? ReportedPostId { get; set; }

        public Guid? ReportedCommentId { get; set; }

        public string Reason { get; set; }

        public string Status { get; set; } // pending, resolved, etc.

        public DateTime? ReviewAt { get; set; }

        // Navigation properties
        public virtual User Reporter { get; set; }

        public virtual Post ReportedPost { get; set; }

        public virtual Comment ReportedComment { get; set; }

        // IAuditedEntity implementation
        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
