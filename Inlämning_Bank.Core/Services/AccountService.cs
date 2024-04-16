using AutoMapper;
using Inlämning_Bank.Core.Interfaces;
using Inlämning_Bank.Data.Interfaces;
using Inlämning_Bank.Domain.DTO;
using Inlämning_Bank.Domain.Entities;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Inlämning_Bank.Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _repo;
        private readonly IDispositionService _dispositionService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public AccountService(IAccountRepo accountRepo, IDispositionService dispositionService, IMapper mapper, IUserService userService)
        {
            _repo = accountRepo;
            _dispositionService = dispositionService;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<Account> GetAccountById(int accountId)
        {
            return await _repo.GetAccountById(accountId);
        }

        public async Task<int> AddAccount(int accountType, int customerId)
        {
            Account account = new Account() 
            {
                AccountTypesId = accountType,
                Frequency = "Monthly",
                Created = DateOnly.FromDateTime(DateTime.Now),
                Balance = 0,
            };

            try
            {
                int returnedAccountId = await _repo.OpenNewAccount(account);

                await _dispositionService.AddDisposition(customerId, returnedAccountId);

                return returnedAccountId;

            }
            catch (Exception ex)
            {
                throw new Exception($"Kunde inte öppna konto \n{ex.Message}");
            }
        }

        public async Task<List<CustomerAccountDTO>> GetCustomerAccounts(ClaimsPrincipal user)
        {
            var customerId = await _userService.RetrieveCustomerId(user);

            var dispositionList = await _dispositionService.GetDispositionsByCustomerId(customerId);

            List<CustomerAccountDTO> accountList = new List<CustomerAccountDTO>();

            foreach (var disposition in dispositionList)
            {
                var customerAccount = _mapper.Map<CustomerAccountDTO>(disposition);
                accountList.Add(customerAccount);
            }
            
            return accountList;
        }

        public async Task UpdateAccount(Account account)
        {
            await _repo.UpdateAccount(account);
        }
    }
}
