using BusinessObjects.DTO;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Service;
using Group03_MVC.Attributes;

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

        // Tất cả user đã đăng nhập đều có thể xem danh sách xe
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm = "", string brandFilter = "", string sortBy = "", decimal? minPrice = null, decimal? maxPrice = null)
        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

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

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    vehicleDTOs = vehicleDTOs.Where(v => 
                        v.Name.ToLower().Contains(searchTerm.ToLower()) ||
                        v.Brand.ToLower().Contains(searchTerm.ToLower()) ||
                        v.Model.ToLower().Contains(searchTerm.ToLower())
                    ).ToList();
                }

                if (!string.IsNullOrEmpty(brandFilter))
                {
                    vehicleDTOs = vehicleDTOs.Where(v => v.Brand.ToLower() == brandFilter.ToLower()).ToList();
                }

                if (minPrice.HasValue)
                {
                    vehicleDTOs = vehicleDTOs.Where(v => v.Price >= minPrice.Value).ToList();
                }

                if (maxPrice.HasValue)
                {
                    vehicleDTOs = vehicleDTOs.Where(v => v.Price <= maxPrice.Value).ToList();
                }

                // Apply sorting
                switch (sortBy)
                {
                    case "name":
                        vehicleDTOs = vehicleDTOs.OrderBy(v => v.Name).ToList();
                        break;
                    case "price_asc":
                        vehicleDTOs = vehicleDTOs.OrderBy(v => v.Price).ToList();
                        break;
                    case "price_desc":
                        vehicleDTOs = vehicleDTOs.OrderByDescending(v => v.Price).ToList();
                        break;
                    case "year":
                        vehicleDTOs = vehicleDTOs.OrderByDescending(v => v.Year).ToList();
                        break;
                    default:
                        vehicleDTOs = vehicleDTOs.OrderBy(v => v.Name).ToList();
                        break;
                }

                // Get unique brands for filter dropdown
                ViewBag.Brands = vehicles.Select(v => v.Brand).Distinct().OrderBy(b => b).ToList();

                // Pass search parameters back to view
                ViewBag.SearchTerm = searchTerm;
                ViewBag.BrandFilter = brandFilter;
                ViewBag.SortBy = sortBy;
                ViewBag.MinPrice = minPrice;
                ViewBag.MaxPrice = maxPrice;
                ViewBag.UserRole = HttpContext.Session.GetString("Role");

                return View(vehicleDTOs);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error retrieving vehicles: {ex.Message}");
                return View(new List<VehicleDTO>());
            }
        }

        // Chỉ có evm_staff và dealer_manager mới có thể tạo xe mới
        [HttpGet]
        [RoleAuthorization("evm_staff", "dealer_manager")]
        public IActionResult Create()
        {
            return View(new VehicleDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorization("evm_staff", "dealer_manager")]
        public async Task<IActionResult> Create(VehicleDTO vehicle, List<IFormFile> imageFiles)
        {
            try
            {
                // Remove Images from ModelState validation
                ModelState.Remove("Images");

                // Manual validation
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
                
                // Additional validations
                if (vehicle.Year.HasValue && (vehicle.Year < 1900 || vehicle.Year > DateTime.Now.Year + 1))
                {
                    ModelState.AddModelError("Year", "Please enter a valid year.");
                }
                
                if (vehicle.StockQuantity.HasValue && vehicle.StockQuantity < 0)
                {
                    ModelState.AddModelError("StockQuantity", "Stock quantity cannot be negative.");
                }

                // Handle image uploads
                if (imageFiles != null && imageFiles.Count > 0)
                {
                    try
                    {
                        var imageUrls = await UploadImages(imageFiles);
                        vehicle.Images = string.Join(",", imageUrls);
                    }
                    catch (Exception imgEx)
                    {
                        ModelState.AddModelError("", $"Error uploading images: {imgEx.Message}");
                        return View(vehicle);
                    }
                }
                else
                {
                    vehicle.Images = "";
                }

                if (!ModelState.IsValid)
                {
                    return View(vehicle);
                }

                // Ensure all required fields are properly set
                vehicle.Name = vehicle.Name?.Trim();
                vehicle.Brand = vehicle.Brand?.Trim();
                vehicle.Model = vehicle.Model?.Trim();
                vehicle.Description = vehicle.Description?.Trim() ?? "";
                vehicle.Specifications = vehicle.Specifications?.Trim() ?? "";
                vehicle.StockQuantity ??= 0;

                var result = await _vehicleService.AddVehicle(vehicle);
                
                if (result)
                {
                    TempData["SuccessMessage"] = "Vehicle added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to add vehicle. Please check all fields and try again.");
                    return View(vehicle);
                }
            }
            catch (ArgumentException argEx)
            {
                ModelState.AddModelError("", $"Validation Error: {argEx.Message}");
                return View(vehicle);
            }
            catch (InvalidOperationException invEx)
            {
                ModelState.AddModelError("", invEx.Message);
                return View(vehicle);
            }
            catch (Exception ex)
            {
                // Log the full exception for debugging
                Console.WriteLine($"Create Vehicle Error: {ex}");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                return View(vehicle);
            }
        }

        // Tất cả user đã đăng nhập đều có thể xem chi tiết xe
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Account");
            }

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

                // Pass user role to view for conditional rendering
                ViewBag.UserRole = HttpContext.Session.GetString("Role");
                return View(vehicleDTO);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error retrieving vehicle: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // Chỉ có evm_staff và dealer_manager mới có thể chỉnh sửa xe
        [HttpGet]
        [RoleAuthorization("evm_staff", "dealer_manager")]
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
        [RoleAuthorization("evm_staff", "dealer_manager")]
        public async Task<IActionResult> Edit(VehicleDTO vehicle, List<IFormFile> imageFiles, List<string> removedImages)
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

                // Additional validations
                if (vehicle.Year.HasValue && (vehicle.Year < 1900 || vehicle.Year > DateTime.Now.Year + 1))
                {
                    ModelState.AddModelError("Year", "Please enter a valid year.");
                }
                
                if (vehicle.StockQuantity.HasValue && vehicle.StockQuantity < 0)
                {
                    ModelState.AddModelError("StockQuantity", "Stock quantity cannot be negative.");
                }

                // Get existing vehicle to preserve current images
                var existingVehicle = await _vehicleService.GetVehicleById(vehicle.Id);
                if (existingVehicle == null)
                {
                    return NotFound();
                }

                // Start with existing images
                var currentImages = new List<string>();
                if (!string.IsNullOrEmpty(existingVehicle.Images))
                {
                    currentImages = existingVehicle.Images.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                         .Select(img => img.Trim())
                                                         .Where(img => !string.IsNullOrEmpty(img))
                                                         .ToList();
                }

                // Remove deleted images
                if (removedImages != null && removedImages.Any())
                {
                    foreach (var removedImage in removedImages)
                    {
                        var trimmedRemovedImage = removedImage.Trim();
                        currentImages.RemoveAll(img => img.Equals(trimmedRemovedImage, StringComparison.OrdinalIgnoreCase));
                        
                        // Delete physical file
                        try
                        {
                            var fileName = Path.GetFileName(trimmedRemovedImage);
                            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "vehicles", fileName);
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log error but don't fail the update
                            Console.WriteLine($"Error deleting image file: {ex.Message}");
                        }
                    }
                }

                // Handle new image uploads
                if (imageFiles != null && imageFiles.Count > 0)
                {
                    try
                    {
                        var newImageUrls = await UploadImages(imageFiles);
                        currentImages.AddRange(newImageUrls);
                    }
                    catch (Exception imgEx)
                    {
                        ModelState.AddModelError("", $"Error uploading images: {imgEx.Message}");
                        return View(vehicle);
                    }
                }

                // Update vehicle with all images
                vehicle.Images = string.Join(",", currentImages.Where(img => !string.IsNullOrEmpty(img)));

                if (!ModelState.IsValid)
                {
                    return View(vehicle);
                }

                // Clean up data before update
                vehicle.Name = vehicle.Name?.Trim();
                vehicle.Brand = vehicle.Brand?.Trim();
                vehicle.Model = vehicle.Model?.Trim();
                vehicle.Description = vehicle.Description?.Trim() ?? "";
                vehicle.Specifications = vehicle.Specifications?.Trim() ?? "";
                vehicle.StockQuantity ??= 0;

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
                Console.WriteLine($"Edit Vehicle Error: {ex}");
                ModelState.AddModelError("", $"Error updating vehicle: {ex.Message}");
                return View(vehicle);
            }
        }

        // Chỉ có evm_staff và dealer_manager mới có thể xóa xe
        [HttpGet]
        [RoleAuthorization("evm_staff", "dealer_manager")]
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
        [RoleAuthorization("evm_staff", "dealer_manager")]
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
            
            Console.WriteLine($"Upload folder: {uploadsFolder}");
            Console.WriteLine($"Files to upload: {imageFiles?.Count ?? 0}");
            
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
                Console.WriteLine("Created upload directory");
            }

            if (imageFiles == null || imageFiles.Count == 0)
            {
                Console.WriteLine("No files to upload");
                return imageUrls;
            }

            foreach (var file in imageFiles)
            {
                if (file != null && file.Length > 0)
                {
                    Console.WriteLine($"Processing file: {file.FileName}, Size: {file.Length}");
                    
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        Console.WriteLine($"Skipping file with invalid extension: {fileExtension}");
                        continue;
                    }

                    var fileName = Guid.NewGuid().ToString() + fileExtension;
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    try
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var imageUrl = $"/images/vehicles/{fileName}";
                        imageUrls.Add(imageUrl);
                        Console.WriteLine($"Successfully uploaded: {imageUrl}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error uploading file {file.FileName}: {ex.Message}");
                        throw;
                    }
                }
            }

            Console.WriteLine($"Total images uploaded: {imageUrls.Count}");
            return imageUrls;
        }
    }
}