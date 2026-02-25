using ECommerceApp.Application.DTOs.Customer;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Infrastructure.Enums;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Mapping.CustomerUserMapping
{
    public static class CustomerUserMapping
    {
    public static void Register()
        {
            TypeAdapterConfig<LoginUserDto, User>
                .NewConfig()
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.Password, src => src.Password)
                .IgnoreNonMapped(true);

            TypeAdapterConfig<RegitserCustomerUserDto, User>
                .NewConfig()
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.Phone, src => src.PhoneNumber)
                .Map(dest => dest.Address, src => src.Address)
                .Map(dest => dest.Password, src => src.Password)
                .Map(dest => dest.Role, src => UserRole.Customer);

            TypeAdapterConfig<User, CustomerDto>
                .NewConfig()
                .Map(dest => dest.PhoneNumber, src => src.Phone);
        }
    }
    //public void LoginMapping()
    //{
    //    TypeAdapterConfig<LoginUserDto, User>
    //        .NewConfig()
    //        .Map(dest => dest.Email, src => src.Email)
    //        .Map(dest => dest.Password, src => src.Password)
    //        .IgnoreNonMapped(true);
    //}
    //public void RegisterCustomerMapping()
    //{
    //    TypeAdapterConfig<RegitserCustomerUserDto, User>
    //        .NewConfig()

    //        .Map(dest => dest.FirstName, src => src.FirstName)
    //        .Map(dest => dest.LastName, src => src.LastName)
    //        .Map(dest => dest.Email, src => src.Email)
    //        .Map(dest => dest.Phone, src => src.PhoneNumber)
    //        .Map(dest => dest.Address, src => src.Address)
    //        .Map(dest => dest.Password, src => src.Password)
    //        .Map(dest => dest.Role, src => UserRole.Customer);
    //}
    //public void UserResponseMapping()
    //{
    //    TypeAdapterConfig<User, CustomerDto>
    //        .NewConfig()

    //        .Map(dest => dest.FirstName, src => src.FirstName)
    //        .Map(dest => dest.LastName, src => src.LastName)
    //        .Map(dest => dest.PhoneNumber, src => src.Phone) 
    //        .Map(dest => dest.Address, src => src.Address);
    //}
}
