namespace RealEstateAPI.Features.Properties.Domain
{
    public interface IPropertyRepository
    {

        // Queries
        Task<Property> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<Property>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<Property>> GetAvailableAsync(CancellationToken cancellationToken = default);
        Task<List<Property>> GetByCityAsync(string city, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default); // Task<bool>

        // Commands
        Task<Property?> AddAsync(Property property, CancellationToken cancellationToken = default);
        Task<Property?> UpdateAsync(Property property, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Property property, CancellationToken cancellationToken = default);

        // Unit of Work
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
