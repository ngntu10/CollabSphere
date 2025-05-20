using System;
using System.Security.Claims;

namespace CollabSphere.Common
{
    public static class ClaimsExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            Console.WriteLine("Attempting to get UserId. Claims present in token:");
            foreach (var claim in user.Claims)
            {
                Console.WriteLine($"  Claim Type: {claim.Type}, Value: {claim.Value}");
            }
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                Console.WriteLine("Error: userIdClaim (NameIdentifier) is null.");
                throw new UnauthorizedAccessException("Không thể xác định UserId từ token.");
            }
            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                Console.WriteLine($"Error: userIdClaim.Value ('{userIdClaim.Value}') is not a valid Guid.");
                throw new UnauthorizedAccessException("Không thể xác định UserId từ token.");
            }

            return userId;
        }
    }
}
