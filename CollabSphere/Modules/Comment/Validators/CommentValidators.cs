using System;

using CollabSphere.Modules.Comment.Models;
using CollabSphere.Modules.Comment.Validators;

using FluentValidation;

namespace CollabSphere.Modules.Comment.Validators;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Nội dung bình luận không được để trống.")
            .MinimumLength(CommentValidatorConfiguration.MinimumContentLength)
                .WithMessage($"Nội dung bình luận phải có ít nhất {CommentValidatorConfiguration.MinimumContentLength} ký tự.")
            .MaximumLength(CommentValidatorConfiguration.MaximumContentLength)
                .WithMessage($"Nội dung bình luận không được vượt quá {CommentValidatorConfiguration.MaximumContentLength} ký tự.");

        RuleFor(x => x.PostId)
            .NotEmpty().WithMessage("ID của bài viết không được để trống.")
            .NotEqual(Guid.Empty).WithMessage("ID của bài viết không hợp lệ.");

        // ParentCommentId có thể null (comment gốc) hoặc là một Guid hợp lệ khác Empty (trả lời comment)
        When(x => x.ParentCommentId.HasValue, () =>
        {
            RuleFor(x => x.ParentCommentId.Value)
                .NotEqual(Guid.Empty).WithMessage("ID của bình luận cha không hợp lệ.");
        });
    }
}

public class UpdateCommentRequestValidator : AbstractValidator<UpdateCommentRequest>
{
    public UpdateCommentRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Nội dung bình luận không được để trống.")
            .MinimumLength(CommentValidatorConfiguration.MinimumContentLength)
                .WithMessage($"Nội dung bình luận phải có ít nhất {CommentValidatorConfiguration.MinimumContentLength} ký tự.")
            .MaximumLength(CommentValidatorConfiguration.MaximumContentLength)
                .WithMessage($"Nội dung bình luận không được vượt quá {CommentValidatorConfiguration.MaximumContentLength} ký tự.");
    }
}
