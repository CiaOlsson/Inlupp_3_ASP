using AutoMapper;
using Inlämning_Bank.Core.Interfaces;
using Inlämning_Bank.Data.Interfaces;
using Inlämning_Bank.Domain.DTO;
using Inlämning_Bank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämning_Bank.Core.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepo _repo;
        private readonly IMapper _mapper;
        private readonly ITransactionService _transactionService;


        public LoanService(ILoanRepo repo, IMapper mapper, ITransactionService transactionService)
        {
            _repo = repo;
            _mapper = mapper;
            _transactionService = transactionService;
        }

        public async Task NewLoan(LoanDTO loanInfo)
        {
            var loan = _mapper.Map<Loan>(loanInfo);
            loan.Date = DateOnly.FromDateTime(DateTime.Now);
            loan.Status = "Running";
            loan.Payments = 0;

            //Först skapas ett nytt lån.
            await _repo.NewLoan(loan);

            //sedan skapas en transaktion i transactions tabellen. Samt att kontot kommer uppdateras med ny balance
            await _transactionService.AddTransaction(loan.AccountId, loan.Amount, "Credit", "Credit - Loan");

        }
    }
}
