using Inlämning_Bank.Domain.Entities;
using Inlämning_Bank.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämning_Bank.Core.Interfaces
{
    public interface IUserService
    {
        Task AddApplicationUser(ApplicationUser userModel, Customer returnedCustomer, string Password);
    }
}
