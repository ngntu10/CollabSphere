using CollabSphere.Common;

namespace CollabSphere.Entities.Domain
{
    public class Message : BaseEntity, IAuditedEntity
    {
        public Guid SenderId { get; set; }

        public Guid ReceiverId { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public DateTime SentAt { get; set; }

        public bool ReadStatus { get; set; }

        public string MessageType { get; set; }

        public virtual User Sender { get; set; }

        public virtual User Receiver { get; set; }

        // IAuditedEntity implementation
        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
