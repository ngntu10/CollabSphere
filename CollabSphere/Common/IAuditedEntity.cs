namespace CollabSphere.Common
{
    public interface IAuditedEntity
    {
        public DateTime CreateAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
