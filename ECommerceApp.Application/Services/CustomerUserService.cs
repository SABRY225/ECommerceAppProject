using ECommerceApp.Application.DTOs.Customer;
using ECommerceApp.Application.Interfaces.Rebositories;
using ECommerceApp.Application.Interfaces.Rebositories.ICustomerUserRepository;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Application.Services
{
    public class CustomerUserService:ICustomerUserService
    {
        public ICustomerUserRepository UserRepository { get; }
        public IGenericRebository<User> _repo { get; }

        public CustomerUserService(ICustomerUserRepository userRepository ,IGenericRebository<User> genericRebository)
        {
            UserRepository = userRepository;
            _repo = genericRebository;
        }


        public CustomerDto Login(LoginUserDto dto)
        {
            var user = UserRepository.GetUserByCredentials(dto.Email, dto.Password);

            return user == null ? throw new Exception("Invalid data") : user.Adapt<CustomerDto>();
        }


        public void RegisterAccount(RegitserCustomerUserDto userDto)
        {
            var existingUser = UserRepository.GetUserEmail(userDto.Email);
            if (existingUser != null)
            {
                throw new Exception("is Already Exits");
            }

            var userEntity = userDto.Adapt<User>();


            UserRepository.RegisterCustomerAccount(userEntity);
            UserRepository.SaveChanges();
        }

        public async Task<List<CustomerForAdminDto>> Customers()
        {
            var customers = await _repo.GetAll().Include(u=>u.Orders).ToListAsync();
            return customers.Adapt<List<CustomerForAdminDto>>();
        }

        //public void RegisterAccount(RegistorcustomerUserDto userDto)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
