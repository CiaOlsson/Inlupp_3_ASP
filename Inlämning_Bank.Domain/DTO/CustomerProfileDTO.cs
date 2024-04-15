﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämning_Bank.Domain.DTO
{
    public class CustomerProfileDTO
    {
        public string Givenname { get; set; } = null!;

        public string Surname { get; set; } = null!;

        public string Streetaddress { get; set; } = null!;

        public string City { get; set; } = null!;

        public string Zipcode { get; set; } = null!;

        public string Country { get; set; } = null!;

        public string CountryCode { get; set; } = null!;

        public DateOnly? Birthday { get; set; }

        public string? Telephonecountrycode { get; set; }

        public string? Telephonenumber { get; set; }

        public string? Emailaddress { get; set; }
    }
}
