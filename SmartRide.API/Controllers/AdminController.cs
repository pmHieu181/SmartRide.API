using Microsoft.AspNetCore.Mvc;
using SmartRide.API.DTOs;
using SmartRide.API.Services;
using System.Threading.Tasks;

namespace SmartRide.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // URL base will be /api/admin
    public class AdminController : ControllerBase
    {
        private readonly AdminDashboardService _adminService;

        public AdminController(AdminDashboardService adminService)
        {
            _adminService = adminService;
        }

        // PUT /api/admin/rides/{rideId}/adjust-fare
        [HttpPut("rides/{rideId}/adjust-fare")]
        public async Task<IActionResult> AdjustFare(int rideId, [FromBody] AdjustFareRequest request)
        {
            var adjustedRide = await _adminService.ReviewAndAdjustFareAsync(rideId, request.AdjustmentAmount, request.Reason);
            if (adjustedRide == null)
            {
                return NotFound($"Ride with ID {rideId} not found.");
            }
            return Ok(adjustedRide);
        }
    }
}