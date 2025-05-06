using CollabSphere.Common;

namespace CollabSphere.Entities.Domain
{
    public class UserBlock : BaseEntity, IAuditedEntity
    {
        public Guid BlockerId { get; set; }
        public Guid BlockedId { get; set; }
        public DateTime BlockedAt { get; set; }

        // Navigation properties
        public virtual User Blocker { get; set; }
        public virtual User Blocked { get; set; }

        // IAuditedEntity implementation
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
