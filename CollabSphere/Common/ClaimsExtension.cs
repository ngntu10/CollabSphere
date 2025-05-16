using System;
using System.Security.Claims;

namespace CollabSphere.Common
{
    public static class ClaimsExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            foreach (var claim in user.Claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
            }
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Không thể xác định UserId từ token.");
            }

            return userId;
        }
    }
}
