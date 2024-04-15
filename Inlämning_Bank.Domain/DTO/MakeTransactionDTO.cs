using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämning_Bank.Domain.DTO
{
    public class MakeTransactionDTO
    {
        public int FromAccount {  get; set; }
        public int ToAccount {  get; set; }
        public decimal Amount { get; set; }
    }
}
