using System.Security.Claims;

using AutoMapper;

using CollabSphere.Database;
using CollabSphere.Entities.Domain;
using CollabSphere.Exceptions;
using CollabSphere.Modules.Posts.Dtos;
using CollabSphere.Modules.Posts.Models;
using CollabSphere.Modules.Posts.Service;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CollabSphere.Modules.Posts.Service.Impl;

public class PostService : IPostService
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public PostService(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<PostDto>> GetAllPostsAsync()
    {

        var posts = await _context.Posts.ToListAsync();
        return _mapper.Map<List<PostDto>>(posts);
    }

    public async Task<PostDto> GetPostByIdAsync(Guid postId)
    {
        var post = await _context.Posts.FindAsync(postId);
        if (post == null) throw new NotFoundException("Post not found");

        return _mapper.Map<PostDto>(post);
    }

    public async Task<PostDto> CreatePostAsync(CreatePostDto createPostDto)
    {
        if (createPostDto == null)
        {
            throw new ArgumentNullException(nameof(createPostDto), "You should ");
        }

        try
        {
            var post = _mapper.Map<CollabSphere.Entities.Domain.Post>(createPostDto) ?? throw new InvalidOperationException("Mapping failed.");

            post.Id = Guid.NewGuid();
            post.CreatedOn = DateTime.UtcNow;
            post.UpvoteCount = 0;
            post.DownvoteCount = 0;
            post.ShareCount = 0;

            _context.Posts.Add(post);
            await _context.SaveChangesAsync(); // EF tự động quản lý transaction

            return _mapper.Map<PostDto>(post);
        }
        catch (DbUpdateException dbEx)
        {
            var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
            throw new Exception($"Database error while creating post: {innerMessage}", dbEx);
        }
        catch (AutoMapperMappingException mapEx)
        {
            throw new Exception($"Mapping error: {mapEx.Message}", mapEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"Unexpected error while creating post: {ex.Message}", ex);
        }
    }



    public async Task<PostResponseModel> UpdatePostAsync(Guid id, UpdatePostModel model, Guid updatedByUserId)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
            throw new KeyNotFoundException("Bài post không tồn tại");

        var updatedUser = await _context.Users.FindAsync(updatedByUserId);
        if (updatedUser == null)
            throw new KeyNotFoundException("Người dùng không tồn tại");

        post.Title = model.Title;
        post.Content = model.Content;
        post.ThumbnailUrl = model.ThumbnailUrl;
        post.UpdatedOn = DateTime.UtcNow;
        post.UpdatedBy = updatedByUserId; // Lưu GUID của user

        _context.Posts.Update(post);
        await _context.SaveChangesAsync();

        return new PostResponseModel
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            ThumbnailUrl = post.ThumbnailUrl,
            SubredditId = post.SubredditId,
            UpdatedBy = post.UpdatedBy,
            UpdatedOn = post.UpdatedOn,
            UpdatedByUsername = updatedUser.UserName
        };
    }



    public async Task<bool> DeletePostAsync(Guid id, Guid deletedByUserId)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
            return false;


        if (post.CreatedBy != deletedByUserId)
            return false;

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<List<PostDto>> GetAllPostByUserId(Guid getPostByUserId)
    {
        var posts = await _context.Posts
            .Where(post => post.CreatedBy == getPostByUserId)
            .Include(post => post.Comments) // ✅ Lấy danh sách comments
            .Include(post => post.Votes)    // ✅ Lấy danh sách votes
            .Include(post => post.Shares)   // ✅ Lấy danh sách shares
            .Include(post => post.Reports)  // ✅ Lấy danh sách reports
            .Select(post => new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                ThumbnailUrl = post.ThumbnailUrl,
                CreatedBy = post.CreatedBy,
                CreatedOn = post.CreatedOn,
                UpvoteCount = post.Votes.Count(v => v.VoteType == "upvote"),
                DownvoteCount = post.Votes.Count(v => v.VoteType == "downvote"),
                ShareCount = post.Shares.Count,

                // ✅ Chỉ hiển thị danh sách (không gán)
                Comments = post.Comments,
                Votes = post.Votes,
                Shares = post.Shares,
                Reports = post.Reports
            })
            .ToListAsync();

        return posts;
    }



}
