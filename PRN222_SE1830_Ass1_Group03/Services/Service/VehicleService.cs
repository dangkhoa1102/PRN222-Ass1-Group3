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

        public async Task<List<VehicleDto>> GetAvailableVehicles()
        {
            var vehicles = await _vehicleRepo.GetAvailableVehicles();
            return vehicles.Select(v => new VehicleDto
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
                StockQuantity = v.StockQuantity,
                CreatedAt = v.CreatedAt,
                IsActive = v.IsActive
            }).ToList();
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
                Specifications = vehicle.Specifications,
                    Images = vehicle.Images ?? "",
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
                throw new Exception("VehicleService ERROR: " + ex.Message, ex);
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
                throw new Exception("VehicleService ERROR: " + ex.Message, ex);
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
                throw new Exception("VehicleService ERROR: " + ex.Message, ex);
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
                throw new Exception("VehicleService ERROR: " + ex.Message, ex);
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

                var product = await _vehicleRepo.GetById(vehicle.Id);
                if (product == null)
                {
                    throw new KeyNotFoundException("Product not found");
                }

                product.Id = vehicle.Id;
                product.Name = vehicle.Name;
                product.Brand = vehicle.Brand;
                product.Model = vehicle.Model;
                product.Year = vehicle.Year;
                product.Price = vehicle.Price;
                product.Description = vehicle.Description;
                product.Specifications = vehicle.Specifications;
                product.Images = vehicle.Images ?? "";
                product.StockQuantity = vehicle.StockQuantity;
                product.IsActive = true;

                if (await _vehicleRepo.UpdateAsync(product))
        {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("VehicleService ERROR: " + ex.Message, ex);
            }
        }
    }
}
