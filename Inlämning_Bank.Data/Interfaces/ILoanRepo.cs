using Inlämning_Bank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämning_Bank.Data.Interfaces
{
    public interface ILoanRepo
    {
        Task NewLoan(Loan loan);
    }
}
