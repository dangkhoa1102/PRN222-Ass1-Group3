using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface IVehicleRepository
    {
        Task<List<Vehicle>> GetAll();
        Task<Vehicle?> GetById(Guid id);
        Task<Boolean> Add(Vehicle vehicle);
        Task<Boolean> UpdateAsync(Vehicle vehicle);
        Task<Boolean> Delete(Guid id);
    }

    public class VehicleRepository : IVehicleRepository
    {
        private readonly Vehicle_Dealer_ManagementContext _context;

        public VehicleRepository(Vehicle_Dealer_ManagementContext context)
        {
            _context = context;
        }

        public async Task<bool> Add(Vehicle vehicle)
        {
            try
            {
                // Validate vehicle data before adding
                if (vehicle == null)
                    throw new ArgumentNullException(nameof(vehicle));

                if (string.IsNullOrWhiteSpace(vehicle.Name))
                    throw new ArgumentException("Vehicle name is required");

                if (string.IsNullOrWhiteSpace(vehicle.Brand))
                    throw new ArgumentException("Vehicle brand is required");

                if (string.IsNullOrWhiteSpace(vehicle.Model))
                    throw new ArgumentException("Vehicle model is required");

                if (vehicle.Price <= 0)
                    throw new ArgumentException("Vehicle price must be greater than 0");

                // Ensure required fields are not null
                vehicle.Name = vehicle.Name?.Trim() ?? throw new ArgumentNullException(nameof(vehicle.Name));
                vehicle.Brand = vehicle.Brand?.Trim() ?? throw new ArgumentNullException(nameof(vehicle.Brand));
                vehicle.Model = vehicle.Model?.Trim() ?? throw new ArgumentNullException(nameof(vehicle.Model));
                vehicle.Description = vehicle.Description?.Trim() ?? "";
                vehicle.Specifications = vehicle.Specifications?.Trim() ?? "";
                vehicle.Images = vehicle.Images?.Trim() ?? "";

                // Set default values if not provided
                if (vehicle.Id == Guid.Empty)
                    vehicle.Id = Guid.NewGuid();

                vehicle.CreatedAt ??= DateTime.UtcNow;
                vehicle.IsActive ??= true;
                vehicle.StockQuantity ??= 0;

                // Check for duplicate vehicle (optional)
                var existingVehicle = await _context.Vehicles
                    .AnyAsync(v => v.Name == vehicle.Name && v.Brand == vehicle.Brand && v.Model == vehicle.Model);

                if (existingVehicle)
                {
                    throw new InvalidOperationException("A vehicle with the same name, brand, and model already exists");
                }

                // Add vehicle
                await _context.Vehicles.AddAsync(vehicle);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database-specific errors
                var innerException = dbEx.InnerException?.Message ?? dbEx.Message;
                throw new Exception($"VehicleRepo ERROR: Database error occurred while adding vehicle: {innerException}", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"VehicleRepo ERROR: {ex.Message}", ex);
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new ArgumentException("Invalid vehicle ID");

                var vehicle = await _context.Vehicles.FindAsync(id);
                if (vehicle == null)
                    return false; // Vehicle not found, consider it as successful deletion

                _context.Vehicles.Remove(vehicle);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (DbUpdateException dbEx)
            {
                var innerException = dbEx.InnerException?.Message ?? dbEx.Message;
                throw new Exception($"VehicleRepo ERROR: Database error occurred while deleting vehicle: {innerException}", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"VehicleRepo ERROR: {ex.Message}", ex);
            }
        }

        public async Task<List<Vehicle>> GetAll()
        {
            try
            {
                return await _context.Vehicles
                    .Where(v => v.IsActive == true)
                    .OrderBy(v => v.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"VehicleRepo ERROR: {ex.Message}", ex);
            }
        }

        public async Task<Vehicle?> GetById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return null;

                return await _context.Vehicles
                    .FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);
            }
            catch (Exception ex)
            {
                throw new Exception($"VehicleRepo ERROR: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateAsync(Vehicle vehicle)
        {
            try
            {
                if (vehicle == null)
                    throw new ArgumentNullException(nameof(vehicle));

                if (vehicle.Id == Guid.Empty)
                    throw new ArgumentException("Invalid vehicle ID");

                // Validate required fields
                if (string.IsNullOrWhiteSpace(vehicle.Name))
                    throw new ArgumentException("Vehicle name is required");

                if (string.IsNullOrWhiteSpace(vehicle.Brand))
                    throw new ArgumentException("Vehicle brand is required");

                if (string.IsNullOrWhiteSpace(vehicle.Model))
                    throw new ArgumentException("Vehicle model is required");

                if (vehicle.Price <= 0)
                    throw new ArgumentException("Vehicle price must be greater than 0");

                // Check if vehicle exists
                var existingVehicle = await _context.Vehicles.FindAsync(vehicle.Id);
                if (existingVehicle == null)
                    throw new KeyNotFoundException("Vehicle not found");

                // Update properties
                existingVehicle.Name = vehicle.Name.Trim();
                existingVehicle.Brand = vehicle.Brand.Trim();
                existingVehicle.Model = vehicle.Model.Trim();
                existingVehicle.Year = vehicle.Year;
                existingVehicle.Price = vehicle.Price;
                existingVehicle.Description = vehicle.Description?.Trim() ?? "";
                existingVehicle.Specifications = vehicle.Specifications?.Trim() ?? "";
                existingVehicle.Images = vehicle.Images?.Trim() ?? "";
                existingVehicle.StockQuantity = vehicle.StockQuantity;
                existingVehicle.IsActive = vehicle.IsActive;

                // Keep original CreatedAt
                // existingVehicle.CreatedAt remains unchanged

                _context.Vehicles.Update(existingVehicle);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (DbUpdateException dbEx)
            {
                var innerException = dbEx.InnerException?.Message ?? dbEx.Message;
                throw new Exception($"VehicleRepo ERROR: Database error occurred while updating vehicle: {innerException}", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"VehicleRepo ERROR: {ex.Message}", ex);
            }
        }
    }
}
