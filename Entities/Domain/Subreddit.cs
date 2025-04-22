using CollabSphere.Common;

namespace CollabSphere.Entities.Domain
{
    public class Subreddit : BaseEntity, IAuditedEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ThumbnailUrl { get; set; }

        public string Rules { get; set; }

        // Navigation properties
        public virtual User Creator { get; set; }

        public virtual ICollection<Subscription> Subscriptions { get; } = new List<Subscription>();

        // IAuditedEntity implementation
        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
