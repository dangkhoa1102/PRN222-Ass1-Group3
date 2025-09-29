using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class AccountDao
    {
        private readonly Vehicle_Dealer_ManagementContext _context;
        public AccountDao(Vehicle_Dealer_ManagementContext context)
        {
            _context = context;
        }

     

        public async Task<bool> CheckUserExists(string username, string email)
        {
            var lowerUsername = username.ToLower();
            var lowerEmail = email.ToLower();
            return await _context.Users
                .AnyAsync(u => u.Username.ToLower() == lowerUsername || u.Email.ToLower() == lowerEmail);
        }

        public async Task<User> Login(string username, string password)
        {
            return  await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }
        public bool AddUser(User user)
        {
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
