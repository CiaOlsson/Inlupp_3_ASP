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
    public class DispositionService : IDispositionService
    {
        private readonly IDispositionRepo _repo;

        public DispositionService(IDispositionRepo dispositionRepo)
        {
            _repo = dispositionRepo;
        }

        public async Task AddDisposition(int customerId, int accountId)
        {
            Disposition disposition = new Disposition()
            {
                CustomerId = customerId,
                AccountId = accountId
            };

            await _repo.AddDisposition(disposition);
        }
    }
}
