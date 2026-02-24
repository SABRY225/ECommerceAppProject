using ECommerceApp.Application.DTOs.Cart;
using ECommerceApp.Application.DTOs.Customer;
using ECommerceApp.Application.Interfaces.Rebositories.ICartRepository;
using ECommerceApp.Application.Interfaces.Rebositories.ICustomerUserRepository;
using ECommerceApp.Application.Interfaces.Services;
using ECommerceApp.Application.Mapping.CartMapping;
using ECommerceApp.Application.Services;
using ECommerceApp.Application.Services.CartService;
using ECommerceApp.Infrastructure.Data;
using ECommerceApp.Infrastructure.Repositories;
using ECommerceApp.Infrastructure.Repositories.CartRepository;
using System.Security;

namespace presention
{

    internal class Program
    {
        static void Main(string[] args)
        {
            ApplicationDbContext dbContext = new();
            ICustomerUserRepository cartrepo = new CustomerUserRepository(dbContext);
            ICustomerUserService userService = new CustomerUserService(cartrepo);

            //RegitserCustomerUserDto regitser = new() { FirstName = "sultan", LastName = "ali", Email = "sultan@gmail.com", Password = "123456", Address = "Qena", Phone = "01111818357" };
            //// userService.RegisterAccount(regitser);


            //LoginUserDto userDto = new() { Email = "sultan@gmail.com", Password = "123456" };
            //userService.Login(userDto);

            ICartRepository cartRepository = new CartRepository(dbContext);
            ICartService cartService = new CartService(cartRepository);
            //CartMapping.RegisterMappings();

            var dto = new CartItemRequest() { ProductId = 24, Quantity = 3, UserId = 6 };
            //var Cart= cartService.GetCustomerCartByID(4);
           // cartService.AddProductToCart(dto);
           // cartService.RemoveProductFromCart(6,24);
             //cartService.UpdateCartItem(dto);


            var Cart4 = cartService.GetAllCarts().ToList();
            //var Cart5 = cartService.GetAllCarts();
            

        }
    }
}


