using System;

using FluentValidation;

namespace CollabSphere.Modules.Comment.Validators;

public class GuidParameterValidator : AbstractValidator<Guid>
{
    public GuidParameterValidator()
    {
        RuleFor(id => id)
            .NotEqual(Guid.Empty).WithMessage("ID không hợp lệ.");
    }
}
