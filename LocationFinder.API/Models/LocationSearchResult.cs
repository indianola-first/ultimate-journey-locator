namespace LocationFinder.API.Models
{
    public class LocationSearchResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? BusinessHours { get; set; }
        public double DistanceMiles { get; set; }
    }
}
