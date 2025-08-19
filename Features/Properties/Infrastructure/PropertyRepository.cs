using Microsoft.EntityFrameworkCore;
using RealEstateAPI.Common.Infrastructure;
using RealEstateAPI.Features.Properties.Domain;
using System;

namespace RealEstateAPI.Features.Properties.Infrastructure
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly AppDbContext _context;

        public PropertyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Property?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Properties
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<List<Property>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Properties
                .OrderBy(p => p.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Property>> GetAvailableAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Properties
                .Where(p => p.IsAvailable)
                .OrderBy(p => p.Price)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Property>> GetByCityAsync(string city, CancellationToken cancellationToken = default)
        {
            return await _context.Properties
                .Where(p => p.City.ToLower() == city.ToLower())
                .OrderBy(p => p.Price)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Properties
                .AnyAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Property> AddAsync(Property property, CancellationToken cancellationToken = default)
        {
            await _context.Properties.AddAsync(property, cancellationToken);
            return property.Id;
        }

        public void UpdateAsync(Property property)
        {
            _context.Properties.Update(property);

        }

        public void DeleteAsync(Property property)
        {
            _context.Properties.Remove(property);

        }


        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}