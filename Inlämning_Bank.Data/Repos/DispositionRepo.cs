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
    public class DispositionRepo : IDispositionRepo
    {
        private readonly BankAppDataContext _context;

        public DispositionRepo(BankAppDataContext context)
        {
            _context = context;
        }

        public async Task AddDisposition(Disposition disposition)
        {

            _context.Dispositions.Add(disposition);
            _context.SaveChanges();
        }

        public async Task<List<Disposition>> GetDispositionsByCustomerId(int id)
        {
            var dispositions = _context.Dispositions
                                            .Include(d=>d.Account) 
                                            .Include(d => d.Account.AccountTypes)
                                            .Where(d => d.CustomerId == id)
                                            .ToList();

            return dispositions;
        }
    }
}
