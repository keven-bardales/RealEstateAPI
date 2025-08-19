using MediatR;
using RealEstateAPI.Features.Properties.Domain;

namespace RealEstateAPI.Features.Properties.Application.Queries
{
    // Query
    public record GetAllPropertiesQuery(
        bool? IsAvailable = null,
        string? City = null,
        decimal? MinPrice = null,
        decimal? MaxPrice = null,
        int? MinBedrooms = null,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<GetAllPropertiesResult>;

    // DTOs
    public record PropertyDto(
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
        string PropertyType,
        bool IsAvailable,
        DateTime ListedDateUtc,
        DateTime? LastUpdatedUtc
    );

    public record GetAllPropertiesResult(
        List<PropertyDto> Properties,
        int TotalCount,
        int PageNumber,
        int PageSize,
        int TotalPages,
        bool HasPreviousPage,
        bool HasNextPage
    );

    // Handler
    public class GetAllPropertiesHandler : IRequestHandler<GetAllPropertiesQuery, GetAllPropertiesResult>
    {
        private readonly IPropertyRepository _repository;

        public GetAllPropertiesHandler(IPropertyRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetAllPropertiesResult> Handle(GetAllPropertiesQuery request, CancellationToken cancellationToken)
        {
            var properties = await _repository.GetAllAsync(cancellationToken);

            // Apply filters
            var query = properties.AsQueryable();

            if (request.IsAvailable.HasValue)
                query = query.Where(p => p.IsAvailable == request.IsAvailable.Value);

            if (!string.IsNullOrEmpty(request.City))
                query = query.Where(p => p.City.Contains(request.City, StringComparison.OrdinalIgnoreCase));

            if (request.MinPrice.HasValue)
                query = query.Where(p => p.Price >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= request.MaxPrice.Value);

            if (request.MinBedrooms.HasValue)
                query = query.Where(p => p.Bedrooms >= request.MinBedrooms.Value);

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            // Pagination
            var pagedProperties = query
                .OrderBy(p => p.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new PropertyDto(
                    p.Id,
                    p.Address,
                    p.City,
                    p.State,
                    p.ZipCode,
                    p.Price,
                    p.MonthlyRent,
                    p.Bedrooms,
                    p.Bathrooms,
                    p.SquareFeet,
                    p.YearBuilt,
                    p.PropertyType,
                    p.IsAvailable,
                    p.ListedDateUtc,
                    p.LastUpdatedUtc
                ))
                .ToList();

            return new GetAllPropertiesResult(
                pagedProperties,
                totalCount,
                request.PageNumber,
                request.PageSize,
                totalPages,
                request.PageNumber > 1,
                request.PageNumber < totalPages
            );
        }
    }
}