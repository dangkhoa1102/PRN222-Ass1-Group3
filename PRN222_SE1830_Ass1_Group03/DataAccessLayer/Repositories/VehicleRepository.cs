using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class VehicleRepository
    {
        private readonly Vehicle_Dealer_ManagementContext _context;

        public VehicleRepository(Vehicle_Dealer_ManagementContext context)
        {
            _context = context;
        }

        public async Task<List<Vehicle>> GetAll()
        {
            return await _context.Vehicles
                                 .Where(v => v.IsActive == true)
                                 .OrderBy(v => v.Name)
                                 .ToListAsync();
        }

        public async Task<Vehicle?> GetById(Guid id)
        {
            return await _context.Vehicles
                                 .FirstOrDefaultAsync(v => v.Id == id && v.IsActive == true);
        }

        public async Task<List<Vehicle>> GetAvailableVehicles()
        {
            return await _context.Vehicles
                                 .Where(v => v.IsActive == true && v.StockQuantity > 0)
                                 .OrderBy(v => v.Name)
                                 .ToListAsync();
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


