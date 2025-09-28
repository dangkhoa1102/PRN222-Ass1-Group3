using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class OrderRepository
    {
        private readonly Vehicle_Dealer_ManagementContext _context;

        public OrderRepository(Vehicle_Dealer_ManagementContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAll()
        {
            return await _context.Orders
                                 .Include(o => o.Customer)
                                 .Include(o => o.Vehicle)
                                 .ToListAsync();
        }

        public async Task<Order?> GetById(Guid id)
        {
            return await _context.Orders
                                 .Include(o => o.Customer)
                                 .Include(o => o.Vehicle)
                                 .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<bool> Add(Order order)
        {
            _context.Orders.Add(order);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(Order order)
        {
            _context.Orders.Update(order);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }
    }
}
