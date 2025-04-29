using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CollabSphere.Database;
using CollabSphere.Entities.Domain;
using CollabSphere.Exceptions;
using CollabSphere.Modules.Comment.Models;

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
}
