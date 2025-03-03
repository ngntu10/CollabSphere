using CollabSphere.Common;

namespace CollabSphere.Modules.User.Models;

public class CreateUserModel
{
    public string Username { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string ConfirmPassword { get; set; }
}

public class CreateUserResponseModel : BaseResponseModel
{
    public string Username { get; set; }
    public string Email { get; set; }
}
