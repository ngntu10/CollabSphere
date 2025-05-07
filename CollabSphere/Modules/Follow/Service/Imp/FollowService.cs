using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CollabSphere.Common;
using CollabSphere.Database;
using CollabSphere.Entities.Domain;
using CollabSphere.Exceptions;
using CollabSphere.Modules.Follow.Models;

using Microsoft.EntityFrameworkCore;

namespace CollabSphere.Modules.Follow.Service.Imp
{
    public class FollowService : IFollowService
    {
        private readonly DatabaseContext _context;

        public FollowService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<FollowDto> FollowUserAsync(Guid followerId, Guid followingId)
        {
            // Không thể follow chính mình
            if (followerId == followingId)
            {
                throw new BadRequestException("Không thể theo dõi chính bạn");
            }

            // Kiểm tra người dùng có tồn tại không
            var follower = await _context.Users.FindAsync(followerId);
            var following = await _context.Users.FindAsync(followingId);

            if (follower == null || following == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng");
            }

            // Kiểm tra xem đã follow chưa
            var existingFollow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

            if (existingFollow != null)
            {
                throw new BadRequestException("Bạn đã theo dõi người dùng này rồi");
            }

            // Kiểm tra xem người dùng có bị chặn không
            var isBlocked = await _context.UserBlocks
                .AnyAsync(b => (b.BlockerId == followingId && b.BlockedId == followerId) ||
                              (b.BlockerId == followerId && b.BlockedId == followingId));

            if (isBlocked)
            {
                throw new ForbiddenException("Không thể theo dõi người dùng này");
            }

            // Tạo mối quan hệ follow mới
            var newFollow = new Entities.Domain.Follow
            {
                Id = Guid.NewGuid(),
                FollowerId = followerId,
                FollowingId = followingId,
                FollowedAt = DateTime.UtcNow,
                CreatedBy = followerId,
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = followerId,
                UpdatedOn = DateTime.UtcNow
            };

            _context.Follows.Add(newFollow);
            await _context.SaveChangesAsync();

            // Trả về thông tin follow
            return new FollowDto
            {
                Id = newFollow.Id,
                FollowerId = follower.Id,
                FollowerName = follower.UserName,
                FollowerAvatarUrl = follower.AvatarId,
                FollowingId = following.Id,
                FollowingName = following.UserName,
                FollowingAvatarUrl = following.AvatarId,
                FollowedAt = newFollow.FollowedAt
            };
        }

        public async Task<bool> UnfollowUserAsync(Guid followerId, Guid followingId)
        {
            var follow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

            if (follow == null)
            {
                return false;
            }

            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PaginationResponse<UserDto>> GetFollowersAsync(Guid userId, int page = 1, int pageSize = 10)
        {
            // Kiểm tra user tồn tại
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng");
            }

            // Lấy tổng số lượng followers
            var totalFollowers = await _context.Follows
                .Where(f => f.FollowingId == userId)
                .CountAsync();

            // Phân trang và lấy danh sách người theo dõi
            var followers = await _context.Follows
                .Where(f => f.FollowingId == userId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new UserDto
                {
                    Id = f.Follower.Id,
                    UserName = f.Follower.UserName,
                    Email = f.Follower.Email,
                    AvatarUrl = f.Follower.AvatarId,
                    Reputation = f.Follower.Reputation,
                    IsFollowing = _context.Follows.Any(ff => ff.FollowerId == userId && ff.FollowingId == f.FollowerId)
                })
                .ToListAsync();

            // Kiểm tra các người dùng đã bị chặn chưa
            var blockedUserIds = await _context.UserBlocks
                .Where(b => b.BlockerId == userId)
                .Select(b => b.BlockedId)
                .ToListAsync();

            foreach (var follower in followers)
            {
                follower.IsBlocked = blockedUserIds.Contains(follower.Id);
            }

            var totalPages = (int) Math.Ceiling(totalFollowers / (double) pageSize);

            return new PaginationResponse<UserDto>(
                page,
                totalPages,
                pageSize,
                totalFollowers,
                followers
            );
        }

        public async Task<PaginationResponse<UserDto>> GetFollowingAsync(Guid userId, int page = 1, int pageSize = 10)
        {
            // Kiểm tra user tồn tại
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng");
            }

            // Lấy tổng số lượng following
            var totalFollowing = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .CountAsync();

            // Phân trang và lấy danh sách người đang theo dõi
            var following = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new UserDto
                {
                    Id = f.Following.Id,
                    UserName = f.Following.UserName,
                    Email = f.Following.Email,
                    AvatarUrl = f.Following.AvatarId,
                    Reputation = f.Following.Reputation,
                    IsFollowing = true // Luôn là true vì đây là danh sách người đang theo dõi
                })
                .ToListAsync();

            // Kiểm tra các người dùng đã bị chặn chưa
            var blockedUserIds = await _context.UserBlocks
                .Where(b => b.BlockerId == userId)
                .Select(b => b.BlockedId)
                .ToListAsync();

            foreach (var follow in following)
            {
                follow.IsBlocked = blockedUserIds.Contains(follow.Id);
            }

            var totalPages = (int) Math.Ceiling(totalFollowing / (double) pageSize);

            return new PaginationResponse<UserDto>(
                page,
                totalPages,
                pageSize,
                totalFollowing,
                following
            );
        }

        public async Task<PaginationResponse<UserDto>> SearchUsersToFollowAsync(Guid currentUserId, SearchUserRequest request)
        {
            // Lấy danh sách user theo search term, loại trừ user hiện tại
            var query = _context.Users
                .Where(u => u.Id != currentUserId)
                .Where(u => string.IsNullOrEmpty(request.SearchTerm) ||
                          u.UserName.Contains(request.SearchTerm) ||
                          u.Email.Contains(request.SearchTerm));

            // Lấy tổng số kết quả
            var totalUsers = await query.CountAsync();

            // Phân trang kết quả
            var users = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    AvatarUrl = u.AvatarId,
                    Reputation = u.Reputation,
                    IsFollowing = _context.Follows.Any(f => f.FollowerId == currentUserId && f.FollowingId == u.Id),
                    IsBlocked = _context.UserBlocks.Any(b => b.BlockerId == currentUserId && b.BlockedId == u.Id)
                })
                .ToListAsync();

            var totalPages = (int) Math.Ceiling(totalUsers / (double) request.PageSize);

            return new PaginationResponse<UserDto>(
                request.Page,
                totalPages,
                request.PageSize,
                totalUsers,
                users
            );
        }

        public async Task<bool> RemoveFollowerAsync(Guid userId, Guid followerToRemoveId)
        {
            // Xóa mối quan hệ follow từ followerToRemoveId đến userId
            var follow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followerToRemoveId && f.FollowingId == userId);

            if (follow == null)
            {
                return false;
            }

            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BlockUserAsync(Guid userId, Guid userToBlockId)
        {
            // Kiểm tra xem đã block chưa
            var existingBlock = await _context.UserBlocks
                .FirstOrDefaultAsync(b => b.BlockerId == userId && b.BlockedId == userToBlockId);

            if (existingBlock != null)
            {
                return true; // Đã block rồi
            }

            // Tạo block mới
            var newBlock = new UserBlock
            {
                Id = Guid.NewGuid(),
                BlockerId = userId,
                BlockedId = userToBlockId,
                BlockedAt = DateTime.UtcNow,
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = userId,
                UpdatedOn = DateTime.UtcNow
            };

            _context.UserBlocks.Add(newBlock);

            // Xóa các mối quan hệ follow giữa hai người dùng
            var follows = await _context.Follows
                .Where(f => (f.FollowerId == userId && f.FollowingId == userToBlockId) ||
                          (f.FollowerId == userToBlockId && f.FollowingId == userId))
                .ToListAsync();

            if (follows.Any())
            {
                _context.Follows.RemoveRange(follows);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnblockUserAsync(Guid userId, Guid userToUnblockId)
        {
            var block = await _context.UserBlocks
                .FirstOrDefaultAsync(b => b.BlockerId == userId && b.BlockedId == userToUnblockId);

            if (block == null)
            {
                return false;
            }

            _context.UserBlocks.Remove(block);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PaginationResponse<UserDto>> GetBlockedUsersAsync(Guid userId, int page = 1, int pageSize = 10)
        {
            // Lấy tổng số người bị chặn
            var totalBlocked = await _context.UserBlocks
                .Where(b => b.BlockerId == userId)
                .CountAsync();

            // Phân trang và lấy danh sách người bị chặn
            var blockedUsers = await _context.UserBlocks
                .Where(b => b.BlockerId == userId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new UserDto
                {
                    Id = b.Blocked.Id,
                    UserName = b.Blocked.UserName,
                    Email = b.Blocked.Email,
                    AvatarUrl = b.Blocked.AvatarId,
                    Reputation = b.Blocked.Reputation,
                    IsFollowing = false, // Không thể follow người bị chặn
                    IsBlocked = true
                })
                .ToListAsync();

            var totalPages = (int) Math.Ceiling(totalBlocked / (double) pageSize);

            return new PaginationResponse<UserDto>(
                page,
                totalPages,
                pageSize,
                totalBlocked,
                blockedUsers
            );
        }
    }
}
