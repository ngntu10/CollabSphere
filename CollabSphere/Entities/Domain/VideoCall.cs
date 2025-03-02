using CollabSphere.Common;

namespace CollabSphere.Entities.Domain
{
    public class VideoCall : BaseEntity, IAuditedEntity
    {
        public Guid CallerId { get; set; }

        public Guid ReceiverId { get; set; }

        public DateTime CallStartTime { get; set; }

        public DateTime? CallEndTime { get; set; }

        public string CallStatus { get; set; } // active, ended, missed

        public string CallSessionUrl { get; set; }

        // Navigation properties
        public virtual User Caller { get; set; }

        public virtual User Receiver { get; set; }

        // IAuditedEntity implementation
        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
