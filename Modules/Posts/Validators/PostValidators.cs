using System;
using System.Text.RegularExpressions;

using CollabSphere.Modules.Posts.Models;

using FluentValidation;

namespace CollabSphere.Modules.Posts.Validators
{
    public class PostValidators : AbstractValidator<CreatePostDto>
    {
        public PostValidators()
        {
            RuleFor(post => post.Title)
                .NotEmpty().WithMessage("Tiêu đề không được để trống.")
                .MaximumLength(200).WithMessage("Tiêu đề không được vượt quá 200 ký tự.");

            RuleFor(post => post.Content)
                .NotEmpty().WithMessage("Nội dung không được để trống.")
                .MinimumLength(10).WithMessage("Nội dung phải có ít nhất 10 ký tự.");

        }

        private bool BeAValidImageUrl(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                return false;
            }

            string pattern = @"\.(jpg|jpeg|png|gif)$"; // Regex kiểm tra đuôi file
            return Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase);
        }

        private bool BeValidSubredditId(Guid? subredditId)
        {
            return !subredditId.HasValue || subredditId.Value != Guid.Empty;
        }
    }
}
