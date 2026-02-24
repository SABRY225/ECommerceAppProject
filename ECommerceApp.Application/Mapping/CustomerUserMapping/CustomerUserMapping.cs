using ECommerceApp.Application.DTOs.Customer;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Infrastructure.Enums;
using Mapster;

namespace ECommerceApp.Application.Mapping.CustomerUserMapping
{
    public static class CustomerUserMapping
    {
        public static TypeAdapterConfig GetRegisterConfig()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<RegitserCustomerUserDto, User>()
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.Phone, src => src.Phone) 
                .Map(dest => dest.Address, src => src.Address)
                .Map(dest => dest.Password, src => src.Password)
                .Map(dest => dest.Role, src => UserRole.Customer); 

            return config;
        }

        public static TypeAdapterConfig GetResponseConfig()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<User, CustomerDto>()
                .Map(dest => dest.FirstName, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.PhoneNumber, src => src.Phone)
                .Map(dest => dest.Address, src => src.Address);

            return config;
        }
    }
}