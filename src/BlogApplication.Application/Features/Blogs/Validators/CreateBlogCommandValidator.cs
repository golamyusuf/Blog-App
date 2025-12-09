using FluentValidation;

namespace BlogApplication.Application.Features.Blogs.Validators;

public class CreateBlogCommandValidator : AbstractValidator<Commands.CreateBlogCommand>
{
    public CreateBlogCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MinimumLength(50).WithMessage("Content must be at least 50 characters");

        RuleFor(x => x.Summary)
            .MaximumLength(500).WithMessage("Summary cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Summary));

        RuleFor(x => x.Tags)
            .Must(tags => tags.Count <= 10).WithMessage("Maximum 10 tags allowed");
    }
}
