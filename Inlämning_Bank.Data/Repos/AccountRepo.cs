using Inlämning_Bank.Data.Contexts;
using Inlämning_Bank.Data.Interfaces;
using Inlämning_Bank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämning_Bank.Data.Repos
{
    public class AccountRepo : IAccountRepo
    {
        private readonly BankAppDataContext _context;

        public AccountRepo(BankAppDataContext context)
        {
            _context = context;
        }

        public async Task<int> OpenNewAccount(Account account)
        {
            _context.Accounts.Add(account);
            _context.SaveChanges();

            return account.AccountId;
        }
    }
}
