using BusinessObjects.Models;
using DataAccessLayer;
using BusinessObjects.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;

namespace Services.Service
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRep;
        
        public AccountService(IAccountRepository accountRep)
        {
            _accountRep = accountRep;
        }

        public async Task<bool> CheckUserExists(string username, string email)
        {
            try
            {
                return await _accountRep.CheckUserExists(username, email);
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
                return await _accountRep.GetUserById(id);
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
                return await _accountRep.Login(username, password);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> Register(User user)
        {
            try
            {
                if (await CheckUserExists(user.Username, user.Email))
                {
                    return false;
                }

                return await Task.FromResult( await _accountRep.AddUser(user));
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateUser(UserDTO userDTO)
        {
            try
            {
                var existingUser = await _accountRep.GetUserById(userDTO.Id);
                if (existingUser == null)
                {
                    return false;
                }

                // Keep existing values that shouldn't be updated
                var username = existingUser.Username;
                var role = existingUser.Role;
                var password = existingUser.Password;
                var dealerId = existingUser.DealerId;
                var createdAt = existingUser.CreatedAt;
                var isActive = existingUser.IsActive;

                // Update allowed properties
                existingUser.Email = userDTO.Email;
                existingUser.FullName = userDTO.FullName;
                existingUser.Phone = userDTO.Phone;

                // Restore protected properties
                existingUser.Username = username;
                existingUser.Role = role;
                existingUser.Password = password;
                existingUser.DealerId = dealerId;
                existingUser.CreatedAt = createdAt;
                existingUser.IsActive = isActive;

                return await Task.FromResult(await _accountRep.UpdateUser(existingUser));
            }
            catch
            {
                return false;
            }
        }
    }
}
