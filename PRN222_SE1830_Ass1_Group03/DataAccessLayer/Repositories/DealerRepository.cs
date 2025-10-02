using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public interface IDealerRepository
    {
        Task<List<Dealer>> GetAll();
        Task<Dealer?> GetById(Guid id);
        Task<bool> Add(Dealer dealer);
        Task<bool> Update(Dealer dealer);
        Task<bool> Delete(Guid id);
    }

    public class DealerRepository : IDealerRepository
    {
        private readonly Vehicle_Dealer_ManagementContext _context;

        public DealerRepository(Vehicle_Dealer_ManagementContext context)
        {
            _context = context;
        }

        public async Task<List<Dealer>> GetAll()
        {
            // Chỉ lấy dealer đang active, sắp xếp theo tên
            return await _context.Dealers
                .Where(d => d.IsActive == true)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<Dealer?> GetById(Guid id)
        {
            if (id == Guid.Empty) return null;
            return await _context.Dealers
                .FirstOrDefaultAsync(d => d.Id == id && d.IsActive == true);
        }

        public async Task<bool> Add(Dealer dealer)
        {
            _context.Dealers.Add(dealer);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(Dealer dealer)
        {
            _context.Dealers.Update(dealer);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(Guid id)
        {
            var dealer = await _context.Dealers.FindAsync(id);
            if (dealer == null) return false;
            _context.Dealers.Remove(dealer);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
