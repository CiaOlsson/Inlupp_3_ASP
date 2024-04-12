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
    public class CustomerRepo : ICustomerRepo
    {
        private readonly BankAppDataContext _context;

        public CustomerRepo(BankAppDataContext context)
        {
            _context = context;
        }


        public async Task<Customer> AddCustomer(Customer customer)
        {
            _context.Add(customer);
            _context.SaveChanges();
            return customer;
        }

        public async Task<Customer> GetCustomerById(int id)
        {
            
            return _context.Customers.SingleOrDefault(c => c.CustomerId == id);
        }

        public async Task<List<Customer>> GetCustomers()
        {
            return _context.Customers.Where(c=>c.City == "VÄSTERVIK").ToList();
        }
    }
}
