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
        Task<Boolean>  Add(Vehicle vehicle);
        Task<Boolean>  UpdateAsync(Vehicle vehicle);
        Task<Boolean>  Delete(Guid id);
        Task<List<Vehicle>> GetAvailableVehicles();
    }

    public class VehicleRepository : IVehicleRepository
    {
        private readonly Vehicle_Dealer_ManagementContext _context;

        public VehicleRepository(Vehicle_Dealer_ManagementContext context)
        {
            _context = context;
        }

        public async Task<List<Vehicle>> GetAvailableVehicles()
        {
            return await _context.Vehicles
                                 .Where(v => v.IsActive == true && v.StockQuantity > 0)
                                 .OrderBy(v => v.Name)
                                 .ToListAsync();
        }

        public async Task<bool> Add(Vehicle vehicle)
        {
            try
            {
                _context.Vehicles.Add(vehicle);
                if(await _context.SaveChangesAsync() > 0)
                {
                    return true;
                }
                return false;
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
                    if (await _context.SaveChangesAsync() > 0)
                    {
                        return true;
                    }
                }
                return false;
                
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("VehicleRepo ERROR: " + ex.Message);
            }
        }

        public async Task<List<Vehicle>> GetAll()
        {
            try
            {
                return await _context.Vehicles.ToListAsync();
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
                Vehicle? vehicle = await _context.Vehicles.FirstOrDefaultAsync(x => x.Id == id);
                return vehicle;
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
                if(await _context.SaveChangesAsync() > 0) 
                {
                    return true; 
            }
            return false;
        }
            catch (Exception ex)
            {
                throw new NotImplementedException("VehicleRepo ERROR: " + ex.Message);
            }
        }

        public async Task<bool> UpdateStock(Guid vehicleId, int quantity)
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
    }
}
