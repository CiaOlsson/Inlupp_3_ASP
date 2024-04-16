using Inlämning_Bank.Core.Interfaces;
using Inlämning_Bank.Data.Interfaces;
using Inlämning_Bank.Domain.Entities;
using Inlämning_Bank.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inlämning_Bank.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Inlämning_Bank.Domain.Profiles;
using Microsoft.EntityFrameworkCore.Metadata;
using AutoMapper;

namespace Inlämning_Bank.Core.Services
{
    public class CustomerService: ICustomerService
    {
        private readonly ICustomerRepo _repo;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly IDispositionService _dispositionService;
        private readonly IUserService _userService;

        public CustomerService(ICustomerRepo repo, IMapper mapper, IAccountService accountService, IDispositionService dispositionService, IUserService userService)
        {
            _repo = repo;
            _mapper = mapper;
            _accountService = accountService;
            _dispositionService = dispositionService;
            _userService = userService;
        }

        public async Task<int> AddCustomer(NewCustomerDTO customerInfo)
        {
            //Först ska jag separera all kundinformation så jag kan lägga in en del i Customer-tabellen och en annan del i AspNetUser-tabellen. 
            var customer = _mapper.Map<Customer>(customerInfo);
            var userModel = _mapper.Map<ApplicationUser>(customerInfo);

            // Så sedan ska jag skapa en customer
            var returnedCustomer = await _repo.AddCustomer(customer);

            // Sen ge den en inloggning
            await _userService.AddApplicationUser(userModel, returnedCustomer, customerInfo.Password);

            // sedan kan jag öppna ett konto till hen. Sist måste jag koppla ihop kunden med kontot.
            return await _accountService.AddAccount(customerInfo.AccountTypeId, returnedCustomer.CustomerId);
        }

        public async Task<Customer> GetCustomerById(int id)
        {
            try
            {
                var customer = await _repo.GetCustomerById(id);

                return customer;

            }
            catch (Exception ex)
            {
                throw new Exception($"Kunde inte hämta kundinformation: \n{ex.Message}");
            }
        }

  
    }
}
