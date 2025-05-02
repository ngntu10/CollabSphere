using FluentValidation;
using CollabSphere.Modules.User.Models;
using System.Text.RegularExpressions;

namespace CollabSphere.Modules.User.Validators
{
    public class CreateUserValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên không được để trống")
                .MaximumLength(30).WithMessage("Tên không được vượt quá 30 ký tự");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Giới tính không được để trống");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Email không đúng định dạng");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Số điện thoại không được để trống")
                .Length(10).WithMessage("Số điện thoại phải 10 ký tự")
                .Matches(new Regex(@"^(\+\d{1,3})?[-.\s]?\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$"))
                .WithMessage("Số điện thoại không đúng định dạng");
        }
    }

    public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên không được để trống")
                .MaximumLength(30).WithMessage("Tên không được vượt quá 30 ký tự");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Giới tính không được để trống");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Email không đúng định dạng");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Số điện thoại không được để trống")
                .Length(10).WithMessage("Số điện thoại phải 10 ký tự")
                .Matches(new Regex(@"^(\+\d{1,3})?[-.\s]?\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}$"))
                .WithMessage("Số điện thoại không đúng định dạng");
        }
    }
} 