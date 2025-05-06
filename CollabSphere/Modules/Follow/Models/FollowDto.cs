using System;

namespace CollabSphere.Modules.Follow.Models
{
    public class FollowDto
    {
        public Guid Id { get; set; }
        public Guid FollowerId { get; set; }
        public string FollowerName { get; set; }
        public string FollowerAvatarUrl { get; set; }
        public Guid FollowingId { get; set; }
        public string FollowingName { get; set; }
        public string FollowingAvatarUrl { get; set; }
        public DateTime FollowedAt { get; set; }
    }
}
