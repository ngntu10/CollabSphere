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
}
