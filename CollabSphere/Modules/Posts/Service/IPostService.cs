using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CollabSphere.Modules.Posts.Dtos;
using CollabSphere.Modules.Posts.Models;

namespace CollabSphere.Modules.Posts.Service
{
    public interface IPostService
    {
        Task<List<PostDto>> GetAllPostsAsync();

        Task<PostDto> GetPostByIdAsync(Guid postId);

        Task<PostDto> CreatePostAsync(CreatePostDto createPostDto);

        Task<PostResponseModel> UpdatePostAsync(Guid id, UpdatePostModel model, Guid updatedByUserId);
        Task<bool> DeletePostAsync(Guid id, Guid deletedByUserId);
        Task<List<PostDto>> GetAllPostByUserId(Guid getPostByUserId);
    }
}

