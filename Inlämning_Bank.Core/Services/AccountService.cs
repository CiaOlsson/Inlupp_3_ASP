using Inlämning_Bank.Core.Interfaces;
using Inlämning_Bank.Data.Interfaces;
using Inlämning_Bank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämning_Bank.Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _accountRepo;

        public AccountService(IAccountRepo accountRepo)
        {
            _accountRepo = accountRepo;
        }

        public async Task<int> AddAccount(Account account)
        {
            account.Frequency = "Monthly";
            account.Created = DateOnly.FromDateTime(DateTime.UtcNow);
            account.Balance = 0;

            try
            {
                int returnedAccountId = await _accountRepo.OpenNewAccount(account);
                return returnedAccountId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Kunde inte öppna konto \n{ex.Message}");
            }
        }
    }
}
