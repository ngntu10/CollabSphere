using CollabSphere.Common;

namespace CollabSphere.Entities.Domain
{
    public class Notification : BaseEntity, IAuditedEntity
    {
        public Guid UserId { get; set; }

        public string Content { get; set; }

        public string Link { get; set; }

        public bool IsRead { get; set; }

        public string NotificationType { get; set; }

        // Navigation properties
        public virtual User User { get; set; }

        // IAuditedEntity implementation
        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
