using SmartRide.API.Models;

namespace SmartRide.API.DTOs
{
    public class RideBookingRequest
    {
        public int PassengerId { get; set; }
        public required Location Pickup { get; set; }
        public required Location Dropoff { get; set; }
    }
}