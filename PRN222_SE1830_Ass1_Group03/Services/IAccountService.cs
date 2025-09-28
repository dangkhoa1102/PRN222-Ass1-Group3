using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IAccountService
    {
        public bool Register(User user);
        public Task<bool> CheckUserExists(string username, string email);
        public Task<User> Login(string username, string password);
    }
}
