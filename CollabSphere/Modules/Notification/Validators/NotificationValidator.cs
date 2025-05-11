using System.Text.RegularExpressions;

using CollabSphere.Modules.Notification.Models;

using FluentValidation;

namespace CollabSphere.Modules.Notification.Validators
{
    // public class CreateNotificationValidator : AbstractValidator<CreateNotificationDto>
    // {
    //     public CreateNotificationValidator()
    //     {
    //         RuleFor(x => x.NotificationName)
    //             .NotEmpty().WithMessage("Tên không được để trống")
    //             .MaximumLength(30).WithMessage("Tên không được vượt quá 30 ký tự");

    //         RuleFor(x => x.Gender)
    //             .NotEmpty().WithMessage("Giới tính không được để trống");

    //         RuleFor(x => x.Email)
    //             .NotEmpty().WithMessage("Email không được để trống")
    //             .EmailAddress().WithMessage("Email không đúng định dạng");

    //         RuleFor(x => x.PhoneNumber)
    //             .NotEmpty().WithMessage("Số điện thoại không được để trống")
    //             .Length(10).WithMessage("Số điện thoại phải 10 ký tự")
    //             .Matches(new Regex(@"^(\+\d{1,3})?[-.\s]?\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$"))
    //             .WithMessage("Số điện thoại không đúng định dạng");
    //     }
    // }

    public class UpdateNotificationValidator : AbstractValidator<UpdateNotificationDto>
    {
        public UpdateNotificationValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("Id người dùng không được để trống");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Nội dung không được để trống")
                .MaximumLength(500).WithMessage("Nội dung không được vượt quá 500 ký tự");

            RuleFor(x => x.Link)
                .NotEmpty()
                .Matches(new Regex(@"^https?:\/\/[^\s]+$"))
                .WithMessage("Link không đúng định dạng");

            RuleFor(x => x.IsRead)
                .NotEmpty().WithMessage("Trạng thái đọc không được để trống");

            RuleFor(x => x.NotificationType)
                .NotEmpty().WithMessage("Loại thông báo không được để trống");
            // .Matches(new Regex(@"^[a-zA-Z0-9]+$"))
            // .WithMessage("Loại thông báo không đúng định dạng");
        }
    }
}
