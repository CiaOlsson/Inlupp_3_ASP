using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämning_Bank.Domain.DTO
{
    public class CustomerAccountDTO
    {
        public string AccountType { get; set; }
        public int AccountNumber { get; set; }
        public decimal AccountBalance { get; set; }
    }
}
