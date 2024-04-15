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
    public class TransactionRepo: ITransactionRepo
    {
        private readonly BankAppDataContext _context;

        public TransactionRepo(BankAppDataContext context)
        {
            _context = context;
        }

        public async Task AddTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }

        public async Task<List<Transaction>> GetTransactionsOnAccount(int id)
        {
            var transactions = _context.Transactions.Where(t=>t.AccountId == id).ToList();
            return transactions;
        }
    }
}
