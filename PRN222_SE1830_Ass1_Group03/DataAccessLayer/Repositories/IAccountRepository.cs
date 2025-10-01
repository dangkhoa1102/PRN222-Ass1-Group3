using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface IAccountRepository
    {
        Task<bool> CheckUserExists(string username, string email);
        Task<User> Login(string username, string password);
        Task<bool> AddUser(User user);
        Task<User> GetUserById(Guid id);
        Task<bool> UpdateUser(User user);
    }
}
