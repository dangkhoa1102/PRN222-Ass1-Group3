using BusinessObjects.DTO;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public interface IAccountService
    {
        Task<bool> Register(User user);
        Task<bool> CheckUserExists(string username, string email);
        Task<User> Login(string username, string password);
        Task<User> GetUserById(Guid id);
        Task<bool> UpdateUser(UserDTO userDTO);
    }
}
