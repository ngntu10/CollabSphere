using CollabSphere.Entities.Domain;
using CollabSphere.Modules.Auth.Models;

using FluentValidation;

using Microsoft.AspNetCore.Identity;

namespace CollabSphere.Modules.Auth.Validators;

public class CreateUserModelValidator : AbstractValidator<CreateUserModel>
{
    private readonly UserManager<User> _userManager;

    public CreateUserModelValidator(UserManager<User> userManager)
    {
        _userManager = userManager;

        RuleFor(u => u.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(UserValidatorConfiguration.MinimumUsernameLength)
            .WithMessage($"Username should have minimum {UserValidatorConfiguration.MinimumUsernameLength} characters")
            .MaximumLength(UserValidatorConfiguration.MaximumUsernameLength)
            .WithMessage($"Username should have maximum {UserValidatorConfiguration.MaximumUsernameLength} characters")
            .Must(UsernameIsUnique)
            .WithMessage("Username is not available");

        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email address is not valid")
            .Must(EmailAddressIsUnique)
            .WithMessage("Email address is already in use");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(UserValidatorConfiguration.MinimumPasswordLength)
            .WithMessage($"Password should have minimum {UserValidatorConfiguration.MinimumPasswordLength} characters")
            .MaximumLength(UserValidatorConfiguration.MaximumPasswordLength)
            .WithMessage($"Password should have maximum {UserValidatorConfiguration.MaximumPasswordLength} characters");

        RuleFor(u => u.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required")
            .Equal(u => u.Password).WithMessage("Passwords do not match");
    }

    private bool EmailAddressIsUnique(string email)
    {
        var user = _userManager.FindByEmailAsync(email).GetAwaiter().GetResult();
        return user == null;
    }

    private bool UsernameIsUnique(string username)
    {
        var user = _userManager.FindByNameAsync(username).GetAwaiter().GetResult();
        return user == null;
    }
}
