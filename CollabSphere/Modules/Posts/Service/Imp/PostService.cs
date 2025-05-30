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

using CommentEntity = CollabSphere.Entities.Domain.Comment;

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

    private ICollection<CommentDto> MapComments(ICollection<CommentEntity> comments)
    {
        // Lấy các comment gốc (không có parent)
        var rootComments = comments.Where(c => c.ParentCommentId == null).ToList();

        // Tạo dictionary để map comment level 1 với tất cả các reply của nó (level 2+)
        var commentReplies = comments
            .Where(c => c.ParentCommentId.HasValue)
            .GroupBy(c => GetLevel1ParentId(c, comments))
            .ToDictionary(g => g.Key, g => g.OrderByDescending(c => c.CreatedOn).ToList());

        return rootComments.Select(c => MapCommentWithChildren(c, comments, commentReplies)).ToList();
    }

    private Guid GetLevel1ParentId(CommentEntity comment, ICollection<CommentEntity> allComments)
    {
        var parentComment = allComments.FirstOrDefault(c => c.Id == comment.ParentCommentId.Value);
        if (parentComment == null)
        {
            return comment.ParentCommentId.Value;
        }

        // Nếu parent là comment gốc (level 0), trả về ID của parent
        if (!parentComment.ParentCommentId.HasValue)
        {
            return parentComment.Id;
        }

        // Nếu parent không phải comment gốc, tìm parent level 1
        var level1Parent = allComments.FirstOrDefault(c => c.Id == parentComment.ParentCommentId.Value);
        if (level1Parent == null || !level1Parent.ParentCommentId.HasValue)
        {
            return parentComment.Id; // Parent là level 1
        }

        return parentComment.Id; // Parent là level 1
    }

    private CommentDto MapCommentWithChildren(CommentEntity comment, ICollection<CommentEntity> allComments, Dictionary<Guid, List<CommentEntity>> commentReplies)
    {
        // Ensure votes are loaded
        var votes = comment.Votes?.ToList() ?? new List<Vote>();
        var upvotes = votes.Count(v => v.VoteType?.ToLower() == "upvote");
        var downvotes = votes.Count(v => v.VoteType?.ToLower() == "downvote");

        // Lấy các comment con trực tiếp (level 1)
        var directChildren = allComments
            .Where(c => c.ParentCommentId == comment.Id)
            .OrderByDescending(c => c.CreatedOn);

        var childComments = new List<CommentDto>();
        foreach (var child in directChildren)
        {
            // Ensure child votes are loaded
            var childVotes = child.Votes?.ToList() ?? new List<Vote>();
            var childUpvotes = childVotes.Count(v => v.VoteType?.ToLower() == "upvote");
            var childDownvotes = childVotes.Count(v => v.VoteType?.ToLower() == "downvote");

            var childDto = new CommentDto
            {
                Id = child.Id,
                Content = child.Content,
                CreatedBy = child.CreatedBy,
                CreatedOn = child.CreatedOn,
                PostId = child.PostId,
                ParentCommentId = child.ParentCommentId,
                UpvoteCount = childUpvotes,
                DownvoteCount = childDownvotes,
                User = child.User == null ? null : new CommentUserDto
                {
                    Id = child.User.Id,
                    UserName = child.User.UserName,
                    AvatarId = child.User.AvatarId
                },
                Votes = childVotes.Select(v => new VoteDto
                {
                    Id = v.Id,
                    UserId = v.UserId,
                    VoteType = v.VoteType,
                    User = v.User == null ? null : new CommentUserDto
                    {
                        Id = v.User.Id,
                        UserName = v.User.UserName,
                        AvatarId = v.User.AvatarId
                    },
                    CreatedOn = v.CreatedOn,
                    CreatedBy = v.CreatedBy,
                    UpdatedOn = v.UpdatedOn,
                    UpdatedBy = v.UpdatedBy
                }).ToList(),
                ChildComments = new List<CommentDto>()
            };

            // Thêm các reply level 2+ vào comment level 1
            if (commentReplies.ContainsKey(child.Id))
            {
                foreach (var reply in commentReplies[child.Id])
                {
                    // Ensure reply votes are loaded
                    var replyVotes = reply.Votes?.ToList() ?? new List<Vote>();
                    var replyUpvotes = replyVotes.Count(v => v.VoteType?.ToLower() == "upvote");
                    var replyDownvotes = replyVotes.Count(v => v.VoteType?.ToLower() == "downvote");

                    childDto.ChildComments.Add(new CommentDto
                    {
                        Id = reply.Id,
                        Content = reply.Content,
                        CreatedBy = reply.CreatedBy,
                        CreatedOn = reply.CreatedOn,
                        PostId = reply.PostId,
                        ParentCommentId = reply.ParentCommentId,
                        UpvoteCount = replyUpvotes,
                        DownvoteCount = replyDownvotes,
                        User = reply.User == null ? null : new CommentUserDto
                        {
                            Id = reply.User.Id,
                            UserName = reply.User.UserName,
                            AvatarId = reply.User.AvatarId
                        },
                        Votes = replyVotes.Select(v => new VoteDto
                        {
                            Id = v.Id,
                            UserId = v.UserId,
                            VoteType = v.VoteType,
                            User = v.User == null ? null : new CommentUserDto
                            {
                                Id = v.User.Id,
                                UserName = v.User.UserName,
                                AvatarId = v.User.AvatarId
                            },
                            CreatedOn = v.CreatedOn,
                            CreatedBy = v.CreatedBy,
                            UpdatedOn = v.UpdatedOn,
                            UpdatedBy = v.UpdatedBy
                        }).ToList(),
                        ChildComments = new List<CommentDto>() // Level 2+ không có child comments
                    });
                }
            }

            childComments.Add(childDto);
        }

        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedBy = comment.CreatedBy,
            CreatedOn = comment.CreatedOn,
            PostId = comment.PostId,
            ParentCommentId = comment.ParentCommentId,
            UpvoteCount = upvotes,
            DownvoteCount = downvotes,
            User = comment.User == null ? null : new CommentUserDto
            {
                Id = comment.User.Id,
                UserName = comment.User.UserName,
                AvatarId = comment.User.AvatarId
            },
            Votes = votes.Select(v => new VoteDto
            {
                Id = v.Id,
                UserId = v.UserId,
                VoteType = v.VoteType,
                User = v.User == null ? null : new CommentUserDto
                {
                    Id = v.User.Id,
                    UserName = v.User.UserName,
                    AvatarId = v.User.AvatarId
                },
                CreatedOn = v.CreatedOn,
                CreatedBy = v.CreatedBy,
                UpdatedOn = v.UpdatedOn,
                UpdatedBy = v.UpdatedBy
            }).ToList(),
            ChildComments = childComments
        };
    }

    private ICollection<VoteDto> MapVotes(ICollection<Vote> votes)
    {
        return votes.Select(v => new VoteDto
        {
            Id = v.Id,
            PostId = v.PostId ?? Guid.Empty,
            UserId = v.UserId,
            VoteType = v.VoteType,
            User = v.User == null ? null : new CommentUserDto
            {
                Id = v.User.Id,
                UserName = v.User.UserName,
                AvatarId = v.User.AvatarId
            },
            CreatedOn = v.CreatedOn,
            CreatedBy = v.CreatedBy,
            UpdatedOn = v.UpdatedOn,
            UpdatedBy = v.UpdatedBy
        }).ToList();
    }

    private PostDto MapPost(Entities.Domain.Post post)
    {
        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            Category = post.Category,
            CreatedBy = post.CreatedBy,
            CreatedOn = post.CreatedOn,
            UpvoteCount = post.UpvoteCount,
            DownvoteCount = post.DownvoteCount,
            ShareCount = post.ShareCount,
            Comments = MapComments(post.Comments),
            Votes = MapVotes(post.Votes),
            Shares = post.Shares,
            Reports = post.Reports,
            PostImages = post.PostImages ?? new List<PostImages>(),
            Username = post.User != null ? post.User.UserName : string.Empty,
            UserAvatar = post.User != null ? post.User.AvatarId : string.Empty
        };
    }

    public async Task<List<PostDto>> GetAllPostsAsync()
    {
        var posts = await _context.Posts
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.ChildComments)
                    .ThenInclude(cc => cc.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.Votes)
                    .ThenInclude(v => v.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.ChildComments)
                    .ThenInclude(cc => cc.Votes)
                        .ThenInclude(v => v.User)
            .Include(p => p.Votes)
                .ThenInclude(v => v.User)
            .Include(p => p.Shares)
            .Include(p => p.Reports)
            .Include(p => p.PostImages)
            .Include(p => p.User)
            .ToListAsync();

        return posts.Select(MapPost).ToList();
    }

    public async Task<PostDto> GetPostByIdAsync(Guid postId)
    {
        var post = await _context.Posts
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.ChildComments)
                    .ThenInclude(cc => cc.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.Votes)
                    .ThenInclude(v => v.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.ChildComments)
                    .ThenInclude(cc => cc.Votes)
                        .ThenInclude(v => v.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.ChildComments)
                    .ThenInclude(cc => cc.ChildComments)
                        .ThenInclude(ccc => ccc.Votes)
                            .ThenInclude(v => v.User)
            .Include(p => p.Votes)
                .ThenInclude(v => v.User)
            .Include(p => p.Shares)
            .Include(p => p.Reports)
            .Include(p => p.PostImages)
            .Include(p => p.User)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (post == null) throw new NotFoundException("Post not found");

        return MapPost(post);
    }

    public async Task<List<PostDto>> GetAllPostByUserId(Guid getPostByUserId)
    {
        var posts = await _context.Posts
            .Where(post => post.CreatedBy == getPostByUserId)
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.ChildComments)
                    .ThenInclude(cc => cc.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.Votes)
                    .ThenInclude(v => v.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.ChildComments)
                    .ThenInclude(cc => cc.Votes)
                        .ThenInclude(v => v.User)
            .Include(p => p.Votes)
                .ThenInclude(v => v.User)
            .Include(p => p.Shares)
            .Include(p => p.Reports)
            .Include(p => p.PostImages)
            .Include(p => p.User)
            .AsNoTracking()
            .ToListAsync();

        return posts.Select(MapPost).ToList();
    }

    public async Task<List<PostDto>> GetPostsByUpDownVoteAsync(Guid userId, string getBy)
    {
        List<Entities.Domain.Post> posts;

        if (getBy == "upvote")
        {
            posts = await _context.Posts
                .Where(p => p.CreatedBy == userId && p.UpvoteCount >= p.DownvoteCount)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .Include(p => p.Votes)
                .Include(p => p.Shares)
                .Include(p => p.Reports)
                .Include(p => p.PostImages)
                .Include(p => p.User)
                .ToListAsync();
        }
        else if (getBy == "downvote")
        {
            posts = await _context.Posts
                .Where(p => p.CreatedBy == userId && p.DownvoteCount > p.UpvoteCount)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .Include(p => p.Votes)
                .Include(p => p.Shares)
                .Include(p => p.Reports)
                .Include(p => p.PostImages)
                .Include(p => p.User)
                .ToListAsync();
        }
        else
        {
            throw new ArgumentException("Giá trị getBy không hợp lệ. Chỉ chấp nhận 'upvote' hoặc 'downvote'.");
        }

        return posts.Select(MapPost).ToList();
    }

    public async Task<Entities.Domain.Post> CreatePostAsync(CreatePostDto createPostDto, Guid userId)
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

            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");

            // Verify user exists before creating post
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new NotFoundException($"User with ID {userId} not found");

            // Ánh xạ từ CreatePostDto sang Post
            var post = _mapper.Map<CollabSphere.Entities.Domain.Post>(createPostDto)
                    ?? throw new InvalidOperationException("Mapping failed.");

            // Cập nhật các trường khác cho Post
            post.Id = Guid.NewGuid();
            post.CreatedOn = DateTime.UtcNow;
            post.UpvoteCount = 0;
            post.DownvoteCount = 0;
            post.ShareCount = 0;
            post.CreatedBy = userId;
            post.UserId = userId; // Ensure UserId is explicitly set to match CreatedBy

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

    public async Task<PostResponseModel> UpdatePostAsync(Guid id, UpdatePostDto model, Guid updatedByUserId)
    {
        var post = await _context.Posts
            .Include(p => p.PostImages) // Load ảnh hiện tại
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            throw new KeyNotFoundException("Bài post không tồn tại");

        var updatedUser = await _context.Users.FindAsync(updatedByUserId);
        if (updatedUser == null)
            throw new KeyNotFoundException("Người dùng không tồn tại");

        // Cập nhật title, content, audit info
        post.Title = model.Title;
        post.Content = model.Content;
        post.UpdatedOn = DateTime.UtcNow;
        post.UpdatedBy = updatedByUserId;

        // Xử lý ảnh: xoá ảnh được đánh dấu
        if (model.PostImages != null && model.PostImages.Any())
        {
            foreach (var imgDto in model.PostImages)
            {
                if (imgDto.Id != null && imgDto.IsDeleted)
                {
                    var existingImage = post.PostImages.FirstOrDefault(x => x.Id == imgDto.Id.Value);
                    if (existingImage != null)
                    {
                        _context.PostImages.Remove(existingImage);
                    }
                }

                if (imgDto.Id == null && !imgDto.IsDeleted)
                {
                    var newImage = new PostImages
                    {
                        Id = Guid.NewGuid(),
                        ImageID = imgDto.ImageID,
                        PostId = post.Id
                    };
                    post.PostImages.Add(newImage);
                }
            }
        }

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
        try
        {
            var post = await _context.Posts
                .Include(p => p.Comments)
                .Include(p => p.Votes)
                .Include(p => p.Shares)
                .Include(p => p.Reports)
                .Include(p => p.PostImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
                return false;

            // Kiểm tra quyền xóa
            if (post.CreatedBy != deletedByUserId)
            {
                // Kiểm tra xem người dùng có phải là admin không
                var userRoles = await _context.UserRoles
                    .Where(ur => ur.UserId == deletedByUserId)
                    .Select(ur => ur.RoleId)
                    .ToListAsync();

                var adminRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Name == "Admin");

                if (adminRole == null || !userRoles.Contains(adminRole.Id))
                    return false;
            }

            // Xóa các dữ liệu liên quan
            _context.Comments.RemoveRange(post.Comments);
            _context.Votes.RemoveRange(post.Votes);
            _context.Shares.RemoveRange(post.Shares);
            _context.Reports.RemoveRange(post.Reports);
            _context.PostImages.RemoveRange(post.PostImages);

            // Xóa post
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting post: {ex.Message}");
            return false;
        }
    }

    public async Task<PaginationResponse<PostDto>> GetPaginatedPostsByUserId(Guid userId, PaginationRequest request)
    {
        var skip = (request.Page - 1) * request.PageSize;
        var spec = PostSpecification.GetPaginatedPostsByUserId(userId, skip, request.PageSize);
        var countSpec = PostSpecification.GetPostCountByUserId(userId);

        var posts = await _postRepository.GetAllAsync(spec);
        var total = await _postRepository.CountAsync(countSpec);

        var postDtos = posts.Select(MapPost).ToList();

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

    public async Task<List<PostDto>> GetHomePostsAsync()
    {
        var posts = await _context.Posts
            .OrderByDescending(p => p.CreatedOn)
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.ChildComments)
                    .ThenInclude(cc => cc.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.Votes)
                    .ThenInclude(v => v.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.ChildComments)
                    .ThenInclude(cc => cc.Votes)
                        .ThenInclude(v => v.User)
            .Include(p => p.Votes)
                .ThenInclude(v => v.User)
            .Include(p => p.Shares)
            .Include(p => p.Reports)
            .Include(p => p.PostImages)
            .Include(p => p.User)
            .ToListAsync();

        return posts.Select(MapPost).ToList();
    }

    public async Task<PaginationResponse<PostDto>> GetPopularPostsAsync(int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Posts
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .Include(p => p.Votes)
            .Include(p => p.Shares)
            .Include(p => p.Reports)
            .Include(p => p.PostImages)
            .Include(p => p.User)
            .AsNoTracking()
            .Select(p => new
            {
                Post = p,
                PopularityScore = p.UpvoteCount + p.Comments.Count
            })
            .OrderByDescending(x => x.PopularityScore)
            .Select(x => x.Post);

        var total = await query.CountAsync();
        var posts = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalPages = (int) Math.Ceiling(total / (double) pageSize);

        return new PaginationResponse<PostDto>(
            pageNumber,
            totalPages,
            pageSize,
            total,
            posts.Select(MapPost).ToList()
        );
    }

    public async Task<PaginationResponse<PostDto>> GetRecentPostsFromFollowedUsersAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
    {
        var followedUserIds = await _context.Follows
            .Where(uf => uf.FollowerId == userId)
            .Select(uf => uf.FollowingId)
            .ToListAsync();

        var query = _context.Posts
            .Where(p => followedUserIds.Contains(p.CreatedBy))
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .Include(p => p.Votes)
            .Include(p => p.Shares)
            .Include(p => p.Reports)
            .Include(p => p.PostImages)
            .Include(p => p.User)
            .OrderByDescending(p => p.CreatedOn);

        var total = await query.CountAsync();
        var posts = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalPages = (int) Math.Ceiling(total / (double) pageSize);

        return new PaginationResponse<PostDto>(
            pageNumber,
            totalPages,
            pageSize,
            total,
            posts.Select(MapPost).ToList()
        );
    }

    public async Task<List<PostDto>> SearchPostsAsync(string searchTerm, int pageNumber = 1, int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetHomePostsAsync();
        }

        var lowerSearchTerm = searchTerm.ToLower();

        var posts = await _context.Posts
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .Include(p => p.Votes)
            .Include(p => p.Shares)
            .Include(p => p.Reports)
            .Include(p => p.PostImages)
            .Include(p => p.User)
            .Where(p =>
                p.Title.ToLower().Contains(lowerSearchTerm) ||
                p.Content.ToLower().Contains(lowerSearchTerm) ||
                p.Category.ToLower().Contains(lowerSearchTerm) ||
                p.User.UserName.ToLower().Contains(lowerSearchTerm)
            )
            .OrderByDescending(p => p.CreatedOn)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return posts.Select(MapPost).ToList();
    }

    public async Task<List<PostDto>> GetUserVotedPostsAsync(Guid userId, string voteType)
    {
        if (string.IsNullOrEmpty(voteType) || (voteType.ToLower() != "upvote" && voteType.ToLower() != "downvote"))
            throw new ArgumentException("Giá trị voteType không hợp lệ. Chỉ chấp nhận 'upvote' hoặc 'downvote'.");

        var normalizedVoteType = voteType.ToLower() == "upvote" ? "upvote" : "downvote";
        var capitalizedVoteType = char.ToUpper(normalizedVoteType[0]) + normalizedVoteType.Substring(1);

        Console.WriteLine($"Tìm bài viết với userId={userId} và voteType={normalizedVoteType} hoặc {capitalizedVoteType}");

        var posts = await _context.Posts
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .Include(p => p.Votes)
            .Include(p => p.Shares)
            .Include(p => p.Reports)
            .Include(p => p.PostImages)
            .Include(p => p.User)
            .Where(p => p.Votes.Any(v => v.UserId == userId &&
                                     (v.VoteType.ToLower() == normalizedVoteType ||
                                      v.VoteType == capitalizedVoteType)))
            .OrderByDescending(p => p.CreatedOn)
            .ToListAsync();

        Console.WriteLine($"Đã tìm thấy {posts.Count} bài viết");
        return posts.Select(MapPost).ToList();
    }
}

