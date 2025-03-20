using System;
using System.Threading.Tasks;

using CollabSphere.Entities.Domain;

namespace CollabSphere.Modules.Auth.Services
{
    public interface IEmailVerificationTokenService
    {
        Task<bool> IsEmailVerificationTokenExpiredAsync(EmailVerificationToken token);
        Task<EmailVerificationToken> CreateEmailTokenAsync(User user);
        Task<User> GetUserByTokenAsync(string token);
        Task DeleteByUserIdAsync(Guid userId);
    }
}
