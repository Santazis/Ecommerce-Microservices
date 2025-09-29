using Catalog.Application.Models.Requests.Product;
using FluentValidation;

namespace Catalog.Application.Validators.Product;

public class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductValidator()
    {
        RuleFor(x=> x.Price).NotEmpty().WithMessage("Price is required").GreaterThan(0).WithMessage("Price must be greater than 0");
        RuleFor(x=> x.Name).NotEmpty().WithMessage("Name is required").MaximumLength(128).WithMessage("Name should not be more than 128 characters");
        RuleFor(x=> x.Description).MaximumLength(256).WithMessage("Description should not be more than 256 characters");
        RuleFor(x=> x.Currency).NotEmpty().WithMessage("Currency is required").MaximumLength(5).WithMessage("Currency should not be more than 5 characters");
    }
}