using ECommerceApp.Application.DTOs.Customer;
using ECommerceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ECommerceApp.Application.Interfaces.Services
{
    public interface ICustomerUserService
    {
        public CustomerDto Login(LoginUserDto user);
        public void RegisterAccount(RegitserCustomerUserDto userDto);
    }
}
