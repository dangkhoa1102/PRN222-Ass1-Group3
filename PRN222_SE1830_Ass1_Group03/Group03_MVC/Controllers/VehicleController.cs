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
        public async Task<IActionResult> GetAllVehicle()
        {
            try
            {
                var rs = await _vehicleService.GetAllVehicles();
                return View(rs);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicleById(Guid Id)
        {
            try
            {
                var rs = await _vehicleService.GetVehicleById(Id);
                return View(rs);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicle(VehicleDTO vehicle)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _vehicleService.AddVehicle(vehicle);
                    return RedirectToAction("Index");
                }
                return View(vehicle);
            }
            catch (Exception ex)
            {
                return BadRequest($"Vehicle {ex.Message}");
            }
        }

        public IActionResult UpdateVehicle(Guid id)
        {
            var vehicle = _vehicleService.GetVehicleById(id);
            if (vehicle == null) return NotFound();
            return View(vehicle);
        }

        [HttpPost]
        public IActionResult UpdateVehicle(VehicleDTO vehicle)
        {
            if (ModelState.IsValid)
            {
                _vehicleService.UpdateVehicle(vehicle);
                return RedirectToAction("Index");
            }
            return View(vehicle);
        }

        public IActionResult Delete(Guid id)
        {
            var vehicle = _vehicleService.GetVehicleById(id);
            if (vehicle == null) return NotFound();
            return View(vehicle);
        }

        [HttpDelete]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _vehicleService.DeleteVehicle(id);
            return RedirectToAction("Index");
        }
    }
}
