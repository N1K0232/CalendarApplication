using CalendarApplication.Shared.Models.Requests;
using FluentValidation;

namespace CalendarApplication.BusinessLayer.Validations;

public class SaveTodoRequestValidator : AbstractValidator<SaveTodoRequest>
{
    public SaveTodoRequestValidator()
    {
        RuleFor(t => t.Name)
            .MaximumLength(256)
            .WithMessage("the maximum length is 256 characters")
            .NotEmpty()
            .WithMessage("the name is required");

        RuleFor(t => t.Description)
            .MaximumLength(2048)
            .WithMessage("the maximum length is 2048 characters");
    }
}