using Inlämning_Bank.Domain.DTO;
using Inlämning_Bank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämning_Bank.Core.Interfaces
{
    public interface ICustomerService
    {
        Task<int> AddCustomer(NewCustomerDTO customerInfo);
        Task<Customer> GetCustomerById(int id);
        
    }
}
