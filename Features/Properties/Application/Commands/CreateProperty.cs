using FluentValidation;
using MediatR;
using RealEstateAPI.Features.Properties.Domain;

namespace RealEstateAPI.Features.Properties.Application.Commands
{
    // Command
    public record CreatePropertyCommand(
        string Address,
        string City,
        string State,
        string ZipCode,
        decimal Price,
        decimal MonthlyRent,
        int Bedrooms,
        decimal Bathrooms,
        int SquareFeet,
        int YearBuilt,
        string? PropertyType
    ) : IRequest<CreatePropertyResult>;

    // Result
    public record CreatePropertyResult(
        int Id,
        string Address,
        string City,
        string State,
        bool Success,
        string Message,
        List<string>? Errors = null
    );

    // Validator
    public class CreatePropertyCommandValidator : AbstractValidator<CreatePropertyCommand>
    {
        public CreatePropertyCommandValidator()
        {
            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required")
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters");

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required")
                .Length(2).WithMessage("State must be 2 characters");

            RuleFor(x => x.ZipCode)
                .NotEmpty().WithMessage("ZipCode is required")
                .Matches(@"^\d{5}$").WithMessage("ZipCode must be 5 digits");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0")
                .LessThanOrEqualTo(10000000).WithMessage("Price cannot exceed $10,000,000");

            RuleFor(x => x.MonthlyRent)
                .GreaterThan(0).WithMessage("Monthly rent must be greater than 0")
                .LessThanOrEqualTo(100000).WithMessage("Monthly rent cannot exceed $100,000");

            RuleFor(x => x.Bedrooms)
                .InclusiveBetween(0, 20).WithMessage("Bedrooms must be between 0 and 20");

            RuleFor(x => x.Bathrooms)
                .InclusiveBetween(0.5m, 20).WithMessage("Bathrooms must be between 0.5 and 20");

            RuleFor(x => x.SquareFeet)
                .GreaterThan(0).WithMessage("Square feet must be greater than 0")
                .LessThanOrEqualTo(50000).WithMessage("Square feet cannot exceed 50,000");

            RuleFor(x => x.YearBuilt)
                .InclusiveBetween(1800, DateTime.Now.Year)
                .WithMessage($"Year built must be between 1800 and {DateTime.Now.Year}");
        }
    }

    // Handler
    public class CreatePropertyHandler : IRequestHandler<CreatePropertyCommand, CreatePropertyResult>
    {
        private readonly IPropertyRepository _repository;
        private readonly IValidator<CreatePropertyCommand> _validator;

        public CreatePropertyHandler(
            IPropertyRepository repository,
            IValidator<CreatePropertyCommand> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<CreatePropertyResult> Handle(
            CreatePropertyCommand request,
            CancellationToken cancellationToken)
        {
            // Validate with FluentValidation
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new CreatePropertyResult(
                    0, "", "", "",
                    false,
                    "Validation failed",
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                );
            }

            try
            {
                // Map to domain value object
                var creationData = new PropertyCreationData(
                    request.Address,
                    request.City,
                    request.State,
                    request.ZipCode,
                    request.Price,
                    request.MonthlyRent,
                    request.Bedrooms,
                    request.Bathrooms,
                    request.SquareFeet,
                    request.YearBuilt,
                    request.PropertyType
                );

                // Create domain entity
                var property = Property.Create(creationData);

                // Save to repository
                await _repository.AddAsync(property, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken);

                return new CreatePropertyResult(
                    property.Id,
                    property.Address,
                    property.City,
                    property.State,
                    true,
                    "Property created successfully"
                );
            }
            catch (PropertyDomainException ex)
            {
                return new CreatePropertyResult(
                    0, "", "", "",
                    false,
                    ex.Message
                );
            }
            catch (Exception ex)
            {
                // Log the exception here
                return new CreatePropertyResult(
                    0, "", "", "",
                    false,
                    "An unexpected error occurred while creating the property"
                );
            }
        }
    }
}