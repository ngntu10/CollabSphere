using System.Security.Claims;

using AutoMapper;

using CollabSphere.Common;
using CollabSphere.Database;
using CollabSphere.Entities.Domain;
using CollabSphere.Exceptions;
using CollabSphere.Infrastructures.Repositories;
using CollabSphere.Modules.Post.Models;
using CollabSphere.Modules.Posts.Models;
using CollabSphere.Modules.Posts.Service;
using CollabSphere.Modules.Posts.Specifications;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CollabSphere.Modules.Posts.Service.Imp;

public class PostService : IPostService
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;
    private readonly IBaseRepository<Entities.Domain.Post> _postRepository;

    public PostService(DatabaseContext context, IMapper mapper, IBaseRepository<Entities.Domain.Post> postRepository)
    {
        _context = context;
        _mapper = mapper;
        _postRepository = postRepository;
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

    public async Task<List<PostDto>> GetPostsByUpDownVoteAsync(Guid userId, string getBy)
    {
        List<Entities.Domain.Post> posts;

        if (getBy == "upvote")
        {
            posts = await _context.Posts
                .Where(p => p.CreatedBy == userId && p.UpvoteCount >= p.DownvoteCount)
                .ToListAsync();
        }
        else if (getBy == "downvote")
        {
            posts = await _context.Posts
                .Where(p => p.CreatedBy == userId && p.DownvoteCount > p.UpvoteCount)
                .ToListAsync();
        }
        else
        {
            throw new ArgumentException("Giá trị getBy không hợp lệ. Chỉ chấp nhận 'upvote' hoặc 'downvote'.");
        }

        return _mapper.Map<List<PostDto>>(posts);
    }

    public async Task<Entities.Domain.Post> CreatePostAsync(CreatePostDto createPostDto)
    {
        if (createPostDto == null)
        {
            throw new ArgumentNullException(nameof(createPostDto), "CreatePostDto không được null.");
        }

        try
        {
            // Validate required fields
            if (string.IsNullOrEmpty(createPostDto.Title))
                throw new ArgumentException("Title is required");

            if (string.IsNullOrEmpty(createPostDto.Category))
                throw new ArgumentException("Category is required");
            if (createPostDto.UserId == Guid.Empty)
                throw new ArgumentException("UserId is required");


            // Ánh xạ từ CreatePostDto sang Post
            var post = _mapper.Map<CollabSphere.Entities.Domain.Post>(createPostDto)
                    ?? throw new InvalidOperationException("Mapping failed.");

            // Cập nhật các trường khác cho Post
            post.Id = Guid.NewGuid();
            post.CreatedOn = DateTime.UtcNow;
            post.UpvoteCount = 0;
            post.DownvoteCount = 0;
            post.ShareCount = 0;
            post.CreatedBy = createPostDto.UserId;
            // Xử lý PostImages nếu có
            if (createPostDto.PostImages != null && createPostDto.PostImages.Any())
            {
                post.PostImages = createPostDto.PostImages
                    .Select(imageId => new PostImages { ImageID = imageId })
                    .ToList();
            }

            // Thêm Post vào DbContext và lưu vào cơ sở dữ liệu
            _context.Posts.Add(post);
            await _context.SaveChangesAsync(); // Lưu vào cơ sở dữ liệu

            return post;
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
        post.UpdatedOn = DateTime.UtcNow;
        post.UpdatedBy = updatedByUserId; // Lưu GUID của user

        _context.Posts.Update(post);
        await _context.SaveChangesAsync();

        return new PostResponseModel
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
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
            .Include(post => post.Comments)
            .Include(post => post.Votes)
            .Include(post => post.Shares)
            .Include(post => post.Reports)
            .Select(post => new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedBy = post.CreatedBy,
                CreatedOn = post.CreatedOn,
                UpvoteCount = post.Votes.Count(v => v.VoteType == "upvote"),
                DownvoteCount = post.Votes.Count(v => v.VoteType == "downvote"),
                ShareCount = post.Shares.Count,
                Comments = post.Comments,
                Votes = post.Votes,
                Shares = post.Shares,
                Reports = post.Reports
            })
            .ToListAsync();

        return posts;
    }

    public async Task<PaginationResponse<PostDto>> GetPaginatedPostsByUserId(Guid userId, PaginationRequest request)
    {
        var skip = (request.Page - 1) * request.PageSize;
        var spec = PostSpecification.GetPaginatedPostsByUserId(userId, skip, request.PageSize);
        var countSpec = PostSpecification.GetPostCountByUserId(userId);

        var posts = await _postRepository.GetAllAsync(spec);
        var total = await _postRepository.CountAsync(countSpec);

        var postDtos = posts.Select(post => new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            CreatedBy = post.CreatedBy,
            CreatedOn = post.CreatedOn,
            UpvoteCount = post.Votes.Count(v => v.VoteType == "upvote"),
            DownvoteCount = post.Votes.Count(v => v.VoteType == "downvote"),
            ShareCount = post.Shares.Count,
            Comments = post.Comments,
            Votes = post.Votes,
            Shares = post.Shares,
            Reports = post.Reports
        }).ToList();

        var totalPages = (int) Math.Ceiling(total / (double) request.PageSize);

        return new PaginationResponse<PostDto>(
            request.Page,
            totalPages,
            request.PageSize,
            total,
            postDtos
        );
    }

    private async Task UpdatePostVoteCount(Guid postId)
    {
        var post = await _context.Posts.FindAsync(postId);
        if (post == null)
            return;
        post.UpvoteCount = await _context.Votes.CountAsync(v => v.PostId == postId && v.VoteType == "Upvote");
        post.DownvoteCount = await _context.Votes.CountAsync(v => v.PostId == postId && v.VoteType == "Downvote");

        _context.Posts.Update(post);
        await _context.SaveChangesAsync();
    }
    public async Task<bool> VotePostAsync(Guid postId, Guid userId, VoteType voteType)
    {
        var post = await _context.Posts.FindAsync(postId);
        if (post == null)
            throw new NotFoundException("Bài post không tồn tại");

        var existingVote = await _context.Votes
            .FirstOrDefaultAsync(v => v.PostId == postId && v.UserId == userId);

        if (existingVote != null)
        {
            Console.WriteLine($"Existing Vote: {existingVote?.Id}, UserId: {existingVote?.UserId}, PostId: {existingVote?.PostId}, VoteType: {existingVote?.VoteType}");
            if (existingVote.VoteType == voteType.ToString())
            {
                _context.Votes.Remove(existingVote);
                await _context.SaveChangesAsync();
                await UpdatePostVoteCount(postId);

                return false;
            }
            existingVote.VoteType = voteType.ToString();
            existingVote.UpdatedOn = DateTime.UtcNow;
            existingVote.UpdatedBy = userId;
            _context.Votes.Update(existingVote);
        }
        else
        {
            var vote = new Vote
            {
                Id = Guid.NewGuid(),
                PostId = postId,
                UserId = userId,
                VoteType = voteType.ToString(),
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = userId,
                UpdatedOn = DateTime.UtcNow
            };
            _context.Votes.Add(vote);

            await _context.SaveChangesAsync();

        }
        await _context.SaveChangesAsync();
        await UpdatePostVoteCount(postId);


        return true;
    }

}

