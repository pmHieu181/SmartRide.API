namespace SmartRide.API.Models
{
    public class Driver : User
    {
        public required string VehicleDetails { get; set; }
        public Location? CurrentLocation { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
