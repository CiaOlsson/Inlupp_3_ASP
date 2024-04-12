using Inlämning_Bank.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Inlämning_Bank.Domain.Identity
{
    public class ApplicationUser : IdentityUser
    {
        //En användare får vissa properties. Men vi kan lägga på flera här om vi vill. 
        public Customer? Customer { get; set; }

    }
}
