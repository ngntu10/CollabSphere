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
        Task<FollowDto> FollowUserAsync(Guid followerId, Guid followingId);
        Task<bool> UnfollowUserAsync(Guid followerId, Guid followingId);

        // Lấy danh sách người theo dõi và đang theo dõi
        Task<PaginationResponse<UserDto>> GetFollowersAsync(Guid userId, int page = 1, int pageSize = 10);
        Task<PaginationResponse<UserDto>> GetFollowingAsync(Guid userId, int page = 1, int pageSize = 10);

        // Tìm kiếm người dùng để follow
        Task<PaginationResponse<UserDto>> SearchUsersToFollowAsync(Guid currentUserId, SearchUserRequest request);

        // Quản lý người theo dõi
        Task<bool> RemoveFollowerAsync(Guid userId, Guid followerToRemoveId);

        // Chặn người dùng
        Task<bool> BlockUserAsync(Guid userId, Guid userToBlockId);
        Task<bool> UnblockUserAsync(Guid userId, Guid userToUnblockId);
        Task<PaginationResponse<UserDto>> GetBlockedUsersAsync(Guid userId, int page = 1, int pageSize = 10);
    }
}
