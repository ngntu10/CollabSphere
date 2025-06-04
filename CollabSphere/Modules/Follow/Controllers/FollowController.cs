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
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized();
            }

            var follow = await _followService.FollowUserAsync(userName, request.FollowingName);
            return Ok(follow);
        }

        [HttpDelete("{followingName}")]
        public async Task<IActionResult> UnfollowUser(string followingName)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized();
            }

            var result = await _followService.UnfollowUserAsync(userName, followingName);
            return result ? Ok(new { message = "Đã hủy theo dõi thành công" }) : NotFound();
        }

        [HttpGet("followers")]
        public async Task<IActionResult> GetFollowers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized();
            }

            var followers = await _followService.GetFollowersAsync(userName, page, pageSize);
            return Ok(followers);
        }

        [HttpGet("following")]
        public async Task<IActionResult> GetFollowing([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized();
            }

            var following = await _followService.GetFollowingAsync(userName, page, pageSize);
            return Ok(following);
        }

        [HttpGet("users/{userName}/followers")]
        public async Task<IActionResult> GetUserFollowers(string userName, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var followers = await _followService.GetFollowersAsync(userName, page, pageSize);
            return Ok(followers);
        }

        [HttpGet("users/{userName}/following")]
        public async Task<IActionResult> GetUserFollowing(string userName, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var following = await _followService.GetFollowingAsync(userName, page, pageSize);
            return Ok(following);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] SearchUserRequest request)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized();
            }

            var users = await _followService.SearchUsersToFollowAsync(userName, request);
            return Ok(users);
        }

        [HttpDelete("followers/{followerName}")]
        public async Task<IActionResult> RemoveFollower(string followerName)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized();
            }

            var result = await _followService.RemoveFollowerAsync(userName, followerName);
            return result ? Ok(new { message = "Đã xóa người theo dõi thành công" }) : NotFound();
        }

        [HttpPost("block")]
        public async Task<IActionResult> BlockUser(BlockUserRequest request)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized();
            }

            var result = await _followService.BlockUserAsync(userName, request.UserToBlockName);
            return Ok(new { message = "Đã chặn người dùng thành công" });
        }

        [HttpDelete("block/{userToUnblockName}")]
        public async Task<IActionResult> UnblockUser(string userToUnblockName)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized();
            }

            var result = await _followService.UnblockUserAsync(userName, userToUnblockName);
            return result ? Ok(new { message = "Đã bỏ chặn người dùng thành công" }) : NotFound();
        }

        [HttpGet("blocked")]
        public async Task<IActionResult> GetBlockedUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized();
            }

            var blockedUsers = await _followService.GetBlockedUsersAsync(userName, page, pageSize);
            return Ok(blockedUsers);
        }
    }
}
