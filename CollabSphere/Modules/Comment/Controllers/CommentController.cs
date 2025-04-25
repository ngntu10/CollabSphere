using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CollabSphere.Common;
using CollabSphere.Exceptions;
using CollabSphere.Modules.Comment.Models;
using CollabSphere.Modules.Comment.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollabSphere.Modules.Comment.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest createCommentRequest)
    {
        if (createCommentRequest == null)
        {
            return BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                new List<string> { "Dữ liệu bình luận không hợp lệ" }
            ));
        }

        try
        {
            var userId = User.GetUserId(); // Lấy userId từ JWT claims
            var comment = await _commentService.CreateCommentAsync(createCommentRequest, userId);

            return Ok(ApiResponse<CommentResponse>.Success(
                StatusCodes.Status201Created,
                comment,
                "Bình luận đã được tạo thành công"
            ));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Failure(
                StatusCodes.Status404NotFound,
                new List<string> { ex.Message }
            ));
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                new List<string> { ex.Message }
            ));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Failure(
                StatusCodes.Status500InternalServerError,
                new List<string> { "Đã xảy ra lỗi trong quá trình tạo bình luận." }
            ));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommentById(Guid id)
    {
        try
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            return Ok(ApiResponse<CommentResponse>.Success(
                StatusCodes.Status200OK,
                comment,
                "Lấy thông tin bình luận thành công"
            ));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Failure(
                StatusCodes.Status404NotFound,
                new List<string> { ex.Message }
            ));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Failure(
                StatusCodes.Status500InternalServerError,
                new List<string> { "Đã xảy ra lỗi trong quá trình lấy thông tin bình luận." }
            ));
        }
    }
}
