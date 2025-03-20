using CollabSphere.Common;

namespace CollabSphere.Entities.Domain
{
    public class Follow : BaseEntity, IAuditedEntity
    {
        public Guid FollowerId { get; set; }

        public Guid FollowingId { get; set; }

        public DateTime FollowedAt { get; set; }

        // Navigation properties
        public virtual User Follower { get; set; }

        public virtual User Following { get; set; }

        // IAuditedEntity implementation
        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
