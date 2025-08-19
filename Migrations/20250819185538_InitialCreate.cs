using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RealEstateAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MonthlyRent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Bedrooms = table.Column<int>(type: "int", nullable: false),
                    Bathrooms = table.Column<decimal>(type: "decimal(3,1)", precision: 3, scale: 1, nullable: false),
                    SquareFeet = table.Column<int>(type: "int", nullable: false),
                    YearBuilt = table.Column<int>(type: "int", nullable: false),
                    PropertyType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    ListedDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Properties",
                columns: new[] { "Id", "Address", "Bathrooms", "Bedrooms", "City", "IsAvailable", "LastUpdatedUtc", "ListedDateUtc", "MonthlyRent", "Price", "PropertyType", "SquareFeet", "State", "YearBuilt", "ZipCode" },
                values: new object[,]
                {
                    { 1, "123 Main Street", 2m, 3, "Houston", true, null, new DateTime(2025, 7, 20, 18, 55, 38, 406, DateTimeKind.Utc).AddTicks(1654), 1500m, 250000m, "House", 1500, "TX", 2010, "77001" },
                    { 2, "456 Oak Avenue", 3m, 4, "Austin", true, null, new DateTime(2025, 8, 4, 18, 55, 38, 406, DateTimeKind.Utc).AddTicks(1661), 2200m, 350000m, "House", 2200, "TX", 2015, "78701" },
                    { 3, "789 Elm Street", 1m, 2, "Dallas", false, null, new DateTime(2025, 7, 5, 18, 55, 38, 406, DateTimeKind.Utc).AddTicks(1662), 1200m, 180000m, "Apartment", 900, "TX", 2018, "75201" },
                    { 4, "321 Pine Road", 3.5m, 5, "Houston", true, null, new DateTime(2025, 8, 12, 18, 55, 38, 406, DateTimeKind.Utc).AddTicks(1664), 2800m, 450000m, "House", 3000, "TX", 2020, "77002" },
                    { 5, "555 Sunset Boulevard", 2.5m, 3, "Austin", true, null, new DateTime(2025, 7, 30, 18, 55, 38, 406, DateTimeKind.Utc).AddTicks(1665), 1800m, 275000m, "Condo", 1700, "TX", 2012, "78702" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Properties");
        }
    }
}
