using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CollabSphere.Common;
using CollabSphere.Modules.Follow.Models;

namespace CollabSphere.Modules.Follow.Service
{
    public interface IFollowService
    {
        // Follow/Unfollow người dùng
        Task<FollowDto> FollowUserAsync(string followerName, string followingName);
        Task<bool> UnfollowUserAsync(string followerName, string followingName);

        // Lấy danh sách người theo dõi và đang theo dõi
        Task<PaginationResponse<UserDto>> GetFollowersAsync(string userName, int page = 1, int pageSize = 10);
        Task<PaginationResponse<UserDto>> GetFollowingAsync(string userName, int page = 1, int pageSize = 10);

        // Tìm kiếm người dùng để follow
        Task<PaginationResponse<UserDto>> SearchUsersToFollowAsync(string currentUserName, SearchUserRequest request);

        // Quản lý người theo dõi
        Task<bool> RemoveFollowerAsync(string userName, string followerToRemoveName);

        // Chặn người dùng
        Task<bool> BlockUserAsync(string userName, string userToBlockName);
        Task<bool> UnblockUserAsync(string userName, string userToUnblockName);
        Task<PaginationResponse<UserDto>> GetBlockedUsersAsync(string userName, int page = 1, int pageSize = 10);
    }
}
