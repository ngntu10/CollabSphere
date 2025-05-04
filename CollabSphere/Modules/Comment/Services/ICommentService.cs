using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CollabSphere.Modules.Comment.Models;

namespace CollabSphere.Modules.Comment.Services;

public interface ICommentService
{
    Task<CommentResponse> CreateCommentAsync(CreateCommentRequest request, Guid userId);
    Task<CommentResponse> GetCommentByIdAsync(Guid commentId);
    Task<List<CommentResponse>> GetCommentsByPostIdAsync(Guid postId);
    Task<bool> DeleteCommentAsync(Guid commentId, Guid userId);
    Task<CommentResponse> UpdateCommentAsync(Guid commentId, UpdateCommentRequest request, Guid userId);
}
