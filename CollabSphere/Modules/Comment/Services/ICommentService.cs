using System;
using System.Threading.Tasks;

using CollabSphere.Modules.Comment.Models;

namespace CollabSphere.Modules.Comment.Services;

public interface ICommentService
{
    Task<CommentResponse> CreateCommentAsync(CreateCommentRequest request, Guid userId);
    Task<CommentResponse> GetCommentByIdAsync(Guid commentId);
}
