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
        }
    }
}
