using CollabSphere.Common;

namespace CollabSphere.Entities.Domain
{
    public class Subscription : BaseEntity, IAuditedEntity
    {
        public Guid UserId { get; set; }

        public Guid SubredditId { get; set; }

        public DateTime Subscribed_at { get; set; }

        // Navigation properties
        public virtual User User { get; set; }

        public virtual Subreddit Subreddit { get; set; }

        // IAuditedEntity implementation
        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
