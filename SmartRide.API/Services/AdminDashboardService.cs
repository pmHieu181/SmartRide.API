using Microsoft.EntityFrameworkCore;
using SmartRide.API.Data;
using SmartRide.API.Models;
using System.Threading.Tasks;
using System;

namespace SmartRide.API.Services
{
    public class AdminDashboardService
    {
        private readonly SmartRideDbContext _context;

        public AdminDashboardService(SmartRideDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Reviews a ride and adjusts the fare as part of a dispute resolution.
        /// </summary>
        /// <param name="rideId">The ID of the ride to adjust.</param>
        /// <param name="adjustmentAmount">The amount to add or subtract from the fare.</param>
        /// <param name="reason">The reason for the adjustment.</param>
        /// <returns>The updated Ride object, or null if the ride is not found.</returns>
        public async Task<Ride?> ReviewAndAdjustFareAsync(int rideId, decimal adjustmentAmount, string reason)
        {
            var ride = await _context.Rides.FindAsync(rideId);
            if (ride == null)
            {
                Console.WriteLine($"[AdminDashboardService] ERROR: Ride with ID {rideId} not found for review.");
                return null;
            }

            Console.WriteLine($"[AdminDashboardService] Admin reviewing ride #{ride.Id}.");
            Console.WriteLine($"[AdminDashboardService] Reason: {reason}.");
            Console.WriteLine($"[AdminDashboardService] Original Fare: {ride.Fare:C}.");

            ride.Fare += adjustmentAmount;

            await _context.SaveChangesAsync();

            Console.WriteLine($"[AdminDashboardService] Fare adjusted. Adjustment amount: {adjustmentAmount:C}.");
            Console.WriteLine($"[AdminDashboardService] New Fare is now: {ride.Fare:C}.");

            return ride;
        }
    }
}