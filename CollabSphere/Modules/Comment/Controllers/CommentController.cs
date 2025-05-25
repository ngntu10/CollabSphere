using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CollabSphere.Common;
using CollabSphere.Database;
using CollabSphere.Exceptions;
using CollabSphere.Modules.Comment.Models;
using CollabSphere.Modules.Comment.Services;
using CollabSphere.Modules.Comment.Validators;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollabSphere.Modules.Comment.Controllers;

[ApiController]
[Route("api/comments")]
[Authorize]
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
        // Validate request
        var validationResult = await this.ValidateCreateCommentRequestAsync(createCommentRequest);
        if (validationResult != null)
            return validationResult;

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
        // Validate id
        var validationResult = await this.ValidateCommentIdAsync(id);
        if (validationResult != null)
            return validationResult;

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

    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetCommentsByPostId(Guid postId)
    {
        // Validate postId
        var validationResult = await this.ValidateCommentIdAsync(postId);
        if (validationResult != null)
            return validationResult;

        try
        {
            var comments = await _commentService.GetCommentsByPostIdAsync(postId);
            return Ok(ApiResponse<List<CommentResponse>>.Success(
                StatusCodes.Status200OK,
                comments,
                "Lấy danh sách bình luận theo bài viết thành công"
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
                new List<string> { "Đã xảy ra lỗi trong quá trình lấy danh sách bình luận." }
            ));
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        // Validate id
        var validationResult = await this.ValidateCommentIdAsync(id);
        if (validationResult != null)
            return validationResult;

        try
        {
            var userId = User.GetUserId();
            var result = await _commentService.DeleteCommentAsync(id, userId);

            return Ok(ApiResponse<bool>.Success(
                StatusCodes.Status200OK,
                result,
                "Xóa bình luận thành công"
            ));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Failure(
                StatusCodes.Status404NotFound,
                new List<string> { ex.Message }
            ));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<object>.Failure(
                StatusCodes.Status401Unauthorized,
                new List<string> { ex.Message }
            ));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Failure(
                StatusCodes.Status500InternalServerError,
                new List<string> { "Đã xảy ra lỗi trong quá trình xóa bình luận." }
            ));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentRequest request)
    {
        // Validate id
        var idValidationResult = await this.ValidateCommentIdAsync(id);
        if (idValidationResult != null)
            return idValidationResult;

        // Validate request
        var requestValidationResult = await this.ValidateUpdateCommentRequestAsync(request);
        if (requestValidationResult != null)
            return requestValidationResult;

        try
        {
            var userId = User.GetUserId();
            var updatedComment = await _commentService.UpdateCommentAsync(id, request, userId);

            return Ok(ApiResponse<CommentResponse>.Success(
                StatusCodes.Status200OK,
                updatedComment,
                "Cập nhật bình luận thành công"
            ));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Failure(
                StatusCodes.Status404NotFound,
                new List<string> { ex.Message }
            ));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<object>.Failure(
                StatusCodes.Status401Unauthorized,
                new List<string> { ex.Message }
            ));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.Failure(
                StatusCodes.Status500InternalServerError,
                new List<string> { "Đã xảy ra lỗi trong quá trình cập nhật bình luận." }
            ));
        }
    }

    [HttpPost("{id}/vote")]
    public async Task<IActionResult> VoteComment(Guid id, [FromBody] VoteCommentRequest request)
    {
        var idValidationResult = await this.ValidateCommentIdAsync(id);
        if (idValidationResult != null)
            return idValidationResult;

        if (request == null)
        {
            return BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                new List<string> { "Dữ liệu vote không hợp lệ" }
            ));
        }

        try
        {
            var userId = User.GetUserId();
            bool result = await _commentService.VoteCommentAsync(id, userId, request.VoteType);
            string message = result
                ? $"Vote {request.VoteType.ToString().ToLower()} thành công"
                : "Bạn đã bỏ vote thành công.";

            return Ok(ApiResponse<object>.Success(
                StatusCodes.Status200OK,
                result,
                message
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
                new List<string> { "Đã xảy ra lỗi trong quá trình vote bình luận." }
            ));
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetCommentsByUserId(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                new List<string> { "UserId không hợp lệ" }
            ));
        }

        try
        {
            var comments = await _commentService.GetCommentsByUserIdAsync(userId);

            return Ok(ApiResponse<List<CommentResponse>>.Success(
                StatusCodes.Status200OK,
                comments,
                $"Lấy danh sách bình luận của người dùng {userId} thành công"
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
                new List<string> { "Đã xảy ra lỗi trong quá trình lấy danh sách bình luận." }
            ));
        }
    }

    [HttpGet("user/{userId}/voted/{voteType}")]
    public async Task<IActionResult> GetUserVotedComments(Guid userId, string voteType)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                new List<string> { "UserId không hợp lệ" }
            ));
        }

        if (voteType != "upvote" && voteType != "downvote")
        {
            return BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                new List<string> { "Giá trị voteType không hợp lệ. Chỉ chấp nhận 'upvote' hoặc 'downvote'." }
            ));
        }

        try
        {
            var comments = await _commentService.GetUserVotedCommentsAsync(userId, voteType);

            return Ok(ApiResponse<List<CommentResponse>>.Success(
                StatusCodes.Status200OK,
                comments,
                $"Lấy danh sách bình luận đã {voteType} của người dùng {userId} thành công"
            ));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<object>.Failure(
                StatusCodes.Status404NotFound,
                new List<string> { ex.Message }
            ));
        }
        catch (ArgumentException ex)
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
                new List<string> { "Đã xảy ra lỗi trong quá trình lấy danh sách bình luận." }
            ));
        }
    }
}
