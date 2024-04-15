using Inlämning_Bank.Domain.DTO;
using Inlämning_Bank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämning_Bank.Core.Interfaces
{
    public interface ITransactionService
    {
        Task AddTransaction(int accountId, decimal amount, string type, string operation, string? toAccount = null);

        Task<List<TransactionsDTO>> GetTransactionsOnAccount(int id);
        Task TransferMoney(MakeTransactionDTO transaction);
    }
}
