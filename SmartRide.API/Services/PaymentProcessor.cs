using Microsoft.EntityFrameworkCore;
using SmartRide.API.Data;
using SmartRide.API.Models;
using System;
using System.Threading.Tasks;

namespace SmartRide.API.Services
{
    public class PaymentProcessor
    {
        private readonly SmartRideDbContext _context;

        public PaymentProcessor(SmartRideDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Completes a ride, calculates the fare, and updates the database.
        /// </summary>
        /// <param name="rideId">The ID of the ride to complete.</param>
        /// <returns>The completed Ride object, or null if the ride is not found or in an invalid state.</returns>
        public async Task<Ride?> CompleteRideAsync(int rideId)
        {
            var ride = await _context.Rides
                .Include(r => r.Driver)
                .Include(r => r.Passenger)
                .FirstOrDefaultAsync(r => r.Id == rideId);

            if (ride == null)
            {
                Console.WriteLine($"[PaymentProcessor] ERROR: Ride with ID {rideId} not found.");
                return null;
            }

            if (ride.Status != RideStatus.InProgress)
            {
                Console.WriteLine($"[PaymentProcessor] ERROR: Ride #{ride.Id} cannot be completed because its status is '{ride.Status}'.");
                return null;
            }

            Console.WriteLine($"[PaymentProcessor] Completing ride #{ride.Id}. Calculating fare...");
            ride.Fare = CalculateFare(ride);
            ride.Status = RideStatus.Completed;

            // Make the driver available for the next ride
            if (ride.Driver != null)
            {
                ride.Driver.IsAvailable = true;
            }

            await _context.SaveChangesAsync();

            Console.WriteLine($"[PaymentProcessor] Ride #{ride.Id} completed. Fare is: {ride.Fare:C}.");
            Console.WriteLine($"--> Simulated payment processed for passenger '{ride.Passenger.Name}'.");

            return ride;
        }

        private decimal CalculateFare(Ride ride)
        {
            // Simple fare calculation logic based on distance
            double distance = Math.Sqrt(
                Math.Pow(ride.DropoffLocation.Latitude - ride.PickupLocation.Latitude, 2) +
                Math.Pow(ride.DropoffLocation.Longitude - ride.PickupLocation.Longitude, 2)
            );

            // Example: Base fare of 1.50 + 2.50 per distance unit
            return 1.50m + ((decimal)distance * 2.50m * 100); // Multiplying to get a reasonable fare
        }
    }
}