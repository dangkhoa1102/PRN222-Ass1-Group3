using BusinessObjects.DTO;
using BusinessObjects.Models;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public interface IVehicleService
    {
        Task<List<Vehicle>> GetAllVehicles();
        Task<Vehicle> GetVehicleById(Guid id);
        Task<bool> AddVehicle(VehicleDTO vehicle);
        Task<bool> UpdateVehicle(VehicleDTO vehicle);
        Task<bool> DeleteVehicle(Guid id);
    }

    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepo;
        
        public VehicleService(IVehicleRepository vehicleRepo)
        {
            _vehicleRepo = vehicleRepo;
        }

        public async Task<bool> AddVehicle(VehicleDTO vehicleDTO)
        {
            try
            {
                if (vehicleDTO == null)
                {
                    throw new ArgumentNullException(nameof(vehicleDTO), "VehicleDTO cannot be null.");
                }

                // Validate DTO
                ValidateVehicleDTO(vehicleDTO);

                var vehicle = new Vehicle
                {
                    Id = Guid.NewGuid(),
                    Name = vehicleDTO.Name?.Trim(),
                    Brand = vehicleDTO.Brand?.Trim(),
                    Model = vehicleDTO.Model?.Trim(),
                    Year = vehicleDTO.Year,
                    Price = vehicleDTO.Price,
                    Description = vehicleDTO.Description?.Trim() ?? "",
                    Specifications = vehicleDTO.Specifications?.Trim() ?? "",
                    Images = vehicleDTO.Images?.Trim() ?? "",
                    StockQuantity = vehicleDTO.StockQuantity ?? 0,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                return await _vehicleRepo.Add(vehicle);
            }
            catch (Exception ex)
            {
                // Log the actual exception for debugging
                Console.WriteLine($"VehicleService AddVehicle Error: {ex}");
                throw new Exception($"VehicleService ERROR: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteVehicle(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("Invalid vehicle ID");
                }

                return await _vehicleRepo.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"VehicleService ERROR: {ex.Message}", ex);
            }
        }

        public async Task<List<Vehicle>> GetAllVehicles()
        {
            try
            {
                return await _vehicleRepo.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception($"VehicleService ERROR: {ex.Message}", ex);
            }
        }

        public async Task<Vehicle> GetVehicleById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("Invalid vehicle ID");
                }

                var vehicle = await _vehicleRepo.GetById(id);
                if (vehicle == null)
                {
                    throw new KeyNotFoundException("Vehicle not found");
                }
                return vehicle;
            }
            catch (Exception ex)
            {
                throw new Exception($"VehicleService ERROR: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateVehicle(VehicleDTO vehicleDTO)
        {
            try
            {
                if (vehicleDTO == null)
                {
                    throw new ArgumentNullException(nameof(vehicleDTO), "VehicleDTO cannot be null.");
                }

                if (vehicleDTO.Id == Guid.Empty)
                {
                    throw new ArgumentException("Invalid vehicle ID");
                }

                // Validate DTO
                ValidateVehicleDTO(vehicleDTO);

                var existingVehicle = await _vehicleRepo.GetById(vehicleDTO.Id);
                if (existingVehicle == null)
                {
                    throw new KeyNotFoundException("Vehicle not found");
                }

                // Update vehicle properties
                existingVehicle.Name = vehicleDTO.Name?.Trim();
                existingVehicle.Brand = vehicleDTO.Brand?.Trim();
                existingVehicle.Model = vehicleDTO.Model?.Trim();
                existingVehicle.Year = vehicleDTO.Year;
                existingVehicle.Price = vehicleDTO.Price;
                existingVehicle.Description = vehicleDTO.Description?.Trim() ?? "";
                existingVehicle.Specifications = vehicleDTO.Specifications?.Trim() ?? "";
                existingVehicle.Images = vehicleDTO.Images?.Trim() ?? "";
                existingVehicle.StockQuantity = vehicleDTO.StockQuantity ?? 0;
                existingVehicle.IsActive = true;

                return await _vehicleRepo.UpdateAsync(existingVehicle);
            }
            catch (Exception ex)
            {
                throw new Exception($"VehicleService ERROR: {ex.Message}", ex);
            }
        }

        private void ValidateVehicleDTO(VehicleDTO vehicleDTO)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(vehicleDTO.Name))
                errors.Add("Vehicle name is required");

            if (string.IsNullOrWhiteSpace(vehicleDTO.Brand))
                errors.Add("Vehicle brand is required");

            if (string.IsNullOrWhiteSpace(vehicleDTO.Model))
                errors.Add("Vehicle model is required");

            if (vehicleDTO.Price <= 0)
                errors.Add("Vehicle price must be greater than 0");

            if (vehicleDTO.Year.HasValue && (vehicleDTO.Year < 1900 || vehicleDTO.Year > DateTime.Now.Year + 1))
                errors.Add("Invalid vehicle year");

            if (vehicleDTO.StockQuantity.HasValue && vehicleDTO.StockQuantity < 0)
                errors.Add("Stock quantity cannot be negative");

            if (errors.Any())
            {
                throw new ArgumentException($"Validation failed: {string.Join(", ", errors)}");
            }
        }
    }
}
