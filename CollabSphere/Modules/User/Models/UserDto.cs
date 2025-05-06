using System;

namespace CollabSphere.Modules.User.Models
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
        public string AvatarId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    // public class CreateUserDto
    // {
    //     public string UserName { get; set; }
    //     public string Email { get; set; }
    //     public string PhoneNumber { get; set; }
    //     public string Gender { get; set; }
    //     public string AvatarId { get; set; }
    // }

    public class UpdateUserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string AvatarId { get; set; }
    }
}
