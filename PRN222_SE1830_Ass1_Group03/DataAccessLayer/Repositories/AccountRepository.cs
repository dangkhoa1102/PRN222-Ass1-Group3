using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly Vehicle_Dealer_ManagementContext _context;

        public AccountRepository(Vehicle_Dealer_ManagementContext context)
        {
            _context = context;
        }

        public async Task<bool> AddUser(User user)
        {
            try
            {
                // Ensure ID is generated if not set
                if (user.Id == Guid.Empty)
                {
                    user.Id = Guid.NewGuid();
                }

                // Set default values if not provided
                user.CreatedAt ??= DateTime.UtcNow;
                user.IsActive ??= true;

                // Detach any existing entity with the same ID
                var existingEntity = _context.Entry(user).Entity;
                if (_context.Entry(user).State != EntityState.Detached)
                {
                    _context.Entry(user).State = EntityState.Detached;
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error adding user: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CheckUserExists(string username, string email)
        {
            try
            {
                var lowerUsername = username.ToLower();
                var lowerEmail = email.ToLower();
                return await _context.Users
                    .AnyAsync(u => u.Username.ToLower() == lowerUsername || u.Email.ToLower() == lowerEmail);
            }
            catch
            {
                return true; // Return true on error to prevent registration
            }
        }

        public async Task<User> GetUserById(Guid id)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Dealer)
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public async Task<User> Login(string username, string password)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Dealer)
                    .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateUser(User user)
        {
            try
            {
                var existingUser = _context.Users.Find(user.Id);
                if (existingUser == null)
                {
                    return false;
                }

                // Update only the necessary fields
                _context.Entry(existingUser).CurrentValues.SetValues(user);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error updating user: {ex.Message}");
                return false;
            }
        }
    }
}
