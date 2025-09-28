using BusinessObjects.DTO;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Service;

namespace Group03_MVC.Controllers
{
    public class VehicleController : Controller
    {
        private readonly IVehicleService _vehicleService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VehicleController(IVehicleService vehicleService, IWebHostEnvironment webHostEnvironment)
        {
            _vehicleService = vehicleService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var vehicles = await _vehicleService.GetAllVehicles();
                var vehicleDTOs = vehicles.Select(v => new VehicleDTO
                {
                    Id = v.Id,
                    Name = v.Name,
                    Brand = v.Brand,
                    Model = v.Model,
                    Year = v.Year,
                    Price = v.Price,
                    Description = v.Description,
                    Specifications = v.Specifications,
                    Images = v.Images,
                    StockQuantity = v.StockQuantity
                }).ToList();
                return View(vehicleDTOs);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error retrieving vehicles: {ex.Message}");
                return View(new List<VehicleDTO>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new VehicleDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VehicleDTO vehicle, List<IFormFile> imageFiles)
        {
            try
            {
                // Xóa validation error cho Images ngay từ đầu
                ModelState.Remove("Images");

                // Validate required fields
                if (string.IsNullOrWhiteSpace(vehicle.Name))
                {
                    ModelState.AddModelError("Name", "Vehicle name is required.");
                }
                if (string.IsNullOrWhiteSpace(vehicle.Brand))
                {
                    ModelState.AddModelError("Brand", "Brand is required.");
                }
                if (string.IsNullOrWhiteSpace(vehicle.Model))
                {
                    ModelState.AddModelError("Model", "Model is required.");
                }
                if (vehicle.Price <= 0)
                {
                    ModelState.AddModelError("Price", "Price must be greater than 0.");
                }

                // Xử lý ảnh
                if (imageFiles != null && imageFiles.Count > 0)
                {
                    var imageUrls = await UploadImages(imageFiles);
                    vehicle.Images = string.Join(",", imageUrls);
                }
                else
                {
                    vehicle.Images = "";
                }

                if (!ModelState.IsValid)
                {
                    return View(vehicle);
                }

                var result = await _vehicleService.AddVehicle(vehicle);
                
                if (result)
                {
                    TempData["SuccessMessage"] = "Vehicle added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to add vehicle. Please try again.");
                    return View(vehicle);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error adding vehicle: {ex.Message}");
                return View(vehicle);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
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
        public async Task<IActionResult> Edit(VehicleDTO vehicle, List<IFormFile> imageFiles)
        {
            try
            {
                ModelState.Remove("Images");

                // Validate required fields
                if (string.IsNullOrWhiteSpace(vehicle.Name))
                {
                    ModelState.AddModelError("Name", "Vehicle name is required.");
                }
                if (string.IsNullOrWhiteSpace(vehicle.Brand))
                {
                    ModelState.AddModelError("Brand", "Brand is required.");
                }
                if (string.IsNullOrWhiteSpace(vehicle.Model))
                {
                    ModelState.AddModelError("Model", "Model is required.");
                }
                if (vehicle.Price <= 0)
                {
                    ModelState.AddModelError("Price", "Price must be greater than 0.");
                }

                // Handle image uploads
                if (imageFiles != null && imageFiles.Count > 0)
                {
                    var imageUrls = await UploadImages(imageFiles);
                    if (!string.IsNullOrEmpty(vehicle.Images))
                    {
                        vehicle.Images += "," + string.Join(",", imageUrls);
                    }
                    else
                    {
                        vehicle.Images = string.Join(",", imageUrls);
                    }
                }

                if (!ModelState.IsValid)
                {
                    return View(vehicle);
                }

                var result = await _vehicleService.UpdateVehicle(vehicle);
                
                if (result)
                {
                    TempData["SuccessMessage"] = "Vehicle updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update vehicle. Please try again.");
                    return View(vehicle);
                }
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
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var result = await _vehicleService.DeleteVehicle(id);
                
                if (result)
                {
                    TempData["SuccessMessage"] = "Vehicle deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete vehicle. Please try again.";
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting vehicle: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<List<string>> UploadImages(List<IFormFile> imageFiles)
        {
            var imageUrls = new List<string>();
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "vehicles");
            
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            foreach (var file in imageFiles)
            {
                if (file != null && file.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        continue;
                    }

                    var fileName = Guid.NewGuid().ToString() + fileExtension;
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    imageUrls.Add($"/images/vehicles/{fileName}");
                }
            }

            return imageUrls;
        }
    }
}