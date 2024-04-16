using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämning_Bank.Domain.DTO
{
    public class LoanDTO
    {
        // Jag ska göra så att kontonummer returneras till swagger när man skapar en kund eller ett konto så får man skriva ner det. 

        // mata in vilket konto det ska hamna på. Man måste veta konto id för en kund kan ju ha flera konton.. 
        public int AccountId { get; set; } 

        // mata in i bodyn hur stort lånet ska vara, det ska sedan sättas in på kontot efter att lånet är skapat. 
        public decimal Amount { get; set; }

        // mata in i bodyn antal månader.
        public int DurationMonths { get; set; }


    }
}
