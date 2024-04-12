using Inlämning_Bank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämning_Bank.Domain.DTO
{
    public class NewCustomerInfoDTO
    {
        //Det första här är för att kunna skapa en ny customer
        public int CustomerId { get; set; }

        public string Gender { get; set; } = null!;

        public string Givenname { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string Streetaddress { get; set; } = null!;

        public string City { get; set; } = null!;

        public string Zipcode { get; set; } = null!;

        public string Country { get; set; } = null!;

        public string CountryCode { get; set; } = null!;

        //Följande är för att kunna skapa en inloggning till den nya kunden. 
        public string Username { get; set; }
        public string Password { get; set; }

        // Detta är för att kunna ge kunden ett första konto, antingen ett sparkonto eller lönekonto. 
        public int AccountTypeId { get; set; }

    }
}
