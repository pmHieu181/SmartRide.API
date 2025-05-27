using Microsoft.EntityFrameworkCore;
using SmartRide.API.Data;
using SmartRide.API.Models;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SmartRide.API.Services
{
    public class RideManager
    {
        private readonly SmartRideDbContext _context;

        public RideManager(SmartRideDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new ride request and attempts to assign the nearest available driver.
        /// </summary>
        /// <param name="passengerId">The ID of the passenger requesting the ride.</param>
        /// <param name="pickup">The pickup location.</param>
        /// <param name="dropoff">The dropoff location.</param>
        /// <returns>The newly created Ride object, or null if the passenger is not found.</returns>
        public async Task<Ride?> RequestAndAssignDriverAsync(int passengerId, Location pickup, Location dropoff)
        {
            Console.WriteLine($"[RideManager] Passenger ID {passengerId} is requesting a new ride.");

            var passenger = await _context.Passengers.FindAsync(passengerId);
            if (passenger == null)
            {
                Console.WriteLine($"[RideManager] ERROR: Passenger with ID {passengerId} not found.");
                return null;
            }

            var ride = new Ride
            {
                PassengerId = passengerId,
                PickupLocation = pickup,
                DropoffLocation = dropoff,
                Status = RideStatus.Requested,
                Passenger = passenger
            };

            // Find the first available driver (in a real app, this logic would be more complex)
            var availableDriver = await _context.Drivers
                .Where(d => d.IsAvailable)
                .FirstOrDefaultAsync();

            if (availableDriver != null)
            {
                availableDriver.IsAvailable = false;
                ride.Driver = availableDriver;
                ride.Status = RideStatus.Confirmed;
                Console.WriteLine($"[RideManager] Driver {availableDriver.Name} has been assigned to the ride. Status: {ride.Status}.");
            }
            else
            {
                ride.Status = RideStatus.Cancelled;
                Console.WriteLine("[RideManager] Sorry, no drivers are available at the moment.");
            }

            await _context.Rides.AddAsync(ride);
            await _context.SaveChangesAsync();

            return ride;
        }

        /// <summary>
        /// Updates the driver's current location and the ride status.
        /// </summary>
        /// <param name="rideId">The ID of the ongoing ride.</param>
        /// <param name="newLocation">The driver's new location.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task UpdateDriverLocationAsync(int rideId, Location newLocation)
        {
            var ride = await _context.Rides.Include(r => r.Driver).FirstOrDefaultAsync(r => r.Id == rideId);

            if (ride?.Driver == null)
            {
                Console.WriteLine($"[RealTimeTracking] ERROR: Ride or Driver not found for Ride ID {rideId}.");
                return;
            }

            ride.Driver.CurrentLocation = newLocation;
            ride.Status = RideStatus.InProgress;

            await _context.SaveChangesAsync();

            Console.WriteLine($"[RealTimeTracking] Driver {ride.Driver.Name}'s location updated for ride #{ride.Id}. Status: {ride.Status}.");
        }

        /// <summary>
        /// Gets a list of all rides that are currently in 'Requested' status.
        /// </summary>
        /// <returns>A list of available rides.</returns>
        public async Task<List<Ride>> GetAvailableRidesAsync()
        {
            Console.WriteLine("[RideManager] Fetching all available ride requests.");

            return await _context.Rides
                .Where(r => r.Status == RideStatus.Requested)
                .Include(r => r.Passenger)
                .ToListAsync();
        }

        /// <summary>
        /// Allows a driver to accept a specific ride request.
        /// </summary>
        /// <param name="rideId">The ID of the ride to accept.</param>
        /// <param name="driverId">The ID of the driver accepting the ride.</param>
        /// <returns>The updated Ride object, or null if not found or could not be accepted.</returns>
        public async Task<Ride?> AcceptRideAsync(int rideId, int driverId)
        {
            var ride = await _context.Rides.FirstOrDefaultAsync(r => r.Id == rideId);
            var driver = await _context.Drivers.FindAsync(driverId);


            if (ride == null || driver == null || !driver.IsAvailable)
            {
                Console.WriteLine($"[RideManager] Cannot accept ride. Ride/Driver not found or driver is not available.");
                return null;
            }


            if (ride.Status != RideStatus.Requested)
            {
                Console.WriteLine($"[RideManager] Cannot accept ride. Ride is no longer available.");
                return null;
            }

            ride.Driver = driver;
            ride.Status = RideStatus.Confirmed;
            driver.IsAvailable = false; 

            await _context.SaveChangesAsync();

            Console.WriteLine($"[RideManager] Driver {driver.Name} accepted ride #{ride.Id}.");
            return ride;
        }
    }
}