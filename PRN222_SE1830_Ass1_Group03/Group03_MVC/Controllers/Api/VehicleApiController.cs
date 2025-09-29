using BusinessObjects.DTO;
using Microsoft.AspNetCore.Mvc;
using Services.Service;

namespace Group03_MVC.Controllers.Api
{
    [ApiController]
    [Route("api/vehicle")]
    public class VehicleApiController : ControllerBase
    {
        private readonly VehicleService _vehicleService;

        public VehicleApiController(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        /// <summary>
        /// Get all available vehicles
        /// </summary>
        /// <returns>List of vehicles</returns>
        [HttpGet]
        public async Task<ActionResult<List<VehicleDto>>> GetVehicles()
        {
            try
            {
                var vehicles = await _vehicleService.GetAvailableVehicles();
                return Ok(vehicles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get vehicle details by ID
        /// </summary>
        /// <param name="id">Vehicle ID</param>
        /// <returns>Vehicle details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleDetailDto>> GetVehicle(Guid id)
        {
            try
            {
                var vehicle = await _vehicleService.GetVehicleById(id);
                if (vehicle == null)
                {
                    return NotFound(new { message = "Vehicle not found" });
                }
                return Ok(vehicle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
