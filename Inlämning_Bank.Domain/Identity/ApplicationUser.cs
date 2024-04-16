using Inlämning_Bank.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Inlämning_Bank.Domain.Identity
{
    public class ApplicationUser : IdentityUser
    {
        //Jag lägger till kunden här för att koppla ihop user som används till inloggning med en customer.
        public Customer? Customer { get; set; }

    }
}
