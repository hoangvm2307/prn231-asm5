using ChildTracking.Repositories.Base;
using ChildTracking.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildTracking.Repositories
{
    public class UserAccountRepository : GenericRepository<UserAccount>
    {
        public UserAccountRepository() { }
        public async Task<UserAccount> GetUserAccount(string userName, string password)
        {
            return await _context.UserAccounts.FirstOrDefaultAsync(c => c.UserName.Equals(userName) && c.Password.Equals(password) && c.IsActive == true);

        }
    }
}
