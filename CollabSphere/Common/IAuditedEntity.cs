namespace CollabSphere.Common
{
    public interface IAuditedEntity
    {
        public Guid CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
