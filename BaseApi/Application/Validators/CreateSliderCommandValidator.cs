using BaseApi.Application.Features.Sliders.Commands.CreateSlider;
using FluentValidation;

namespace BaseApi.Application.Validators
{
    public class CreateSliderCommandValidator : AbstractValidator<CreateSliderCommand>
    {
        public CreateSliderCommandValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

            RuleFor(x => x.Subtitle)
                .MaximumLength(500).WithMessage("Subtitle must not exceed 500 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("Image URL is required")
                .MaximumLength(1000).WithMessage("Image URL must not exceed 1000 characters");

            RuleFor(x => x.MobileImageUrl)
                .MaximumLength(1000).WithMessage("Mobile image URL must not exceed 1000 characters");

            RuleFor(x => x.LinkUrl)
                .MaximumLength(500).WithMessage("Link URL must not exceed 500 characters");

            RuleFor(x => x.ButtonText)
                .MaximumLength(100).WithMessage("Button text must not exceed 100 characters");

            RuleFor(x => x.TargetLocation)
                .MaximumLength(100).WithMessage("Target location must not exceed 100 characters");

            RuleFor(x => x.Order)
                .GreaterThanOrEqualTo(0).WithMessage("Order must be greater than or equal to 0");

            RuleFor(x => x.SliderType)
                .IsInEnum().WithMessage("Invalid slider type");

            RuleFor(x => x.LinkType)
                .IsInEnum().WithMessage("Invalid link type");

            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("Start date must be before end date");

            RuleFor(x => x.CreatedBy)
                .GreaterThan(0).WithMessage("Created by must be greater than 0");
        }
    }
}