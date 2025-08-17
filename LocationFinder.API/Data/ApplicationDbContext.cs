using Microsoft.EntityFrameworkCore;
using LocationFinder.API.Models;

namespace LocationFinder.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ZipCode> ZipCodes { get; set; }
        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ZipCode entity configuration
            modelBuilder.Entity<ZipCode>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ZipCodeValue).IsRequired().HasMaxLength(5);
                entity.Property(e => e.Latitude).HasColumnType("decimal(10,8)");
                entity.Property(e => e.Longitude).HasColumnType("decimal(11,8)");
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.State).HasMaxLength(50);

                // Create unique index on ZipCodeValue
                entity.HasIndex(e => e.ZipCodeValue).IsUnique();
            });

            // Location entity configuration
            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
                entity.Property(e => e.City).IsRequired().HasMaxLength(100);
                entity.Property(e => e.State).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ZipCode).IsRequired().HasMaxLength(5);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Latitude).HasColumnType("decimal(10,8)");
                entity.Property(e => e.Longitude).HasColumnType("decimal(11,8)");
                entity.Property(e => e.BusinessHours).HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");

                // Create indexes for performance
                entity.HasIndex(e => e.ZipCode);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => new { e.Latitude, e.Longitude });
            });

            // Seed sample data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed ZipCodes with major US cities
            var zipCodes = new[]
            {
                new ZipCode { Id = 1, ZipCodeValue = "10001", Latitude = 40.7505m, Longitude = -73.9934m, City = "New York", State = "NY" },
                new ZipCode { Id = 2, ZipCodeValue = "90210", Latitude = 34.0901m, Longitude = -118.4065m, City = "Beverly Hills", State = "CA" },
                new ZipCode { Id = 3, ZipCodeValue = "60601", Latitude = 41.8781m, Longitude = -87.6298m, City = "Chicago", State = "IL" },
                new ZipCode { Id = 4, ZipCodeValue = "77001", Latitude = 29.7604m, Longitude = -95.3698m, City = "Houston", State = "TX" },
                new ZipCode { Id = 5, ZipCodeValue = "33101", Latitude = 25.7617m, Longitude = -80.1918m, City = "Miami", State = "FL" },
                new ZipCode { Id = 6, ZipCodeValue = "85001", Latitude = 33.4484m, Longitude = -112.0740m, City = "Phoenix", State = "AZ" },
                new ZipCode { Id = 7, ZipCodeValue = "19101", Latitude = 39.9526m, Longitude = -75.1652m, City = "Philadelphia", State = "PA" },
                new ZipCode { Id = 8, ZipCodeValue = "78201", Latitude = 29.4241m, Longitude = -98.4936m, City = "San Antonio", State = "TX" },
                new ZipCode { Id = 9, ZipCodeValue = "92101", Latitude = 32.7157m, Longitude = -117.1611m, City = "San Diego", State = "CA" },
                new ZipCode { Id = 10, ZipCodeValue = "75201", Latitude = 32.7767m, Longitude = -96.7970m, City = "Dallas", State = "TX" },
                new ZipCode { Id = 11, ZipCodeValue = "98101", Latitude = 47.6062m, Longitude = -122.3321m, City = "Seattle", State = "WA" },
                new ZipCode { Id = 12, ZipCodeValue = "80201", Latitude = 39.7392m, Longitude = -104.9903m, City = "Denver", State = "CO" },
                new ZipCode { Id = 13, ZipCodeValue = "37201", Latitude = 36.1627m, Longitude = -86.7816m, City = "Nashville", State = "TN" },
                new ZipCode { Id = 14, ZipCodeValue = "70112", Latitude = 29.9511m, Longitude = -90.0715m, City = "New Orleans", State = "LA" },
                new ZipCode { Id = 15, ZipCodeValue = "20001", Latitude = 38.9072m, Longitude = -77.0369m, City = "Washington", State = "DC" }
            };

            modelBuilder.Entity<ZipCode>().HasData(zipCodes);

            // Seed sample Locations
            var locations = new[]
            {
                new Location { Id = 1, Name = "Downtown Office", Address = "123 Main St", City = "New York", State = "NY", ZipCode = "10001", Latitude = 40.7505m, Longitude = -73.9934m, Phone = "(212) 555-0101", BusinessHours = "Mon-Fri 9AM-5PM", IsActive = true, CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Location { Id = 2, Name = "Beverly Hills Branch", Address = "456 Rodeo Dr", City = "Beverly Hills", State = "CA", ZipCode = "90210", Latitude = 34.0901m, Longitude = -118.4065m, Phone = "(310) 555-0202", BusinessHours = "Mon-Sat 10AM-6PM", IsActive = true, CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Location { Id = 3, Name = "Chicago Loop Office", Address = "789 Michigan Ave", City = "Chicago", State = "IL", ZipCode = "60601", Latitude = 41.8781m, Longitude = -87.6298m, Phone = "(312) 555-0303", BusinessHours = "Mon-Fri 8AM-6PM", IsActive = true, CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Location { Id = 4, Name = "Houston Downtown", Address = "321 Texas Ave", City = "Houston", State = "TX", ZipCode = "77001", Latitude = 29.7604m, Longitude = -95.3698m, Phone = "(713) 555-0404", BusinessHours = "Mon-Fri 9AM-5PM", IsActive = true, CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Location { Id = 5, Name = "Miami Beach Office", Address = "654 Ocean Dr", City = "Miami", State = "FL", ZipCode = "33101", Latitude = 25.7617m, Longitude = -80.1918m, Phone = "(305) 555-0505", BusinessHours = "Mon-Sat 9AM-7PM", IsActive = true, CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Location { Id = 6, Name = "Phoenix Central", Address = "987 Central Ave", City = "Phoenix", State = "AZ", ZipCode = "85001", Latitude = 33.4484m, Longitude = -112.0740m, Phone = "(602) 555-0606", BusinessHours = "Mon-Fri 8AM-5PM", IsActive = true, CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Location { Id = 7, Name = "Philadelphia Historic", Address = "147 Liberty Bell Way", City = "Philadelphia", State = "PA", ZipCode = "19101", Latitude = 39.9526m, Longitude = -75.1652m, Phone = "(215) 555-0707", BusinessHours = "Mon-Fri 9AM-5PM", IsActive = true, CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Location { Id = 8, Name = "San Antonio Riverwalk", Address = "258 Riverwalk Blvd", City = "San Antonio", State = "TX", ZipCode = "78201", Latitude = 29.4241m, Longitude = -98.4936m, Phone = "(210) 555-0808", BusinessHours = "Mon-Sat 10AM-8PM", IsActive = true, CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Location { Id = 9, Name = "San Diego Harbor", Address = "369 Harbor Dr", City = "San Diego", State = "CA", ZipCode = "92101", Latitude = 32.7157m, Longitude = -117.1611m, Phone = "(619) 555-0909", BusinessHours = "Mon-Fri 9AM-6PM", IsActive = true, CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Location { Id = 10, Name = "Dallas Downtown", Address = "741 Commerce St", City = "Dallas", State = "TX", ZipCode = "75201", Latitude = 32.7767m, Longitude = -96.7970m, Phone = "(214) 555-1010", BusinessHours = "Mon-Fri 8AM-6PM", IsActive = true, CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            };

            modelBuilder.Entity<Location>().HasData(locations);
        }
    }
}
