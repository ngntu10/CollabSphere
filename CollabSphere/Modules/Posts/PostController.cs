using CollabSphere.Common;
using CollabSphere.Database;
using CollabSphere.Entities.Domain;
using CollabSphere.Exceptions;
using CollabSphere.Modules.Post.Models;
using CollabSphere.Modules.Posts.Models;
using CollabSphere.Modules.Posts.Service;

using Microsoft.AspNetCore.Mvc;
namespace CollabSphere.Modules.Posts.Controllers;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/posts")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly DatabaseContext _context;

    public PostController(IPostService postService, DatabaseContext context)
    {
        _postService = postService;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto createPostDto)
    {
        if (createPostDto == null)
        {
            return BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                new List<string> { "Dữ liệu bài post không hợp lệ" }
            ));
        }

        // Tạo bài post thông qua service
        var post = await _postService.CreatePostAsync(createPostDto);
        if (post == null)
        {
            return BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                new List<string> { "Không thể tạo bài post" }
            ));
        }

        // Tạo response
        var response = new CreatePostModel
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            PostImages = post.PostImages?.Select(x => x.ImageID ?? "").ToList() // Xử lý null cho ImageID
        };

        return Ok(ApiResponse<CreatePostModel>.Success(
            StatusCodes.Status200OK,
            response,
            "Đăng bài thành công"
        ));
    }

    [HttpGet("user/{userId}/{getBy}")]
    public async Task<IActionResult> GetPostsByUpDownVote(Guid userId, string getBy)
    {
        if (getBy != "upvote" && getBy != "downvote")
        {
            return BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                new List<string> { "Giá trị getBy không hợp lệ. Chỉ chấp nhận 'upvote' hoặc 'downvote'." }
            ));
        }
        var posts = await _postService.GetPostsByUpDownVoteAsync(userId, getBy);
        return Ok(ApiResponse<List<PostDto>>.Success(
            StatusCodes.Status200OK, posts, "Lấy danh sách bài post thành công"
        ));
    }

    [HttpPut("{id}/update")]
    public async Task<IActionResult> UpdatePost(Guid id, [FromBody] UpdatePostModel model)
    {
        if (model == null)
        {
            return BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                new List<string> { "Dữ liệu cập nhật không hợp lệ" }
            ));
        }

        var updatedByUserId = User.GetUserId();

        var updatedPost = await _postService.UpdatePostAsync(id, model, updatedByUserId);

        return Ok(ApiResponse<PostResponseModel>.Success(
            StatusCodes.Status200OK,
            updatedPost,
            $"Người dùng {updatedPost.UpdatedByUsername} đã cập nhật bài post thành công ngày {updatedPost.UpdatedOn:dd/MM/yyyy HH:mm:ss}"
        ));
    }
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        var deletedByUserId = User.GetUserId();

        var isDeleted = await _postService.DeletePostAsync(id, deletedByUserId);

        if (!isDeleted)
        {
            return NotFound(ApiResponse<object>.Failure(
                StatusCodes.Status404NotFound,
                new List<string> { "Bài post không tồn tại hoặc bạn không có quyền xóa" }
            ));
        }

        return Ok(ApiResponse<object>.Success(
            StatusCodes.Status200OK,
            null,
            $"Người dùng {deletedByUserId} đã xóa bài post thành công"
        ));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(Guid id)
    {
        try
        {
            var post = await _postService.GetPostByIdAsync(id);
            return Ok(ApiResponse<PostDto>.Success(
                StatusCodes.Status200OK,
                post,
                "Lấy thông tin bài post thành công"
            ));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Failure(
                StatusCodes.Status404NotFound,
                new List<string> { ex.Message }
            ));
        }
    }

    [HttpPost("{id}/vote")]
    public async Task<IActionResult> VotePost(Guid id, [FromBody] VoteRequest request)
    {
        if (request == null || request.UserId == Guid.Empty)
        {
            Console.WriteLine("Lỗi: Request không hợp lệ hoặc UserId trống");
            return BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                new List<string> { "Dữ liệu vote không hợp lệ hoặc UserId trống" }
            ));
        }

        if (_context == null)
        {
            Console.WriteLine("Lỗi: _context bị null!");
            return StatusCode(500, "Lỗi hệ thống: Database context không tồn tại");
        }

        if (_context.Users == null)
        {
            Console.WriteLine("Lỗi: Bảng Users trong DbContext bị null!");
            return StatusCode(500, "Lỗi hệ thống: Không thể truy vấn người dùng");
        }

        var userId = request.UserId;

        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            Console.WriteLine($"Lỗi: Người dùng {userId} không tồn tại");
            return NotFound(ApiResponse<object>.Failure(
                StatusCodes.Status404NotFound,
                new List<string> { "Người dùng không tồn tại" }
            ));
        }

        bool result = false;
        string message = "";

        try
        {
            result = await _postService.VotePostAsync(id, userId, request.VoteType);
            message = result
                ? $"Vote {request.VoteType.ToString().ToLower()} thành công"
                : "Bạn đã bỏ vote thành công.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi hệ thống: {ex.Message}");
            return StatusCode(500, "Lỗi hệ thống");
        }

        return Ok(ApiResponse<object>.Success(
            StatusCodes.Status200OK,
            result,
            message
        ));
    }



    [HttpGet("user/{userId}/paginated")]
    public async Task<IActionResult> GetPaginatedPostsByUserId(
        Guid userId,
        [FromQuery] PaginationRequest request)
    {
        var paginatedPosts = await _postService.GetPaginatedPostsByUserId(userId, request);

        return Ok(ApiResponse<PaginationResponse<PostDto>>.Success(
            StatusCodes.Status200OK,
            paginatedPosts,
            $"Lấy danh sách bài post của người dùng {userId} thành công"
        ));
    }
}
