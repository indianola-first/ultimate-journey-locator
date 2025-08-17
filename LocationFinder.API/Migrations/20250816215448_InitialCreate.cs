using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LocationFinder.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(10,8)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(11,8)", nullable: false),
                    BusinessHours = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZipCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZipCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(10,8)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(11,8)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZipCodes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Address", "BusinessHours", "City", "CreatedDate", "IsActive", "Latitude", "Longitude", "Name", "Phone", "State", "ZipCode" },
                values: new object[,]
                {
                    { 1, "123 Main St", "Mon-Fri 9AM-5PM", "New York", new DateTime(2025, 8, 16, 21, 54, 47, 108, DateTimeKind.Utc).AddTicks(6170), true, 40.7505m, -73.9934m, "Downtown Office", "(212) 555-0101", "NY", "10001" },
                    { 2, "456 Rodeo Dr", "Mon-Sat 10AM-6PM", "Beverly Hills", new DateTime(2025, 8, 16, 21, 54, 47, 108, DateTimeKind.Utc).AddTicks(6770), true, 34.0901m, -118.4065m, "Beverly Hills Branch", "(310) 555-0202", "CA", "90210" },
                    { 3, "789 Michigan Ave", "Mon-Fri 8AM-6PM", "Chicago", new DateTime(2025, 8, 16, 21, 54, 47, 108, DateTimeKind.Utc).AddTicks(6770), true, 41.8781m, -87.6298m, "Chicago Loop Office", "(312) 555-0303", "IL", "60601" },
                    { 4, "321 Texas Ave", "Mon-Fri 9AM-5PM", "Houston", new DateTime(2025, 8, 16, 21, 54, 47, 108, DateTimeKind.Utc).AddTicks(6780), true, 29.7604m, -95.3698m, "Houston Downtown", "(713) 555-0404", "TX", "77001" },
                    { 5, "654 Ocean Dr", "Mon-Sat 9AM-7PM", "Miami", new DateTime(2025, 8, 16, 21, 54, 47, 108, DateTimeKind.Utc).AddTicks(6780), true, 25.7617m, -80.1918m, "Miami Beach Office", "(305) 555-0505", "FL", "33101" },
                    { 6, "987 Central Ave", "Mon-Fri 8AM-5PM", "Phoenix", new DateTime(2025, 8, 16, 21, 54, 47, 108, DateTimeKind.Utc).AddTicks(6790), true, 33.4484m, -112.0740m, "Phoenix Central", "(602) 555-0606", "AZ", "85001" },
                    { 7, "147 Liberty Bell Way", "Mon-Fri 9AM-5PM", "Philadelphia", new DateTime(2025, 8, 16, 21, 54, 47, 108, DateTimeKind.Utc).AddTicks(6790), true, 39.9526m, -75.1652m, "Philadelphia Historic", "(215) 555-0707", "PA", "19101" },
                    { 8, "258 Riverwalk Blvd", "Mon-Sat 10AM-8PM", "San Antonio", new DateTime(2025, 8, 16, 21, 54, 47, 108, DateTimeKind.Utc).AddTicks(6790), true, 29.4241m, -98.4936m, "San Antonio Riverwalk", "(210) 555-0808", "TX", "78201" },
                    { 9, "369 Harbor Dr", "Mon-Fri 9AM-6PM", "San Diego", new DateTime(2025, 8, 16, 21, 54, 47, 108, DateTimeKind.Utc).AddTicks(6800), true, 32.7157m, -117.1611m, "San Diego Harbor", "(619) 555-0909", "CA", "92101" },
                    { 10, "741 Commerce St", "Mon-Fri 8AM-6PM", "Dallas", new DateTime(2025, 8, 16, 21, 54, 47, 108, DateTimeKind.Utc).AddTicks(6800), true, 32.7767m, -96.7970m, "Dallas Downtown", "(214) 555-1010", "TX", "75201" }
                });

            migrationBuilder.InsertData(
                table: "ZipCodes",
                columns: new[] { "Id", "City", "Latitude", "Longitude", "State", "ZipCode" },
                values: new object[,]
                {
                    { 1, "New York", 40.7505m, -73.9934m, "NY", "10001" },
                    { 2, "Beverly Hills", 34.0901m, -118.4065m, "CA", "90210" },
                    { 3, "Chicago", 41.8781m, -87.6298m, "IL", "60601" },
                    { 4, "Houston", 29.7604m, -95.3698m, "TX", "77001" },
                    { 5, "Miami", 25.7617m, -80.1918m, "FL", "33101" },
                    { 6, "Phoenix", 33.4484m, -112.0740m, "AZ", "85001" },
                    { 7, "Philadelphia", 39.9526m, -75.1652m, "PA", "19101" },
                    { 8, "San Antonio", 29.4241m, -98.4936m, "TX", "78201" },
                    { 9, "San Diego", 32.7157m, -117.1611m, "CA", "92101" },
                    { 10, "Dallas", 32.7767m, -96.7970m, "TX", "75201" },
                    { 11, "Seattle", 47.6062m, -122.3321m, "WA", "98101" },
                    { 12, "Denver", 39.7392m, -104.9903m, "CO", "80201" },
                    { 13, "Nashville", 36.1627m, -86.7816m, "TN", "37201" },
                    { 14, "New Orleans", 29.9511m, -90.0715m, "LA", "70112" },
                    { 15, "Washington", 38.9072m, -77.0369m, "DC", "20001" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locations_IsActive",
                table: "Locations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Latitude_Longitude",
                table: "Locations",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ZipCode",
                table: "Locations",
                column: "ZipCode");

            migrationBuilder.CreateIndex(
                name: "IX_ZipCodes_ZipCode",
                table: "ZipCodes",
                column: "ZipCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "ZipCodes");
        }
    }
}
