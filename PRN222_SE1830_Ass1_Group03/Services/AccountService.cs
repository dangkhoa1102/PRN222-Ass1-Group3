using BusinessObjects.Models;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AccountService : IAccountService
    {
        private readonly AccountDao _accountDao;
        public AccountService(AccountDao accountDao)
        {
            _accountDao = accountDao;
        }

        public async Task<bool> CheckUserExists(string username, string email)
        {
            return await _accountDao.CheckUserExists(username, email);
        }

        public async Task<User> Login(string username, string password)
        {
           return await _accountDao.Login(username, password); 
        }

        public bool Register(User user)
        {
          bool check = _accountDao.AddUser(user);
           if(check) return true;
              return false;
        }
    }
}
