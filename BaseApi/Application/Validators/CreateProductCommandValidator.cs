using BaseApi.Application.Features.Products.Commands.CreateProduct;
using FluentValidation;

namespace BaseApi.Application.Validators
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(200).WithMessage("Product name must not exceed 200 characters");

            RuleFor(x => x.Slug)
                .MaximumLength(300).WithMessage("Slug must not exceed 300 characters")
                .Matches("^[a-z0-9-]*$").WithMessage("Slug can only contain lowercase letters, numbers and hyphens")
                .When(x => !string.IsNullOrEmpty(x.Slug));

            RuleFor(x => x.ShortDescription)
                .MaximumLength(500).WithMessage("Short description must not exceed 500 characters");

            RuleFor(x => x.Description)
                .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters");

            RuleFor(x => x.ProductCode)
                .MaximumLength(100).WithMessage("Product code must not exceed 100 characters");

            RuleFor(x => x.MainImageUrl)
                .MaximumLength(1000).WithMessage("Main image URL must not exceed 1000 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0")
                .When(x => x.Price.HasValue);

            RuleFor(x => x.DiscountPrice)
                .GreaterThan(0).WithMessage("Discount price must be greater than 0")
                .LessThan(x => x.Price).WithMessage("Discount price must be less than regular price")
                .When(x => x.DiscountPrice.HasValue && x.Price.HasValue);

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required")
                .Length(3).WithMessage("Currency must be 3 characters");

            RuleFor(x => x.Dimensions)
                .MaximumLength(1000).WithMessage("Dimensions must not exceed 1000 characters");

            RuleFor(x => x.Material)
                .MaximumLength(200).WithMessage("Material must not exceed 200 characters");

            RuleFor(x => x.Colors)
                .MaximumLength(200).WithMessage("Colors must not exceed 200 characters");

            RuleFor(x => x.MetaTitle)
                .MaximumLength(200).WithMessage("Meta title must not exceed 200 characters");

            RuleFor(x => x.MetaDescription)
                .MaximumLength(500).WithMessage("Meta description must not exceed 500 characters");

            RuleFor(x => x.Keywords)
                .MaximumLength(500).WithMessage("Keywords must not exceed 500 characters");

            RuleFor(x => x.Order)
                .GreaterThanOrEqualTo(0).WithMessage("Order must be greater than or equal to 0");

            RuleFor(x => x.Category)
                .IsInEnum().WithMessage("Invalid category");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status");

            RuleFor(x => x.CreatedBy)
                .GreaterThan(0).WithMessage("Created by must be greater than 0");
        }
    }
}