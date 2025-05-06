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

        Task<Entities.Domain.Post> CreatePostAsync(CreatePostDto createPostDto);

        Task<PostResponseModel> UpdatePostAsync(Guid id, UpdatePostDto model, Guid updatedByUserId);
        Task<bool> DeletePostAsync(Guid id, Guid deletedByUserId);
        Task<List<PostDto>> GetAllPostByUserId(Guid userId);
        Task<PaginationResponse<PostDto>> GetPaginatedPostsByUserId(Guid userId, PaginationRequest request);
        Task<bool> VotePostAsync(Guid postId, Guid userId, VoteType voteType);
        Task<List<PostDto>> GetHomePostsAsync(int pageNumber, int pageSize);
        Task<List<PostDto>> GetPopularPostsAsync(int pageNumber, int pageSize);
        Task<List<PostDto>> GetPostsByUpDownVoteAsync(Guid userId, string getBy);
        Task<List<PostDto>> GetRecentPostsFromFollowedUsersAsync(Guid userId, int count = 3);
    }
}

