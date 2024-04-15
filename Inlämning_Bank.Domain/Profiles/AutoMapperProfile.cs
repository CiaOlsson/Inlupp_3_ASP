using AutoMapper;
using Inlämning_Bank.Domain.DTO;
using Inlämning_Bank.Domain.Entities;
using Inlämning_Bank.Domain.Identity;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämning_Bank.Domain.Profiles
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {

            // från klienten till databasen

            CreateMap<NewCustomerDTO, Customer>();
            //.ForMember(dest => dest.Gender,
            //option=>option.MapFrom(src => src.Gender))
            //.ForMember(dest => dest.Givenname,
            //option => option.MapFrom(src => src.Givenname))
            //.ForMember(dest => dest.Surname,
            //option => option.MapFrom(src => src.Surname))
            //.ForMember(dest => dest.Streetaddress,
            //option => option.MapFrom(src => src.Streetaddress))
            //.ForMember(dest => dest.City,
            //option => option.MapFrom(src => src.City))
            //.ForMember(dest => dest.Zipcode,
            //option => option.MapFrom(src => src.Zipcode))
            //.ForMember(dest => dest.Country,
            //option => option.MapFrom(src => src.Country))
            //.ForMember(dest => dest.CountryCode,
            //option => option.MapFrom(src => src.CountryCode));

            CreateMap<NewCustomerDTO, ApplicationUser>()
            .ForMember(dest => dest.UserName,
            option => option.MapFrom(src => src.Username));


            //CreateMap<NewCustomerDTO, Account>()
            //.ForMember(dest => dest.AccountTypesId,
            //option => option.MapFrom(src => src.AccountTypeId));

            CreateMap<LoanDTO, Loan>();
            //.ForMember(dest => dest.AccountId,
            //option => option.MapFrom(src => src.AccountId))
            //.ForMember(dest=>dest.Duration,
            //option => option.MapFrom(src => src.Duration))
            //.ForMember(dest => dest.Amount,
            //option => option.MapFrom(src => src.Amount));

            CreateMap<MakeTransactionDTO, Transaction>()
                .ForMember(dest => dest.AccountId,
                option => option.MapFrom(src => src.FromAccount))
                .ForMember(dest => dest.Account,
                option => option.MapFrom(src => src.ToAccount));




            // Från databas till klienten

            CreateMap<Customer, CustomerProfileDTO>();

            CreateMap<Disposition, CustomerAccountDTO>()
                .ForMember(dest => dest.AccountType,
                option => option.MapFrom(src => src.Account.AccountTypes.TypeName))
                .ForMember(dest=>dest.AccountNumber,
                option=>option.MapFrom(src=>src.AccountId))
                .ForMember(dest => dest.AccountBalance,
                option => option.MapFrom(src => src.Account.Balance));

            CreateMap<Transaction, TransactionsDTO>()
                .ForMember(dest => dest.ToOrFromAccount,
                option => option.MapFrom(src => src.Account));

            

    }

    }
}
