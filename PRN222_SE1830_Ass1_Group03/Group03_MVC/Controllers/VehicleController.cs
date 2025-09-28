using BusinessObjects.DTO;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Service;

namespace Group03_MVC.Controllers
{
    public class VehicleController : Controller
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var vehicles = await _vehicleService.GetAllVehicles();
                return View(vehicles);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error retrieving vehicles: {ex.Message}");
                return View(new List<Vehicle>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var vehicle = await _vehicleService.GetVehicleById(id);
                return View(vehicle);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error retrieving vehicle: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VehicleDTO vehicle)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(vehicle);
                }
                await _vehicleService.AddVehicle(vehicle);
                TempData["SuccessMessage"] = "Vehicle added successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error adding vehicle: {ex.Message}");
                return View(vehicle);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var vehicle = await _vehicleService.GetVehicleById(id);
                if (vehicle == null)
                {
                    return NotFound();
                }
                var vehicleDTO = new VehicleDTO
                {
                    Id = vehicle.Id,
                    Name = vehicle.Name,
                    Brand = vehicle.Brand,
                    Model = vehicle.Model,
                    Year = vehicle.Year,
                    Price = vehicle.Price,
                    Description = vehicle.Description,
                    Specifications = vehicle.Specifications,
                    Images = vehicle.Images,
                    StockQuantity = vehicle.StockQuantity
                };
                return View(vehicleDTO);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error retrieving vehicle: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VehicleDTO vehicle)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(vehicle);
                }
                await _vehicleService.UpdateVehicle(vehicle);
                TempData["SuccessMessage"] = "Vehicle updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating vehicle: {ex.Message}");
                return View(vehicle);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var vehicle = await _vehicleService.GetVehicleById(id);
                if (vehicle == null)
                {
                    return NotFound();
                }
                var vehicleDTO = new VehicleDTO
                {
                    Id = vehicle.Id,
                    Name = vehicle.Name,
                    Brand = vehicle.Brand,
                    Model = vehicle.Model,
                    Year = vehicle.Year,
                    Price = vehicle.Price,
                    Description = vehicle.Description,
                    Specifications = vehicle.Specifications,
                    Images = vehicle.Images,
                    StockQuantity = vehicle.StockQuantity
                };
                return View(vehicleDTO);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error retrieving vehicle: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _vehicleService.DeleteVehicle(id);
                TempData["SuccessMessage"] = "Vehicle deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error deleting vehicle: {ex.Message}");
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}