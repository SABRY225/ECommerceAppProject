using ECommerceApp.Application.DTOs.Customer;
using ECommerceApp.Application.Interfaces.Rebositories.ICustomerUserRepository;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Application.Services;
using ECommerceApp.Infrastructure.Data;
using ECommerceApp.Infrastructure.Repositories;

namespace Presentation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ApplicationDbContext dbContext = new();
            ICustomerUserRepository cartrepo =new CustomerUserRepository(dbContext);
            ICustomerUserService userService = new CustomerUserService(cartrepo);

            RegitserCustomerUserDto regitser = new() { FirstName = "sultan", LastName = "ali", Email = "sultan@gmail.com", Password = "123456", Address = "Qena", PhoneNumber = "01111818357" };
            userService.RegisterAccount(regitser);


            LoginUserDto userDto = new() { Email = "sultan@gmail.com", Password = "123456" };
            userService.Login(userDto);
        }
    }
}
