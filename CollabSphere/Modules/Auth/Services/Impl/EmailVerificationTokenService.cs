using System;
using System.Threading.Tasks;

using CollabSphere.Database;
using CollabSphere.Entities.Domain;
using CollabSphere.Exceptions;
using CollabSphere.Helpers;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using UserEntity = CollabSphere.Entities.Domain.User;

namespace CollabSphere.Modules.Auth.Services.Impl
{
    public class EmailVerificationTokenService : IEmailVerificationTokenService
    {
        private readonly DatabaseContext _context;

        public EmailVerificationTokenService(DatabaseContext context)
        {
            _context = context;
        }

        public Task<bool> IsEmailVerificationTokenExpiredAsync(EmailVerificationToken token)
        {
            return Task.FromResult(token.ExpirationDate < DateTime.UtcNow);
        }

        public async Task<EmailVerificationToken> CreateEmailTokenAsync(UserEntity user)
        {
            string newToken = Guid.NewGuid().ToString();
            DateTime expirationDate = DateTime.UtcNow.AddSeconds(300);

            // Tìm token cũ nếu có
            var oldToken = await _context.EmailVerificationTokens
                .FirstOrDefaultAsync(t => t.UserId == user.Id);

            EmailVerificationToken emailVerificationToken;

            if (oldToken != null)
            {
                // Cập nhật token cũ
                oldToken.Token = newToken;
                oldToken.ExpirationDate = expirationDate;
                emailVerificationToken = oldToken;
            }
            else
            {
                // Tạo token mới
                emailVerificationToken = new EmailVerificationToken
                {
                    User = user,
                    UserId = user.Id,
                    Token = newToken,
                    ExpirationDate = expirationDate
                };

                _context.EmailVerificationTokens.Add(emailVerificationToken);
            }

            await _context.SaveChangesAsync();
            return emailVerificationToken;
        }

        public async Task<UserEntity> GetUserByTokenAsync(string token)
        {
            var emailVerificationToken = await _context.EmailVerificationTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token);

            if (emailVerificationToken == null)
                throw new NotFoundException("Token xác thực email không tồn tại");

            if (await IsEmailVerificationTokenExpiredAsync(emailVerificationToken))
                throw new BadRequestException("Token xác thực email đã hết hạn");

            return emailVerificationToken.User;
        }

        public async Task DeleteByUserIdAsync(Guid userId)
        {
            var tokens = await _context.EmailVerificationTokens
                .Where(t => t.UserId == userId)
                .ToListAsync();

            _context.EmailVerificationTokens.RemoveRange(tokens);
            await _context.SaveChangesAsync();
        }
    }
}
