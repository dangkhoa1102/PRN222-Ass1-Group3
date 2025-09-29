using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public interface IVehicleRepository
    {
        Task<List<Vehicle>> GetAll();
        Task<Vehicle?> GetById(Guid id);
        Task<bool> Add(Vehicle vehicle);
        Task<bool> UpdateAsync(Vehicle vehicle);
        Task<bool> Delete(Guid id);
        Task<List<Vehicle>> GetAvailableVehicles();
        Task<bool> UpdateStock(Guid vehicleId, int quantity);
    }

    public class VehicleRepository : IVehicleRepository
    {
        private readonly Vehicle_Dealer_ManagementContext _context;

        public VehicleRepository(Vehicle_Dealer_ManagementContext context)
        {
            _context = context;
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
                throw new NotImplementedException("VehicleRepo ERROR: " + ex.Message);
            }
        }

        public async Task<Vehicle?> GetById(Guid id)
        {
            try
            {
                return await _context.Vehicles
                                     .FirstOrDefaultAsync(v => v.Id == id && v.IsActive == true);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("VehicleRepo ERROR: " + ex.Message);
            }
        }

        public async Task<bool> Add(Vehicle vehicle)
        {
            try
            {
                _context.Vehicles.Add(vehicle);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("VehicleRepo ERROR: " + ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(Vehicle vehicle)
        {
            try
            {
                _context.Vehicles.Update(vehicle);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("VehicleRepo ERROR: " + ex.Message);
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            try
            {
                var vehicle = await _context.Vehicles.FindAsync(id);
                if (vehicle != null)
                {
                    _context.Vehicles.Remove(vehicle);
                    return await _context.SaveChangesAsync() > 0;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("VehicleRepo ERROR: " + ex.Message);
            }
        }

        public async Task<List<Vehicle>> GetAvailableVehicles()
        {
            try
            {
                return await _context.Vehicles
                                     .Where(v => v.IsActive == true && v.StockQuantity > 0)
                                     .OrderBy(v => v.Name)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("VehicleRepo ERROR: " + ex.Message);
            }
        }

        public async Task<bool> UpdateStock(Guid vehicleId, int quantity)
        {
            try
            {
                var vehicle = await _context.Vehicles.FindAsync(vehicleId);
                if (vehicle != null)
                {
                    vehicle.StockQuantity = quantity;
                    _context.Vehicles.Update(vehicle);
                    return await _context.SaveChangesAsync() > 0;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("VehicleRepo ERROR: " + ex.Message);
            }
        }
    }
}