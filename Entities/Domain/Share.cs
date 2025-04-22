using CollabSphere.Common;

namespace CollabSphere.Entities.Domain
{
    public class Share : BaseEntity, IAuditedEntity
    {
        public Guid UserId { get; set; }

        public Guid PostId { get; set; }

        public Guid? ShareTo { get; set; }

        public string ShareComment { get; set; }

        // Navigation properties
        public virtual User User { get; set; }

        public virtual Post Post { get; set; }

        // IAuditedEntity implementation
        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
