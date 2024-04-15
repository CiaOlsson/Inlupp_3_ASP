using AutoMapper;
using Inlämning_Bank.Core.Interfaces;
using Inlämning_Bank.Data.Interfaces;
using Inlämning_Bank.Data.Repos;
using Inlämning_Bank.Domain.DTO;
using Inlämning_Bank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Inlämning_Bank.Core.Services
{
    public class TransactionService: ITransactionService
    {
        private readonly ITransactionRepo _repo;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public TransactionService(ITransactionRepo transactionRepo, IAccountService accountService, IMapper mapper)
        {
            _repo = transactionRepo;
            _accountService = accountService;
            _mapper = mapper;
        }

        public async Task TransferMoney(MakeTransactionDTO transaction)
        {

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await AddTransaction(transaction.FromAccount, (-1) * transaction.Amount, "Debit", "Transfer to another account", transaction.ToAccount.ToString());

                await AddTransaction(transaction.ToAccount, transaction.Amount, "Credit", "Transfer from another account", transaction.FromAccount.ToString());

                scope.Complete();
            }
        }

        public async Task AddTransaction(int transactionAccountId, decimal amount, string type, string operation, string? toAccountId = null)
        {

            var account = await _accountService.GetAccountById(transactionAccountId);

            if (account == null)
            {
                throw new Exception("Kontot hittades inte."); // Gäller både från-konto och till-konto. 
            }

            account.Balance += amount;
            if (account.Balance < 0)
            {
                throw new Exception("Det finns inte täckning på kontot");
            }

            Domain.Entities.Transaction transaction = new Domain.Entities.Transaction() {

                AccountId = transactionAccountId,
                Date = DateOnly.FromDateTime(DateTime.Now),
                Type = type,
                Operation = operation,
                Amount = amount,
                Balance = account.Balance,
                Symbol = null,
                Bank = null,
                Account = toAccountId,  
                AccountNavigation = account
            };

            await _accountService.UpdateAccount(account);

            await _repo.AddTransaction(transaction);

        }

        public async Task<List<TransactionsDTO>> GetTransactionsOnAccount(int id)
        {
            var list = await _repo.GetTransactionsOnAccount(id);

            List<TransactionsDTO> transactions = new List<TransactionsDTO>();
            foreach (var transaction in list)
            {
                var t = _mapper.Map<TransactionsDTO>(transaction);
                transactions.Add(t);
            }
            return transactions;
        }
    }
}
