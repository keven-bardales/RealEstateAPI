using FluentValidation;
using MediatR;
using RealEstateAPI.Features.Properties.Domain;

namespace RealEstateAPI.Features.Properties.Application.Commands
{
    public record UpdatePropertyCommand(
        int Id,
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
    ) : IRequest<UpdatePropertyResult>;

    public record UpdatePropertyResult(bool Success, string Message);

    public class UpdatePropertyCommandValidator : AbstractValidator<UpdatePropertyCommand>
    {
        public UpdatePropertyCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Address).NotEmpty().MaximumLength(200);
            RuleFor(x => x.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.State).NotEmpty().Length(2);
            RuleFor(x => x.ZipCode).NotEmpty().Matches(@"^\d{5}$");
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.MonthlyRent).GreaterThan(0);
            RuleFor(x => x.Bedrooms).InclusiveBetween(0, 20);
            RuleFor(x => x.Bathrooms).InclusiveBetween(0.5m, 20);
            RuleFor(x => x.SquareFeet).GreaterThan(0);
            RuleFor(x => x.YearBuilt).InclusiveBetween(1800, DateTime.Now.Year);
        }
    }

    public class UpdatePropertyHandler : IRequestHandler<UpdatePropertyCommand, UpdatePropertyResult>
    {
        private readonly IPropertyRepository _repository;
        private readonly IValidator<UpdatePropertyCommand> _validator;

        public UpdatePropertyHandler(IPropertyRepository repository, IValidator<UpdatePropertyCommand> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<UpdatePropertyResult> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new UpdatePropertyResult(false, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var property = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (property == null)
            {
                return new UpdatePropertyResult(false, $"Property with ID {request.Id} not found");
            }

            try
            {
                // Necesitas agregar el método Update en Property.cs si no lo tienes
                property.Update(new PropertyUpdateData(
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
                ));

                _repository.Update(property);
                await _repository.SaveChangesAsync(cancellationToken);

                return new UpdatePropertyResult(true, "Property updated successfully");
            }
            catch (PropertyDomainException ex)
            {
                return new UpdatePropertyResult(false, ex.Message);
            }
        }
    }
}