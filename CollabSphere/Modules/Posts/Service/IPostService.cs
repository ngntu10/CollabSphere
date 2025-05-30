using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CollabSphere.Common;
using CollabSphere.Entities.Domain;
using CollabSphere.Modules.Post.Models;
using CollabSphere.Modules.Posts.Models;

namespace CollabSphere.Modules.Posts.Service
{
    public interface IPostService
    {
        Task<List<PostDto>> GetAllPostsAsync();

        Task<PostDto> GetPostByIdAsync(Guid postId);

        Task<Entities.Domain.Post> CreatePostAsync(CreatePostDto createPostDto, Guid userId);

        Task<PostResponseModel> UpdatePostAsync(Guid id, UpdatePostDto model, Guid updatedByUserId);
        Task<bool> DeletePostAsync(Guid id, Guid deletedByUserId);
        Task<List<PostDto>> GetAllPostByUserId(Guid userId);
        Task<PaginationResponse<PostDto>> GetPaginatedPostsByUserId(Guid userId, PaginationRequest request);
        Task<bool> VotePostAsync(Guid postId, Guid userId, VoteType voteType);
        Task<List<PostDto>> GetHomePostsAsync();
        Task<PaginationResponse<PostDto>> GetPopularPostsAsync(int pageNumber = 1, int pageSize = 10);
        Task<List<PostDto>> GetPostsByUpDownVoteAsync(Guid userId, string getBy);
        Task<PaginationResponse<PostDto>> GetRecentPostsFromFollowedUsersAsync(Guid userId, int pageNumber = 1, int pageSize = 10);
        Task<List<PostDto>> SearchPostsAsync(string searchTerm, int pageNumber = 1, int pageSize = 10);
        Task<List<PostDto>> GetUserVotedPostsAsync(Guid userId, string voteType);
    }
}

