using SmartRide.API.Models;

namespace SmartRide.API.DTOs
{
    public class LocationUpdateRequest
    {
        public required Location NewLocation { get; set; }
    }
}