using System;
using System.Threading.Tasks;

using CollabSphere.Common;
using CollabSphere.Modules.Comment.Models;

using FluentValidation;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace CollabSphere.Modules.Comment.Validators;

public static class CommentValidationExtensions
{
    /// <summary>
    /// Validate một CommentId từ route parameter
    /// </summary>
    public static async Task<IActionResult> ValidateCommentIdAsync(this ControllerBase controller, Guid commentId)
    {
        var validator = controller.HttpContext.RequestServices.GetRequiredService<GuidParameterValidator>();
        var validationResult = await validator.ValidateAsync(commentId);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return controller.BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                errors
            ));
        }

        return null; // Validation thành công, tiếp tục request
    }

    /// <summary>
    /// Validate một CreateCommentRequest
    /// </summary>
    public static async Task<IActionResult> ValidateCreateCommentRequestAsync(this ControllerBase controller, CreateCommentRequest request)
    {
        var validator = controller.HttpContext.RequestServices.GetRequiredService<CreateCommentRequestValidator>();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return controller.BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                errors
            ));
        }

        return null; // Validation thành công, tiếp tục request
    }

    /// <summary>
    /// Validate một UpdateCommentRequest
    /// </summary>
    public static async Task<IActionResult> ValidateUpdateCommentRequestAsync(this ControllerBase controller, UpdateCommentRequest request)
    {
        var validator = controller.HttpContext.RequestServices.GetRequiredService<UpdateCommentRequestValidator>();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return controller.BadRequest(ApiResponse<object>.Failure(
                StatusCodes.Status400BadRequest,
                errors
            ));
        }

        return null; // Validation thành công, tiếp tục request
    }
}
