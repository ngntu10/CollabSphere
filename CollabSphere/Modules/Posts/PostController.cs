using CollabSphere.Common;
using CollabSphere.Modules.Post.Models;

using CollabSphere.Modules.Posts.Models;
using CollabSphere.Modules.Posts.Service;

using Microsoft.AspNetCore.Mvc;

namespace CollabSphere.Modules.Posts.Controllers;

[ApiController]
[Route("api/posts")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
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

        var post = await _postService.CreatePostAsync(createPostDto);

        var response = new CreatePostModel
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            ThumbnailUrl = post.ThumbnailUrl,
            SubredditId = post.SubredditId
        };

        return Ok(ApiResponse<CreatePostModel>.Success(
            StatusCodes.Status200OK,
            response,
            "Đăng bài thành công"
        ));
    }

    [HttpPut("{id}")]
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
    [HttpDelete("{id}")]
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
    [HttpGet("user/{userId}")]


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
