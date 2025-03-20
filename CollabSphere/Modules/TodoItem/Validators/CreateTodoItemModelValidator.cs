using CollabSphere.Modules.TodoItem.Models;

using FluentValidation;

namespace CollabSphere.Modules.TodoItem.Validators;

public class CreateTodoItemModelValidator : AbstractValidator<CreateTodoItemModel>
{
    public CreateTodoItemModelValidator()
    {
        RuleFor(cti => cti.Title)
            .MinimumLength(TodoItemValidatorConfiguration.MinimumTitleLength)
            .WithMessage(
                $"Todo item title should have minimum {TodoItemValidatorConfiguration.MinimumTitleLength} characters")
            .MaximumLength(TodoItemValidatorConfiguration.MaximumTitleLength)
            .WithMessage(
                $"Todo item title should have maximum {TodoItemValidatorConfiguration.MaximumTitleLength} characters");

        RuleFor(cti => cti.Body)
            .MinimumLength(TodoItemValidatorConfiguration.MinimumBodyLength)
            .WithMessage(
                $"Todo item body should have minimum {TodoItemValidatorConfiguration.MinimumBodyLength} characters")
            .MaximumLength(TodoItemValidatorConfiguration.MaximumBodyLength)
            .WithMessage(
                $"Todo item body should have maximum {TodoItemValidatorConfiguration.MaximumBodyLength} characters");
    }
}
