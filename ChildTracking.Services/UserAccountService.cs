using ChildTracking.Repositories;
using ChildTracking.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildTracking.Services
{
    public class UserAccountService
    {
        private readonly UserAccountRepository _repository;
        public UserAccountService()
        {
            _repository = new UserAccountRepository();
        }

        public async Task<UserAccount> Authenticate(string userName, string password)
        {
            return await _repository.GetUserAccount(userName, password);
        }
    }
}
