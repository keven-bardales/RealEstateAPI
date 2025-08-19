using Microsoft.EntityFrameworkCore;
using RealEstateAPI.Common.Infrastructure;
using RealEstateAPI.Features.Properties.Domain;
using System;
using System.Threading.Tasks;

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

        public async Task<  List<Property>> GetByCityAsync(string city, CancellationToken cancellationToken = default)
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


            return property;
        }

        public async Task<Property> UpdateAsync(Property property, CancellationToken ct = default)
        {
            _context.Properties.Update(property);

            await _context.SaveChangesAsync(ct);

            return property;
        }


        public async Task <bool> DeleteAsync(Property property, CancellationToken cancellationToken = default)
        {
           _context.Properties.Remove(property);

           var result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                throw new Exception("Failed to delete property");
            }

            return true;

        }


        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}