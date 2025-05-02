using System;

namespace CollabSphere.Modules.User.Models
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string AvatarId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class CreateUserDto
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string AvatarId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class UpdateUserDto
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string AvatarId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
} 