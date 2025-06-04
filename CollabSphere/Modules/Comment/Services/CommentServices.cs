using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CollabSphere.Database;
using CollabSphere.Entities.Domain;
using CollabSphere.Exceptions;
using CollabSphere.Modules.Comment.Models;
using CollabSphere.Modules.Posts.Models;

using Microsoft.EntityFrameworkCore;

namespace CollabSphere.Modules.Comment.Services;

public class CommentServices : ICommentService
{
    private readonly DatabaseContext _context;

    public CommentServices(DatabaseContext context)
    {
        _context = context;
    }

    private async Task<CommentResponse> MapCommentToResponseAsync(CollabSphere.Entities.Domain.Comment comment)
    {
        Console.WriteLine($"Mapping comment {comment.Id}");
        Console.WriteLine($"Raw votes count: {comment.Votes?.Count ?? 0}");
        if (comment.Votes != null)
        {
            foreach (var vote in comment.Votes)
            {
                Console.WriteLine($"Vote: Id={vote.Id}, UserId={vote.UserId}, Type={vote.VoteType}");
            }
        }

        var user = await _context.Users
            .Where(u => u.Id == comment.UserId)
            .Select(u => new CollabSphere.Modules.Comment.Models.CommentUserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                AvatarId = u.AvatarId
            })
            .FirstOrDefaultAsync();

        // Ensure votes are properly loaded and counted
        var votes = comment.Votes?.ToList() ?? new List<Vote>();
        Console.WriteLine($"Processed votes count: {votes.Count}");

        var upvoteCount = votes.Count(v => v.VoteType?.ToLower() == "upvote");
        var downvoteCount = votes.Count(v => v.VoteType?.ToLower() == "downvote");
        Console.WriteLine($"Upvotes: {upvoteCount}, Downvotes: {downvoteCount}");

        var voteDtos = votes.Select(v => new CommentVoteDto
        {
            Id = v.Id,
            CommentId = comment.Id,
            UserId = v.UserId,
            VoteType = v.VoteType,
            User = v.User == null ? null : new CommentVoteUserDto
            {
                Id = v.User.Id,
                UserName = v.User.UserName,
                AvatarId = v.User.AvatarId
            },
            CreatedOn = v.CreatedOn,
            CreatedBy = v.CreatedBy,
            UpdatedOn = v.UpdatedOn,
            UpdatedBy = v.UpdatedBy
        }).ToList();
        Console.WriteLine($"Mapped vote DTOs count: {voteDtos.Count}");

        var childComments = new List<CommentResponse>();
        if (comment.ChildComments != null && comment.ChildComments.Any())
        {
            foreach (var child in comment.ChildComments.OrderByDescending(c => c.CreatedOn))
            {
                childComments.Add(await MapCommentToResponseAsync(child));
            }
        }

        return new CommentResponse
        {
            Id = comment.Id,
            Content = comment.Content,
            User = user,
            PostId = comment.PostId,
            ParentCommentId = comment.ParentCommentId,
            UpvoteCount = upvoteCount,
            DownvoteCount = downvoteCount,
            Votes = voteDtos,
            CreatedOn = comment.CreatedOn,
            ChildComments = childComments
        };
    }

    public async Task<CommentResponse> CreateCommentAsync(CreateCommentRequest request, Guid userId)
    {
        // Check if post exists
        var postExists = await _context.Posts.AnyAsync(p => p.Id == request.PostId);
        if (!postExists)
        {
            throw new NotFoundException("Bài viết không tồn tại");
        }

        // Check if parent comment exists (if provided) and belongs to the same post
        if (request.ParentCommentId.HasValue)
        {
            var parentComment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == request.ParentCommentId.Value);

            if (parentComment == null)
            {
                throw new NotFoundException("Bình luận cha không tồn tại");
            }

            if (parentComment.PostId != request.PostId)
            {
                throw new BadRequestException("Bình luận cha không thuộc bài viết này");
            }
        }

        // Create new comment
        var comment = new CollabSphere.Entities.Domain.Comment
        {
            Content = request.Content,
            UserId = userId,
            PostId = request.PostId,
            ParentCommentId = request.ParentCommentId,
            Score = 0,
            CreatedOn = DateTime.Now,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        return await MapCommentToResponseAsync(comment);
    }

    public async Task<CommentResponse> GetCommentByIdAsync(Guid commentId)
    {
        var comment = await _context.Comments
            .Include(c => c.Votes).ThenInclude(v => v.User)
            .Include(c => c.ChildComments)
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null)
        {
            throw new NotFoundException("Bình luận không tồn tại");
        }

        return await MapCommentToResponseAsync(comment);
    }

    public async Task<List<CommentResponse>> GetCommentsByPostIdAsync(Guid postId)
    {
        // Check if post exists
        var postExists = await _context.Posts.AnyAsync(p => p.Id == postId);
        if (!postExists)
        {
            throw new NotFoundException("Bài viết không tồn tại");
        }

        // Get all comments for the post with votes and users
        var allComments = await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Votes)
                .ThenInclude(v => v.User)
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CreatedOn)
            .AsSplitQuery()
            .ToListAsync();

        // Organize comments into a hierarchy
        var rootComments = allComments
            .Where(c => c.ParentCommentId == null)
            .ToList();

        var commentsByParentId = allComments
            .Where(c => c.ParentCommentId != null)
            .GroupBy(c => c.ParentCommentId.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Map root comments and their children
        var response = new List<CommentResponse>();
        foreach (var comment in rootComments)
        {
            var commentResponse = await MapCommentToResponseAsync(comment);
            MapChildComments(commentResponse, commentsByParentId);
            response.Add(commentResponse);
        }

        return response;
    }

    private void MapChildComments(CommentResponse parentComment, Dictionary<Guid, List<CollabSphere.Entities.Domain.Comment>> commentsByParentId)
    {
        if (!commentsByParentId.ContainsKey(parentComment.Id))
            return;

        var childComments = commentsByParentId[parentComment.Id];
        foreach (var childComment in childComments.OrderByDescending(c => c.CreatedOn))
        {
            // Ensure votes are properly loaded and counted
            var votes = childComment.Votes?.ToList() ?? new List<Vote>();
            var upvoteCount = votes.Count(v => v.VoteType?.ToLower() == "upvote");
            var downvoteCount = votes.Count(v => v.VoteType?.ToLower() == "downvote");

            var childResponse = new CommentResponse
            {
                Id = childComment.Id,
                Content = childComment.Content,
                User = childComment.User == null ? null : new CollabSphere.Modules.Comment.Models.CommentUserDto
                {
                    Id = childComment.User.Id,
                    UserName = childComment.User.UserName,
                    AvatarId = childComment.User.AvatarId
                },
                PostId = childComment.PostId,
                ParentCommentId = childComment.ParentCommentId,
                UpvoteCount = upvoteCount,
                DownvoteCount = downvoteCount,
                Votes = votes.Select(v => new CommentVoteDto
                {
                    Id = v.Id,
                    CommentId = childComment.Id,
                    UserId = v.UserId,
                    VoteType = v.VoteType,
                    User = v.User == null ? null : new CommentVoteUserDto
                    {
                        Id = v.User.Id,
                        UserName = v.User.UserName,
                        AvatarId = v.User.AvatarId
                    },
                    CreatedOn = v.CreatedOn,
                    CreatedBy = v.CreatedBy,
                    UpdatedOn = v.UpdatedOn,
                    UpdatedBy = v.UpdatedBy
                }).ToList(),
                CreatedOn = childComment.CreatedOn,
                ChildComments = new List<CommentResponse>()
            };

            parentComment.ChildComments.Add(childResponse);
            MapChildComments(childResponse, commentsByParentId);
        }
    }

    public async Task<bool> DeleteCommentAsync(Guid commentId, Guid userId)
    {
        var comment = await _context.Comments
            .Include(c => c.ChildComments)
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null)
        {
            throw new NotFoundException("Bình luận không tồn tại");
        }

        // Kiểm tra quyền xóa (chỉ người tạo comment mới được xóa)
        if (comment.UserId != userId)
        {
            throw new UnauthorizedAccessException("Bạn không có quyền xóa bình luận này");
        }

        // Xóa tất cả các reply của comment này (nếu có)
        if (comment.ChildComments != null && comment.ChildComments.Any())
        {
            _context.Comments.RemoveRange(comment.ChildComments);
        }

        // Xóa comment chính
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<CommentResponse> UpdateCommentAsync(Guid commentId, UpdateCommentRequest request, Guid userId)
    {
        var comment = await _context.Comments
            .Include(c => c.ChildComments)
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null)
        {
            throw new NotFoundException("Bình luận không tồn tại");
        }

        // Kiểm tra quyền cập nhật (chỉ người tạo comment mới được cập nhật)
        if (comment.UserId != userId)
        {
            throw new UnauthorizedAccessException("Bạn không có quyền cập nhật bình luận này");
        }

        comment.Content = request.Content;
        comment.UpdatedBy = userId;
        comment.UpdatedOn = DateTime.Now;

        await _context.SaveChangesAsync();

        return await MapCommentToResponseAsync(comment);
    }

    private async Task UpdateCommentScoreAsync(Guid commentId)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null)
            return;

        // Đếm số lượng upvote và downvote
        int upvotes = await _context.Votes.CountAsync(v => v.CommentId == commentId && v.VoteType.ToLower() == "upvote");
        int downvotes = await _context.Votes.CountAsync(v => v.CommentId == commentId && v.VoteType.ToLower() == "downvote");

        // Tính toán điểm (upvotes - downvotes)
        comment.Score = upvotes - downvotes;

        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> VoteCommentAsync(Guid commentId, Guid userId, VoteType voteType)
    {
        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null)
            throw new NotFoundException("Bình luận không tồn tại");

        var existingVote = await _context.Votes
            .FirstOrDefaultAsync(v => v.CommentId == commentId && v.UserId == userId);

        if (existingVote != null)
        {
            // Nếu vote type giống với vote cũ -> xóa vote (hủy vote)
            if (existingVote.VoteType.ToLower() == voteType.ToString().ToLower())
            {
                _context.Votes.Remove(existingVote);
                await _context.SaveChangesAsync();
                await UpdateCommentScoreAsync(commentId);
                return false;
            }

            // Nếu vote type khác với vote cũ -> cập nhật vote
            existingVote.VoteType = voteType.ToString();
            existingVote.UpdatedOn = DateTime.UtcNow;
            existingVote.UpdatedBy = userId;
            _context.Votes.Update(existingVote);
        }
        else
        {
            // Nếu chưa từng vote -> tạo vote mới
            var vote = new Vote
            {
                Id = Guid.NewGuid(),
                CommentId = commentId,
                UserId = userId,
                VoteType = voteType.ToString(),
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = userId,
                UpdatedOn = DateTime.UtcNow
            };
            _context.Votes.Add(vote);
        }

        await _context.SaveChangesAsync();
        await UpdateCommentScoreAsync(commentId);

        return true;
    }

    public async Task<List<CommentResponse>> GetCommentsByUserIdAsync(Guid userId)
    {
        // Kiểm tra user có tồn tại không
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new NotFoundException("Người dùng không tồn tại");
        }

        // Lấy tất cả comment của user
        var comments = await _context.Comments
            .Include(c => c.Votes)
                .ThenInclude(v => v.User)
            .Include(c => c.User)
            .Include(c => c.ChildComments)
                .ThenInclude(cc => cc.Votes)
                    .ThenInclude(v => v.User)
            .Include(c => c.ChildComments)
                .ThenInclude(cc => cc.User)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedOn)
            .AsSplitQuery()
            .ToListAsync();

        var response = new List<CommentResponse>();
        foreach (var comment in comments)
        {
            response.Add(await MapCommentToResponseAsync(comment));
        }

        return response;
    }

    public async Task<List<CommentResponse>> GetUserVotedCommentsAsync(Guid userId, string voteType)
    {
        // Kiểm tra voteType hợp lệ
        if (string.IsNullOrEmpty(voteType) || (voteType.ToLower() != "upvote" && voteType.ToLower() != "downvote"))
            throw new ArgumentException("Giá trị voteType không hợp lệ. Chỉ chấp nhận 'upvote' hoặc 'downvote'.");

        // Chuẩn hóa loại vote
        var normalizedVoteType = voteType.ToLower() == "upvote" ? "upvote" : "downvote";
        var capitalizedVoteType = char.ToUpper(normalizedVoteType[0]) + normalizedVoteType.Substring(1);

        // Kiểm tra user có tồn tại không
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new NotFoundException("Người dùng không tồn tại");
        }

        Console.WriteLine($"Finding comments voted by user {userId} with vote type {normalizedVoteType}");

        // Lấy danh sách các comment mà người dùng đã vote theo loại vote
        var comments = await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Votes)
                .ThenInclude(v => v.User)
            .Include(c => c.ChildComments)
                .ThenInclude(cc => cc.User)
            .Include(c => c.ChildComments)
                .ThenInclude(cc => cc.Votes)
                    .ThenInclude(v => v.User)
            .Where(c => c.Votes.Any(v => v.UserId == userId &&
                                    (v.VoteType.ToLower() == normalizedVoteType ||
                                     v.VoteType == capitalizedVoteType)))
            .OrderByDescending(c => c.CreatedOn)
            .AsSplitQuery()
            .ToListAsync();

        Console.WriteLine($"Found {comments.Count} comments");
        foreach (var comment in comments)
        {
            Console.WriteLine($"Comment {comment.Id} has {comment.Votes?.Count ?? 0} votes");
        }

        var response = new List<CommentResponse>();
        foreach (var comment in comments)
        {
            response.Add(await MapCommentToResponseAsync(comment));
        }

        return response;
    }
}
