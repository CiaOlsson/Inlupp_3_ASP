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
    public class LoanRepo : ILoanRepo
    {
        private readonly BankAppDataContext _context;

        public LoanRepo(BankAppDataContext context)
        {
            _context = context;
        }

        public async Task NewLoan(Loan loan)
        {
            _context.Loans.Add(loan);
            _context.SaveChanges();
        }
    }
}
