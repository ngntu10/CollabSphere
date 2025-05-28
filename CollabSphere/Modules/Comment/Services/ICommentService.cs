using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CollabSphere.Modules.Comment.Models;
using CollabSphere.Modules.Posts.Models;

namespace CollabSphere.Modules.Comment.Services;

public interface ICommentService
{
    Task<CommentResponse> CreateCommentAsync(CreateCommentRequest request, Guid userId);
    Task<CommentResponse> GetCommentByIdAsync(Guid commentId);
    Task<List<CommentResponse>> GetCommentsByPostIdAsync(Guid postId);
    Task<bool> DeleteCommentAsync(Guid commentId, Guid userId);
    Task<CommentResponse> UpdateCommentAsync(Guid commentId, UpdateCommentRequest request, Guid userId);
    Task<bool> VoteCommentAsync(Guid commentId, Guid userId, VoteType voteType);
    Task<List<CommentResponse>> GetCommentsByUserIdAsync(Guid userId);
    Task<List<CommentResponse>> GetUserVotedCommentsAsync(Guid userId, string voteType);
}
