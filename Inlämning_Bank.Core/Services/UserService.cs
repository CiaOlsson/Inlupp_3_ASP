using Inlämning_Bank.Core.Interfaces;
using Inlämning_Bank.Domain.Entities;
using Inlämning_Bank.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämning_Bank.Core.Services
{
    public class UserService:IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task AddApplicationUser(ApplicationUser userModel, Customer returnedCustomer, string password)
        {
            userModel.Customer = returnedCustomer;

            //Detta sparar användaren i databasen. 
            var result = await _userManager.CreateAsync(userModel, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(userModel, "Customer");
            }
            else
            {
                throw new Exception("Error occurred");
            }
        }
    }
}
