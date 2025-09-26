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

        public async Task<bool> AddVehicle(VehicleDTO vehicle)
        {
            try
            {
                if (vehicle == null)
                {
                    throw new ArgumentNullException(nameof(vehicle), "VehicleDTO cannot be null.");
                }

                var x = new Vehicle
                {
                    Id = Guid.NewGuid(),
                    Name = vehicle.Name,
                    Brand = vehicle.Brand,
                    Model = vehicle.Model,
                    Year = vehicle.Year,
                    Price = vehicle.Price,
                    Description = vehicle.Description,
                    Specifications  = vehicle.Specifications,
                    Images = vehicle.Images,
                    StockQuantity = vehicle.StockQuantity,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                if (await _vehicleRepo.Add(x))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("VehicleService ERROR: " + ex.Message);
            }
        }

        public async Task<bool> DeleteVehicle(Guid id)
        {
            try
            {
                if (await _vehicleRepo.Delete(id))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("VehicleService ERROR: " + ex.Message);
            }
        }

        public async Task<List<Vehicle>> GetAllVehicles()
        {
            try
            {
                var x = await _vehicleRepo.GetAll();
                if (x == null)
                    throw new KeyNotFoundException("Product not found");
                return x;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("VehicleService ERROR: " + ex.Message);
            }
        }

        public async Task<Vehicle> GetVehicleById(Guid id)
        {
            try
            {
                var x = await _vehicleRepo.GetById(id);
                if (x == null)
                {
                    throw new KeyNotFoundException("Product not found");
                }
                return x;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("VehicleService ERROR: " + ex.Message);
            }
        }

        public async Task<bool> UpdateVehicle(VehicleDTO vehicle)
        {
            try
            {
                if (vehicle == null)
                {
                    throw new ArgumentNullException(nameof(vehicle), "VehicleDTO cannot be null.");
                }

                if (string.IsNullOrWhiteSpace(vehicle.Name))
                {
                    throw new ArgumentException("Product name is required");
                }
                if (vehicle.Price < 0)
                {
                    throw new ArgumentException("Price cannot be negative");
                }
                var product = await _vehicleRepo.GetById(vehicle.Id);
                if (product == null)
                {
                    throw new KeyNotFoundException("Product not found");
                }

                var x = new Vehicle
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
                    StockQuantity = vehicle.StockQuantity,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                if (await _vehicleRepo.Update(x))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("VehicleService ERROR: " + ex.Message);
            }
        }
    }
}
