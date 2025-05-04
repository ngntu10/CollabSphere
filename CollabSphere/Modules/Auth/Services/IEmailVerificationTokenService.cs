using System;
using System.Threading.Tasks;

using CollabSphere.Entities.Domain;

using UserEntity = CollabSphere.Entities.Domain.User;

namespace CollabSphere.Modules.Auth.Services
{
    public interface IEmailVerificationTokenService
    {
        Task<bool> IsEmailVerificationTokenExpiredAsync(EmailVerificationToken token);
        Task<EmailVerificationToken> CreateEmailTokenAsync(UserEntity user);
        Task<UserEntity> GetUserByTokenAsync(string token);
        Task DeleteByUserIdAsync(Guid userId);
    }
}
