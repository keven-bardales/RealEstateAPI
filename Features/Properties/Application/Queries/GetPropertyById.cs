using MediatR;
using RealEstateAPI.Features.Properties.Domain;

namespace RealEstateAPI.Features.Properties.Application.Queries
{
    public record GetPropertyByIdQuery(int Id) : IRequest<PropertyDto?>;

    public class GetPropertyByIdHandler : IRequestHandler<GetPropertyByIdQuery, PropertyDto?>
    {
        private readonly IPropertyRepository _repository;

        public GetPropertyByIdHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<PropertyDto?> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
        {
            var property = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (property == null)
                return null;

            return new PropertyDto(
                property.Id,
                property.Address,
                property.City,
                property.State,
                property.ZipCode,
                property.Price,
                property.MonthlyRent,
                property.Bedrooms,
                property.Bathrooms,
                property.SquareFeet,
                property.YearBuilt,
                property.PropertyType,
                property.IsAvailable,
                property.ListedDateUtc,
                property.LastUpdatedUtc
            );
        }
    }
}