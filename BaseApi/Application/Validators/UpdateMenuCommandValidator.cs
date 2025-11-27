using BaseApi.Application.Features.Menus.Commands.UpdateMenu;
using FluentValidation;

namespace BaseApi.Application.Validators
{
    public class UpdateMenuCommandValidator : AbstractValidator<UpdateMenuCommand>
    {
        public UpdateMenuCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Menu ID must be greater than 0");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

            RuleFor(x => x.Slug)
                .MaximumLength(150).WithMessage("Slug must not exceed 150 characters")
                .Matches("^[a-z0-9-]*$").WithMessage("Slug can only contain lowercase letters, numbers and hyphens")
                .When(x => !string.IsNullOrEmpty(x.Slug));

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

            RuleFor(x => x.Url)
                .MaximumLength(500).WithMessage("URL must not exceed 500 characters");

            RuleFor(x => x.Icon)
                .MaximumLength(50).WithMessage("Icon must not exceed 50 characters");

            RuleFor(x => x.ImageUrl)
                .MaximumLength(500).WithMessage("Image URL must not exceed 500 characters");

            RuleFor(x => x.Order)
                .GreaterThanOrEqualTo(0).WithMessage("Order must be greater than or equal to 0");

            RuleFor(x => x.MenuType)
                .IsInEnum().WithMessage("Invalid menu type");

            RuleFor(x => x.LinkType)
                .IsInEnum().WithMessage("Invalid link type");
        }
    }
}