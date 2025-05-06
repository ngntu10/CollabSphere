using System;
using System.Security.Claims;
using System.Threading.Tasks;

using CollabSphere.Modules.Follow.Models;
using CollabSphere.Modules.Follow.Service;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollabSphere.Modules.Follow.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/follows")]
    public class FollowController : ControllerBase
    {
        private readonly IFollowService _followService;

        public FollowController(IFollowService followService)
        {
            _followService = followService;
        }

        [HttpPost]
        public async Task<IActionResult> FollowUser(FollowRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var follow = await _followService.FollowUserAsync(Guid.Parse(userId), request.FollowingId);
            return Ok(follow);
        }

        [HttpDelete("{followingId}")]
        public async Task<IActionResult> UnfollowUser(Guid followingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _followService.UnfollowUserAsync(Guid.Parse(userId), followingId);
            return result ? Ok(new { message = "Đã hủy theo dõi thành công" }) : NotFound();
        }

        [HttpGet("followers")]
        public async Task<IActionResult> GetFollowers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var followers = await _followService.GetFollowersAsync(Guid.Parse(userId), page, pageSize);
            return Ok(followers);
        }

        [HttpGet("following")]
        public async Task<IActionResult> GetFollowing([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var following = await _followService.GetFollowingAsync(Guid.Parse(userId), page, pageSize);
            return Ok(following);
        }

        [HttpGet("users/{userId}/followers")]
        public async Task<IActionResult> GetUserFollowers(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var followers = await _followService.GetFollowersAsync(userId, page, pageSize);
            return Ok(followers);
        }

        [HttpGet("users/{userId}/following")]
        public async Task<IActionResult> GetUserFollowing(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var following = await _followService.GetFollowingAsync(userId, page, pageSize);
            return Ok(following);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] SearchUserRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var users = await _followService.SearchUsersToFollowAsync(Guid.Parse(userId), request);
            return Ok(users);
        }

        [HttpDelete("followers/{followerId}")]
        public async Task<IActionResult> RemoveFollower(Guid followerId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _followService.RemoveFollowerAsync(Guid.Parse(userId), followerId);
            return result ? Ok(new { message = "Đã xóa người theo dõi thành công" }) : NotFound();
        }

        [HttpPost("block")]
        public async Task<IActionResult> BlockUser(BlockUserRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _followService.BlockUserAsync(Guid.Parse(userId), request.UserToBlockId);
            return Ok(new { message = "Đã chặn người dùng thành công" });
        }

        [HttpDelete("block/{userToUnblockId}")]
        public async Task<IActionResult> UnblockUser(Guid userToUnblockId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _followService.UnblockUserAsync(Guid.Parse(userId), userToUnblockId);
            return result ? Ok(new { message = "Đã bỏ chặn người dùng thành công" }) : NotFound();
        }

        [HttpGet("blocked")]
        public async Task<IActionResult> GetBlockedUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var blockedUsers = await _followService.GetBlockedUsersAsync(Guid.Parse(userId), page, pageSize);
            return Ok(blockedUsers);
        }
    }
}
