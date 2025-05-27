using Microsoft.AspNetCore.Mvc;
using SmartRide.API.DTOs;
using SmartRide.API.Services;
using System.Threading.Tasks;

namespace SmartRide.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // URL base for this controller will be /api/rides
    public class RidesController : ControllerBase
    {
        private readonly RideManager _rideManager;
        private readonly PaymentProcessor _paymentProcessor;

        public RidesController(RideManager rideManager, PaymentProcessor paymentProcessor)
        {
            _rideManager = rideManager;
            _paymentProcessor = paymentProcessor;
        }

        // POST /api/rides/book
        [HttpPost("book")]
        public async Task<IActionResult> BookRide([FromBody] RideBookingRequest request)
        {
            var ride = await _rideManager.RequestAndAssignDriverAsync(request.PassengerId, request.Pickup, request.Dropoff);

            if (ride == null)
            {
                return NotFound($"Passenger with ID {request.PassengerId} not found.");
            }

            if (ride.Status == Models.RideStatus.Cancelled)
            {
                return BadRequest("No available drivers found.");
            }

            return Ok(ride); // Return the created ride details
        }

        // PUT /api/rides/{rideId}/location
        [HttpPut("{rideId}/location")]
        public async Task<IActionResult> UpdateLocation(int rideId, [FromBody] LocationUpdateRequest request)
        {
            await _rideManager.UpdateDriverLocationAsync(rideId, request.NewLocation);
            return NoContent(); // HTTP 204 No Content is a standard response for successful updates
        }

        // POST /api/rides/{rideId}/complete
        [HttpPost("{rideId}/complete")]
        public async Task<IActionResult> CompleteRide(int rideId)
        {
            var completedRide = await _paymentProcessor.CompleteRideAsync(rideId);
            if (completedRide == null)
            {
                return NotFound($"Ride with ID {rideId} not found or cannot be completed.");
            }
            return Ok(completedRide);
        }

        // GET /api/rides/available
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableRides()
        {
            var availableRides = await _rideManager.GetAvailableRidesAsync();
            return Ok(availableRides);
        }

        // POST /api/rides/{rideId}/accept
        [HttpPost("{rideId}/accept")]
        public async Task<IActionResult> AcceptRide(int rideId, [FromBody] int driverId) 
        {
            var acceptedRide = await _rideManager.AcceptRideAsync(rideId, driverId);
            if (acceptedRide == null)
            {
                return BadRequest("Ride could not be accepted. It may have already been taken or the driver is not available.");
            }
            return Ok(acceptedRide);
        }
    }
}