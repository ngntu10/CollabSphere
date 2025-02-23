using CollabSphere.Modules.TodoList.Models;

using FluentValidation;

namespace CollabSphere.Modules.TodoList.Validators;

public class UpdateTodoListModelValidator : AbstractValidator<UpdateTodoListModel>
{
    public UpdateTodoListModelValidator()
    {
        RuleFor(ctl => ctl.Title)
            .MinimumLength(TodoListValidatorConfiguration.MinimumTitleLength)
            .WithMessage(
                $"Todo list title must contain a minimum of {TodoListValidatorConfiguration.MinimumTitleLength} characters")
            .MaximumLength(TodoListValidatorConfiguration.MaximumTitleLength)
            .WithMessage(
                $"Todo list title must contain a minimum of {TodoListValidatorConfiguration.MaximumTitleLength} characters");
    }
}
