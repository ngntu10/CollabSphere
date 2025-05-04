using System;

namespace CollabSphere.Modules.User.Models;

public class UserResponseModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Gender { get; set; }
    public string AvatarId { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}

// public class CreateUserModel
// {
//     public string UserName { get; set; }
//     public string Gender { get; set; }
//     public string AvatarId { get; set; }
//     public string Email { get; set; }
//     public string PhoneNumber { get; set; }
// }

public class UpdateUserModel
{
    public string UserName { get; set; }
    public string Gender { get; set; }
    public string AvatarId { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}
