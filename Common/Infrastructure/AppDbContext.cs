using Microsoft.EntityFrameworkCore;
using RealEstateAPI.Features.Properties.Domain;

namespace RealEstateAPI.Common.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Property Configuration
            modelBuilder.Entity<Property>(entity =>
            {
                
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
                
                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(2);
                
                entity.Property(e => e.ZipCode)
                    .IsRequired()
                    .HasMaxLength(10);
                
                entity.Property(e => e.Price)
                    .HasPrecision(18, 2)
                    .IsRequired();
                
                entity.Property(e => e.MonthlyRent)
                    .HasPrecision(18, 2)
                    .IsRequired();
                
                entity.Property(e => e.Bathrooms)
                    .HasPrecision(3, 1)
                    .IsRequired();
                
                entity.Property(e => e.PropertyType)
                    .HasMaxLength(50)
                    .IsRequired();
            });

            // Seed Data
            SeedProperties(modelBuilder);
        }

        private void SeedProperties(ModelBuilder modelBuilder)
        {
            var properties = new[]
            {
                new
                {
                    Id = 1,
                    Address = "123 Main Street",
                    City = "Houston",
                    State = "TX",
                    ZipCode = "77001",
                    Price = 250000m,
                    MonthlyRent = 1500m,
                    Bedrooms = 3,
                    Bathrooms = 2m,
                    SquareFeet = 1500,
                    YearBuilt = 2010,
                    PropertyType = "House",
                    IsAvailable = true,
                    ListedDateUtc = DateTime.UtcNow.AddDays(-30),
                    LastUpdatedUtc = (DateTime?)null
                },
                new
                {
                    Id = 2,
                    Address = "456 Oak Avenue",
                    City = "Austin",
                    State = "TX",
                    ZipCode = "78701",
                    Price = 350000m,
                    MonthlyRent = 2200m,
                    Bedrooms = 4,
                    Bathrooms = 3m,
                    SquareFeet = 2200,
                    YearBuilt = 2015,
                    PropertyType = "House",
                    IsAvailable = true,
                    ListedDateUtc = DateTime.UtcNow.AddDays(-15),
                    LastUpdatedUtc = (DateTime?)null
                },
                new
                {
                    Id = 3,
                    Address = "789 Elm Street",
                    City = "Dallas",
                    State = "TX",
                    ZipCode = "75201",
                    Price = 180000m,
                    MonthlyRent = 1200m,
                    Bedrooms = 2,
                    Bathrooms = 1m,
                    SquareFeet = 900,
                    YearBuilt = 2018,
                    PropertyType = "Apartment",
                    IsAvailable = false,
                    ListedDateUtc = DateTime.UtcNow.AddDays(-45),
                    LastUpdatedUtc = (DateTime?)null
                },
                new
                {
                    Id = 4,
                    Address = "321 Pine Road",
                    City = "Houston",
                    State = "TX",
                    ZipCode = "77002",
                    Price = 450000m,
                    MonthlyRent = 2800m,
                    Bedrooms = 5,
                    Bathrooms = 3.5m,
                    SquareFeet = 3000,
                    YearBuilt = 2020,
                    PropertyType = "House",
                    IsAvailable = true,
                    ListedDateUtc = DateTime.UtcNow.AddDays(-7),
                    LastUpdatedUtc = (DateTime?)null
                },
                new
                {
                    Id = 5,
                    Address = "555 Sunset Boulevard",
                    City = "Austin",
                    State = "TX",
                    ZipCode = "78702",
                    Price = 275000m,
                    MonthlyRent = 1800m,
                    Bedrooms = 3,
                    Bathrooms = 2.5m,
                    SquareFeet = 1700,
                    YearBuilt = 2012,
                    PropertyType = "Condo",
                    IsAvailable = true,
                    ListedDateUtc = DateTime.UtcNow.AddDays(-20),
                    LastUpdatedUtc = (DateTime?)null
                }
            };

            modelBuilder.Entity<Property>().HasData(properties);
        }
    }
}