using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        public readonly Vehicle_Dealer_ManagementContext _context;
        public OrderRepository(Vehicle_Dealer_ManagementContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrderByUserId(Guid id)
        {
            return await _context.Orders.Include(o => o.Vehicle).Where(o => o.CustomerId == id).ToListAsync();
        }
    }
}
