namespace SmartRide.API.Models
{
    public enum RideStatus
    {
        Requested,
        Confirmed,
        InProgress,
        Completed,
        Cancelled
    }
    public class Ride
    {
        public int Id { get; set; }
        public int PassengerId { get; set; }
        public int? DriverId { get; set; }

        public Location PickupLocation { get; set; }
        public Location DropoffLocation { get; set; }
        public RideStatus Status { get; set; }
        public decimal Fare { get; set; }

        // Navigation properties
        public virtual Passenger Passenger { get; set; }
        public virtual Driver Driver { get; set; }
    }
}
