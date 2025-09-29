using BusinessObjects.DTO;
using BusinessObjects.Models;
using DataAccessLayer.Repositories;

namespace Services.Service
{
    public class VehicleService
    {
        private readonly VehicleRepository _vehicleRepository;

        public VehicleService(VehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public async Task<List<VehicleDto>> GetAllVehicles()
        {
            var vehicles = await _vehicleRepository.GetAll();
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

        public async Task<VehicleDetailDto?> GetVehicleById(Guid id)
        {
            var vehicle = await _vehicleRepository.GetById(id);
            if (vehicle == null) return null;

            return new VehicleDetailDto
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
                CreatedAt = vehicle.CreatedAt,
                IsActive = vehicle.IsActive
            };
        }

        public async Task<List<VehicleDto>> GetAvailableVehicles()
        {
            var vehicles = await _vehicleRepository.GetAvailableVehicles();
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
    }
}
