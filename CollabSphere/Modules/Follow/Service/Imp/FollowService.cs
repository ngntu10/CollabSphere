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

        public async Task<FollowDto> FollowUserAsync(string followerName, string followingName)
        {
            // Không thể follow chính mình
            if (followerName == followingName)
            {
                throw new BadRequestException("Không thể theo dõi chính bạn");
            }

            // Kiểm tra người dùng có tồn tại không
            var follower = await _context.Users.FirstOrDefaultAsync(u => u.UserName == followerName);
            var following = await _context.Users.FirstOrDefaultAsync(u => u.UserName == followingName);

            if (follower == null || following == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng");
            }

            // Kiểm tra xem đã follow chưa
            var existingFollow = await _context.Follows
                .FirstOrDefaultAsync(f => f.Follower.UserName == followerName && f.Following.UserName == followingName);

            if (existingFollow != null)
            {
                throw new BadRequestException("Bạn đã theo dõi người dùng này rồi");
            }

            // Kiểm tra xem người dùng có bị chặn không
            var isBlocked = await _context.UserBlocks
                .AnyAsync(b => (b.Blocker.UserName == followingName && b.Blocked.UserName == followerName) ||
                              (b.Blocker.UserName == followerName && b.Blocked.UserName == followingName));

            if (isBlocked)
            {
                throw new ForbiddenException("Không thể theo dõi người dùng này");
            }

            // Tạo mối quan hệ follow mới
            var newFollow = new Entities.Domain.Follow
            {
                Id = Guid.NewGuid(),
                FollowerId = follower.Id,
                FollowingId = following.Id,
                FollowedAt = DateTime.UtcNow,
                CreatedBy = follower.Id,
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = follower.Id,
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

        public async Task<bool> UnfollowUserAsync(string followerName, string followingName)
        {
            var follow = await _context.Follows
                .FirstOrDefaultAsync(f => f.Follower.UserName == followerName && f.Following.UserName == followingName);

            if (follow == null)
            {
                return false;
            }

            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PaginationResponse<UserDto>> GetFollowersAsync(string userName, int page = 1, int pageSize = 10)
        {
            // Kiểm tra user tồn tại
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng");
            }

            // Lấy tổng số lượng followers
            var totalFollowers = await _context.Follows
                .Where(f => f.Following.UserName == userName)
                .CountAsync();

            // Phân trang và lấy danh sách người theo dõi
            var followers = await _context.Follows
                .Where(f => f.Following.UserName == userName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new UserDto
                {
                    Id = f.Follower.Id,
                    UserName = f.Follower.UserName,
                    Email = f.Follower.Email,
                    AvatarUrl = f.Follower.AvatarId,
                    Reputation = f.Follower.Reputation,
                    IsFollowing = _context.Follows.Any(ff => ff.Follower.UserName == userName && ff.Following.UserName == f.Follower.UserName)
                })
                .ToListAsync();

            // Kiểm tra các người dùng đã bị chặn chưa
            var blockedUserNames = await _context.UserBlocks
                .Where(b => b.Blocker.UserName == userName)
                .Select(b => b.Blocked.UserName)
                .ToListAsync();

            foreach (var follower in followers)
            {
                follower.IsBlocked = blockedUserNames.Contains(follower.UserName);
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

        public async Task<PaginationResponse<UserDto>> GetFollowingAsync(string userName, int page = 1, int pageSize = 10)
        {
            // Kiểm tra user tồn tại
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng");
            }

            // Lấy tổng số lượng following
            var totalFollowing = await _context.Follows
                .Where(f => f.Follower.UserName == userName)
                .CountAsync();

            // Phân trang và lấy danh sách người đang theo dõi
            var following = await _context.Follows
                .Where(f => f.Follower.UserName == userName)
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
            var blockedUserNames = await _context.UserBlocks
                .Where(b => b.Blocker.UserName == userName)
                .Select(b => b.Blocked.UserName)
                .ToListAsync();

            foreach (var follow in following)
            {
                follow.IsBlocked = blockedUserNames.Contains(follow.UserName);
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

        public async Task<PaginationResponse<UserDto>> SearchUsersToFollowAsync(string currentUserName, SearchUserRequest request)
        {
            // Lấy danh sách user theo search term
            var query = _context.Users
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
                    IsFollowing = _context.Follows.Any(f => f.Follower.UserName == currentUserName && f.Following.UserName == u.UserName),
                    IsBlocked = _context.UserBlocks.Any(b => b.Blocker.UserName == currentUserName && b.Blocked.UserName == u.UserName)
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

        public async Task<bool> RemoveFollowerAsync(string userName, string followerToRemoveName)
        {
            var follow = await _context.Follows
                .FirstOrDefaultAsync(f => f.Following.UserName == userName && f.Follower.UserName == followerToRemoveName);

            if (follow == null)
            {
                return false;
            }

            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BlockUserAsync(string userName, string userToBlockName)
        {
            if (userName == userToBlockName)
            {
                throw new BadRequestException("Không thể chặn chính bạn");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            var userToBlock = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userToBlockName);

            if (user == null || userToBlock == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng");
            }

            var existingBlock = await _context.UserBlocks
                .FirstOrDefaultAsync(b => b.Blocker.UserName == userName && b.Blocked.UserName == userToBlockName);

            if (existingBlock != null)
            {
                throw new BadRequestException("Bạn đã chặn người dùng này rồi");
            }

            // Tự động unfollow nếu đang follow
            var existingFollow = await _context.Follows
                .FirstOrDefaultAsync(f => (f.Follower.UserName == userName && f.Following.UserName == userToBlockName) ||
                                        (f.Follower.UserName == userToBlockName && f.Following.UserName == userName));
            if (existingFollow != null)
            {
                _context.Follows.Remove(existingFollow);
            }

            var block = new UserBlock
            {
                Id = Guid.NewGuid(),
                BlockerId = user.Id,
                BlockedId = userToBlock.Id,
                BlockedAt = DateTime.UtcNow,
                CreatedBy = user.Id,
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = user.Id,
                UpdatedOn = DateTime.UtcNow
            };

            _context.UserBlocks.Add(block);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnblockUserAsync(string userName, string userToUnblockName)
        {
            var block = await _context.UserBlocks
                .FirstOrDefaultAsync(b => b.Blocker.UserName == userName && b.Blocked.UserName == userToUnblockName);

            if (block == null)
            {
                return false;
            }

            _context.UserBlocks.Remove(block);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PaginationResponse<UserDto>> GetBlockedUsersAsync(string userName, int page = 1, int pageSize = 10)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                throw new NotFoundException("Không tìm thấy người dùng");
            }

            var totalBlocked = await _context.UserBlocks
                .Where(b => b.Blocker.UserName == userName)
                .CountAsync();

            var blockedUsers = await _context.UserBlocks
                .Where(b => b.Blocker.UserName == userName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new UserDto
                {
                    Id = b.Blocked.Id,
                    UserName = b.Blocked.UserName,
                    Email = b.Blocked.Email,
                    AvatarUrl = b.Blocked.AvatarId,
                    Reputation = b.Blocked.Reputation,
                    IsBlocked = true,
                    IsFollowing = false // Không thể following người bị block
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
