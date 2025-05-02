using System;

namespace CollabSphere.Modules.User.Models;

public class UserResponseModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Gender { get; set; }
    public string AvatarId { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}

public class CreateUserModel
{
    public string Name { get; set; }
    public string Gender { get; set; }
    public string AvatarId { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}

public class UpdateUserModel
{
    public string Name { get; set; }
    public string Gender { get; set; }
    public string AvatarId { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}