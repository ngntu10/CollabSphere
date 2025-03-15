namespace CollabSphere.Entities.Domain
{
    public class EmailVerificationToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}
