using AutoMapper;
using Inlämning_Bank.Domain.DTO;
using Inlämning_Bank.Domain.Entities;
using Inlämning_Bank.Domain.Identity;
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
            CreateMap<NewCustomerInfoDTO, Customer>()
            .ForMember(dest => dest.Gender,
            option=>option.MapFrom(src => src.Gender))
            .ForMember(dest => dest.Givenname,
            option => option.MapFrom(src => src.Givenname))
            .ForMember(dest => dest.Surname,
            option => option.MapFrom(src => src.Surname))
            .ForMember(dest => dest.Streetaddress,
            option => option.MapFrom(src => src.Streetaddress))
            .ForMember(dest => dest.City,
            option => option.MapFrom(src => src.City))
            .ForMember(dest => dest.Zipcode,
            option => option.MapFrom(src => src.Zipcode))
            .ForMember(dest => dest.Country,
            option => option.MapFrom(src => src.Country))
            .ForMember(dest => dest.CountryCode,
            option => option.MapFrom(src => src.CountryCode));

            CreateMap<NewCustomerInfoDTO, ApplicationUser>()
            .ForMember(dest => dest.UserName,
            option => option.MapFrom(src => src.Username));
            

            CreateMap<NewCustomerInfoDTO, Account>()
            .ForMember(dest => dest.AccountTypesId,
            option => option.MapFrom(src => src.AccountTypeId));
        }

    }
}
