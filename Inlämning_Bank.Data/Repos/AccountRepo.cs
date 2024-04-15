using Inlämning_Bank.Data.Contexts;
using Inlämning_Bank.Data.Interfaces;
using Inlämning_Bank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
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
        public async Task<Account> GetAccountById(int accountId)
        {
            var account = _context.Accounts.SingleOrDefault(a => a.AccountId == accountId);
            return account;
        }

        //public async Task InsertMoneyOnAccount(int accountId, decimal amount)
        //{
        //    var account = _context.Accounts.SingleOrDefault(a=>a.AccountId == accountId);
        //    account.Balance += amount;
        //    _context.SaveChanges();
        //}

        public async Task<int> OpenNewAccount(Account account)
        {
            _context.Accounts.Add(account);
            _context.SaveChanges();

            return account.AccountId;
        }

        public async Task UpdateAccount(Account account)
        {
            _context.Entry(account).State = EntityState.Modified;
        }
    }
}
