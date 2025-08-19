namespace RealEstateAPI.Features.Properties.Domain
{
    // Entidad principal
    public class Property
    {
        public int Id { get; private set; }
        public string Address { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string ZipCode { get; private set; }
        public decimal Price { get; private set; }
        public decimal MonthlyRent { get; private set; }
        public int Bedrooms { get; private set; }
        public decimal Bathrooms { get; private set; }
        public int SquareFeet { get; private set; }
        public int YearBuilt { get; private set; }
        public string PropertyType { get; private set; }
        public bool IsAvailable { get; private set; }
        public DateTime ListedDateUtc { get; private set; }
        public DateTime? LastUpdatedUtc { get; private set; }

        private Property() { }

        public static Property Create(PropertyCreationData creationData)
        {
            ValidateCreationData(creationData);

            return new Property
            {
                Address = creationData.Address,
                City = creationData.City,
                State = creationData.State,
                ZipCode = creationData.ZipCode,
                Price = creationData.Price,
                MonthlyRent = creationData.MonthlyRent,
                Bedrooms = creationData.Bedrooms,
                Bathrooms = creationData.Bathrooms,
                SquareFeet = creationData.SquareFeet,
                YearBuilt = creationData.YearBuilt,
                PropertyType = creationData.PropertyType ?? "Apartment",
                IsAvailable = true,
                ListedDateUtc = DateTime.UtcNow
            };
        }

        private static void ValidateCreationData(PropertyCreationData data)
        {
            if (string.IsNullOrWhiteSpace(data.Address))
                throw new PropertyDomainException("Address is required");

            if (string.IsNullOrWhiteSpace(data.City))
                throw new PropertyDomainException("City is required");

            if (string.IsNullOrWhiteSpace(data.State) || data.State.Length != 2)
                throw new PropertyDomainException("State must be 2 characters");

            if (string.IsNullOrWhiteSpace(data.ZipCode) || data.ZipCode.Length != 5)
                throw new PropertyDomainException("ZipCode must be 5 digits");

            if (data.Price <= 0)
                throw new PropertyDomainException("Price must be greater than 0");

            if (data.MonthlyRent <= 0)
                throw new PropertyDomainException("Monthly Rent must be greater than 0");

            if (data.Bedrooms < 0 || data.Bedrooms > 20)
                throw new PropertyDomainException("Bedrooms must be between 0 and 20");

            if (data.Bathrooms < 0 || data.Bathrooms > 20)
                throw new PropertyDomainException("Bathrooms must be between 0 and 20");

            if (data.SquareFeet <= 0)
                throw new PropertyDomainException("Square feet must be greater than 0");

            if (data.YearBuilt < 1800 || data.YearBuilt > DateTime.Now.Year)
                throw new PropertyDomainException($"Year built must be between 1800 and {DateTime.Now.Year}");
        }

        public void Update(PropertyUpdateData updateData)
        {
            Address = updateData.Address ?? Address;
            City = updateData.City ?? City;
            State = updateData.State ?? State;
            ZipCode = updateData.ZipCode ?? ZipCode;
            Price = updateData.Price ?? Price;
            MonthlyRent = updateData.MonthlyRent ?? MonthlyRent;
            Bedrooms = updateData.Bedrooms ?? Bedrooms;
            Bathrooms = updateData.Bathrooms ?? Bathrooms;
            SquareFeet = updateData.SquareFeet ?? SquareFeet;
            YearBuilt = updateData.YearBuilt ?? YearBuilt;
            PropertyType = updateData.PropertyType ?? PropertyType;
            LastUpdatedUtc = DateTime.UtcNow;
        }

        public void MarkAsAvailable() => IsAvailable = true;
        public void MarkAsOccupied() => IsAvailable = false;
    }

    // Value Object para crear propiedades (en el mismo archivo)
    public record PropertyCreationData(
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
        string? PropertyType = null
    );

    public record PropertyUpdateData(
        string? Address = null,
        string? City = null,
        string? State = null,
        string? ZipCode = null,
        decimal? Price = null,
        decimal? MonthlyRent = null,
        int? Bedrooms = null,
        decimal? Bathrooms = null,
        int? SquareFeet = null,
        int? YearBuilt = null,
        string? PropertyType = null
    );

    // Exception personalizada del dominio
    public class PropertyDomainException : Exception
    {
        public PropertyDomainException(string message) : base(message) { }
    }
}