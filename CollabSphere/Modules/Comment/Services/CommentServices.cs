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

        // Return response
        return new CommentResponse
        {
            Id = comment.Id,
            Content = comment.Content,
            UserId = comment.UserId,
            PostId = comment.PostId,
            ParentCommentId = comment.ParentCommentId,
            Score = comment.Score,
            CreatedOn = comment.CreatedOn,
            Replies = new List<CommentResponse>()
        };
    }

    public async Task<CommentResponse> GetCommentByIdAsync(Guid commentId)
    {
        var comment = await _context.Comments
            .Include(c => c.ChildComments)
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null)
        {
            throw new NotFoundException("Bình luận không tồn tại");
        }

        var response = new CommentResponse
        {
            Id = comment.Id,
            Content = comment.Content,
            UserId = comment.UserId,
            PostId = comment.PostId,
            ParentCommentId = comment.ParentCommentId,
            Score = comment.Score,
            CreatedOn = comment.CreatedOn,
            Replies = comment.ChildComments.Select(c => new CommentResponse
            {
                Id = c.Id,
                Content = c.Content,
                UserId = c.UserId,
                PostId = c.PostId,
                ParentCommentId = c.ParentCommentId,
                Score = c.Score,
                CreatedOn = c.CreatedOn,
                Replies = new List<CommentResponse>()
            }).ToList()
        };

        return response;
    }

    public async Task<List<CommentResponse>> GetCommentsByPostIdAsync(Guid postId)
    {
        // Check if post exists
        var postExists = await _context.Posts.AnyAsync(p => p.Id == postId);
        if (!postExists)
        {
            throw new NotFoundException("Bài viết không tồn tại");
        }

        // Get all root comments (comments without parent) for the post
        var comments = await _context.Comments
            .Include(c => c.ChildComments)
            .Where(c => c.PostId == postId && c.ParentCommentId == null)
            .OrderByDescending(c => c.CreatedOn)
            .ToListAsync();

        // Map to response model with nested replies
        var response = comments.Select(c => new CommentResponse
        {
            Id = c.Id,
            Content = c.Content,
            UserId = c.UserId,
            PostId = c.PostId,
            ParentCommentId = c.ParentCommentId,
            Score = c.Score,
            CreatedOn = c.CreatedOn,
            Replies = c.ChildComments.Select(reply => new CommentResponse
            {
                Id = reply.Id,
                Content = reply.Content,
                UserId = reply.UserId,
                PostId = reply.PostId,
                ParentCommentId = reply.ParentCommentId,
                Score = reply.Score,
                CreatedOn = reply.CreatedOn,
                Replies = new List<CommentResponse>()
            }).OrderByDescending(r => r.CreatedOn).ToList()
        }).ToList();

        return response;
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

        // Cập nhật nội dung
        comment.Content = request.Content;
        comment.UpdatedBy = userId;
        comment.UpdatedOn = DateTime.Now;

        await _context.SaveChangesAsync();

        // Return response
        return new CommentResponse
        {
            Id = comment.Id,
            Content = comment.Content,
            UserId = comment.UserId,
            PostId = comment.PostId,
            ParentCommentId = comment.ParentCommentId,
            Score = comment.Score,
            CreatedOn = comment.CreatedOn,
            Replies = comment.ChildComments.Select(c => new CommentResponse
            {
                Id = c.Id,
                Content = c.Content,
                UserId = c.UserId,
                PostId = c.PostId,
                ParentCommentId = c.ParentCommentId,
                Score = c.Score,
                CreatedOn = c.CreatedOn,
                Replies = new List<CommentResponse>()
            }).OrderByDescending(r => r.CreatedOn).ToList()
        };
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
            .Include(c => c.ChildComments)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedOn)
            .ToListAsync();

        // Map to response model
        var response = comments.Select(c => new CommentResponse
        {
            Id = c.Id,
            Content = c.Content,
            UserId = c.UserId,
            PostId = c.PostId,
            ParentCommentId = c.ParentCommentId,
            Score = c.Score,
            CreatedOn = c.CreatedOn,
            Replies = c.ChildComments.Select(reply => new CommentResponse
            {
                Id = reply.Id,
                Content = reply.Content,
                UserId = reply.UserId,
                PostId = reply.PostId,
                ParentCommentId = reply.ParentCommentId,
                Score = reply.Score,
                CreatedOn = reply.CreatedOn,
                Replies = new List<CommentResponse>()
            }).OrderByDescending(r => r.CreatedOn).ToList()
        }).ToList();

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

        // Lấy danh sách các comment mà người dùng đã vote theo loại vote
        var comments = await _context.Comments
            .Include(c => c.ChildComments)
            .Where(c => c.Votes.Any(v => v.UserId == userId &&
                                    (v.VoteType.ToLower() == normalizedVoteType ||
                                     v.VoteType == capitalizedVoteType)))
            .OrderByDescending(c => c.CreatedOn)
            .ToListAsync();

        // Map to response model
        var response = comments.Select(c => new CommentResponse
        {
            Id = c.Id,
            Content = c.Content,
            UserId = c.UserId,
            PostId = c.PostId,
            ParentCommentId = c.ParentCommentId,
            Score = c.Score,
            CreatedOn = c.CreatedOn,
            Replies = c.ChildComments.Select(reply => new CommentResponse
            {
                Id = reply.Id,
                Content = reply.Content,
                UserId = reply.UserId,
                PostId = reply.PostId,
                ParentCommentId = reply.ParentCommentId,
                Score = reply.Score,
                CreatedOn = reply.CreatedOn,
                Replies = new List<CommentResponse>()
            }).OrderByDescending(r => r.CreatedOn).ToList()
        }).ToList();

        return response;
    }
}
