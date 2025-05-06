using System;

namespace CollabSphere.Modules.Follow.Models
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public int Reputation { get; set; }
        public bool IsFollowing { get; set; }
        public bool IsBlocked { get; set; }
    }
}
