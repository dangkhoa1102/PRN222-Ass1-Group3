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
        Task<Boolean>  Update(Vehicle vehicle);
        Task<Boolean>  Delete(Guid id);
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
                var vehicle = _context.Vehicles.Find(id);
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

        public async Task<bool> Update(Vehicle vehicle)
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
    }
}
