using CollabSphere.Common;
using CollabSphere.Entities.Domain;
using CollabSphere.Modules.Auth.Models;

using UserEntity = CollabSphere.Entities.Domain.User;

namespace CollabSphere.Modules.Auth.Services;

public interface IAuthService
{
    Task<BaseResponseModel> ChangePasswordAsync(Guid userId, ChangePasswordModel changePasswordModel);

    Task<LoginResponseModel> CreateAsync(CreateUserModel createUserModel);

    Task<LoginResponseModel> LoginAsync(LoginUserModel loginUserModel);

    Task VerifyEmailAsync(string token);

    Task<UserEntity> GetCurrentUserAsync();

    Task<(UserEntity user, DateTime expiresAt)> GetCurrentUserWithExpirationAsync();
}
