using ECommerceApp.Application.DTOs.Customer;
using ECommerceApp.Application.Interfaces.Rebositories.ICustomerUserRepository;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Domain.Entities;
using Mapster;


namespace ECommerceApp.Application.Services
{
    public class CustomerUserService:ICustomerUserService
    {
        public ICustomerUserRepository UserRepository { get; }
        public CustomerUserService(ICustomerUserRepository userRepository)
        {
            UserRepository = userRepository;
        }


        public CustomerDto Login(LoginUserDto dto)
        {
            var user = UserRepository.GetUserByCredentials(dto.Email, dto.Password);

            if (user == null)
            {
                throw new Exception("Invalid data");
            }

            return user.Adapt<CustomerDto>();
        }


        public void RegisterAccount(RegistorcustomerUserDto userDto)
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

        //public void RegisterAccount(RegistorcustomerUserDto userDto)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
