using Inlämning_Bank.Data.Interfaces;
using Inlämning_Bank.Domain.DTO;
using Inlämning_Bank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämning_Bank.Core.Interfaces
{
    public interface IAccountService
    {

        Task AddAccount(int accountType, int customerId);
        Task<Account> GetAccountById(int accountId);
        Task<List<CustomerAccountDTO>> GetCustomerAccounts(System.Security.Claims.ClaimsPrincipal user);
        Task UpdateAccount(Account account);
    }
}
